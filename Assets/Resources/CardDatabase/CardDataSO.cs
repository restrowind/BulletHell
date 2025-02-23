using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card")]
public class CardDataSO : ScriptableObject
{
    [Header("基本信息")]
    public int cardID;
    public string cardName;
    public Sprite cardArtwork; // 卡牌图片
    public string description;

    [Header("卡牌属性")]
    public ElementCost cost; // 费用

    [Header("卡牌类型")]
    public CardType cardType;
    public Rarity rarity;

    [Header("卡牌效果")]
    public List<CardEffectData> effects = new List<CardEffectData>();

    // **执行卡牌效果**
    public void ExecuteEffects(GameObject target)
    {
        foreach (var effect in effects)
        {
            CardEffectManager.ExecuteEffect(effect.effectName, target, effect.GetParametersAsObjects());
        }
    }
}

// **卡牌类型（可扩展）**
public enum CardType
{
    Attack,
    Survive,
    Special
}

// **稀有度（可扩展）**
public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

// **卡牌费用**
[System.Serializable] // 让 Unity Inspector 可以显示
public struct ElementCost
{
    public int fire;  // 火元素费用
    public int water; // 水元素费用
    public int earth; // 土元素费用

    // **构造方法**
    public ElementCost(int fire, int water, int earth)
    {
        this.fire = fire;
        this.water = water;
        this.earth = earth;
    }

    // **返回费用字符串**
    public override string ToString()
    {
        return $"{fire} {water} {earth}";
    }
}

// **卡牌效果数据**
[System.Serializable]
public class CardEffectData
{
    public string effectName; // 效果名称
    public List<EffectParameter> parameters = new List<EffectParameter>(); // 让参数在 Inspector 显示

    public object[] GetParametersAsObjects()
    {
        object[] paramArray = new object[parameters.Count];
        for (int i = 0; i < parameters.Count; i++)
        {
            paramArray[i] = parameters[i].GetValue();
        }
        return paramArray;
    }
}
public static class CardEffectManager
{
    private static Dictionary<string, Action<GameObject, object[]>> effectRegistry = new Dictionary<string, Action<GameObject, object[]>>
    {
        { "DealDamage", (target, args) =>
            {
                if (args.Length > 0 && args[0] is int damage)
                {
                    //target.GetComponent<Health>().TakeDamage(damage);
                    //Debug.Log($"对 {target.name} 造成 {damage} 点伤害");
                }
                else
                {
                    //Debug.LogWarning("DealDamage 需要一个 int 参数");
                }
            }
        },
        { "Heal", (target, args) =>
            {
                if (args.Length > 0 && args[0] is int healAmount)
                {
                    //target.GetComponent<Health>().Heal(healAmount);
                    //Debug.Log($"{target.name} 回复了 {healAmount} 点生命值");
                }
                else
                {
                    //Debug.LogWarning("Heal 需要一个 int 参数");
                }
            }
        },
        { "ApplyDebuff", (target, args) =>
            {
                if (args.Length > 0 && args[0] is string debuffName)
                {
                    //target.GetComponent<DebuffManager>().ApplyDebuff(debuffName);
                    //Debug.Log($"{target.name} 被施加了 {debuffName} 状态");
                }
                else
                {
                    //Debug.LogWarning("ApplyDebuff 需要一个 string 参数");
                }
            }
        }
    };

    public static void ExecuteEffect(string effectName, GameObject target, object[] parameters)
    {
        if (effectRegistry.TryGetValue(effectName, out var effectAction))
        {
            effectAction(target, parameters);
        }
        else
        {
            //Debug.LogWarning($"效果 {effectName} 未注册！");
        }
    }
}

// **让参数可序列化**
[System.Serializable]
public class EffectParameter
{
    public ParameterType type;
    public string stringValue;
    public int intValue;
    public float floatValue;

    public object GetValue()
    {
        return type switch
        {
            ParameterType.String => stringValue,
            ParameterType.Int => intValue,
            ParameterType.Float => floatValue,
            _ => null
        };
    }
}

// **支持的参数类型**
public enum ParameterType
{
    String,
    Int,
    Float
}
