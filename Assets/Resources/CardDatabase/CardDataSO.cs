using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card")]
public class CardDataSO : ScriptableObject
{
    [Header("������Ϣ")]
    public int cardID;
    public string cardName;
    public Sprite cardArtwork; // ����ͼƬ
    public string description;

    [Header("��������")]
    public ElementCost cost; // ����

    [Header("��������")]
    public CardType cardType;
    public Rarity rarity;

    [Header("����Ч��")]
    public List<CardEffectData> effects = new List<CardEffectData>();

    // **ִ�п���Ч��**
    public void ExecuteEffects(GameObject target)
    {
        foreach (var effect in effects)
        {
            CardEffectManager.ExecuteEffect(effect.effectName, target, effect.GetParametersAsObjects());
        }
    }
}

// **�������ͣ�����չ��**
public enum CardType
{
    Attack,
    Survive,
    Special
}

// **ϡ�жȣ�����չ��**
public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

// **���Ʒ���**
[System.Serializable] // �� Unity Inspector ������ʾ
public struct ElementCost
{
    public int fire;  // ��Ԫ�ط���
    public int water; // ˮԪ�ط���
    public int earth; // ��Ԫ�ط���

    // **���췽��**
    public ElementCost(int fire, int water, int earth)
    {
        this.fire = fire;
        this.water = water;
        this.earth = earth;
    }

    // **���ط����ַ���**
    public override string ToString()
    {
        return $"{fire} {water} {earth}";
    }
}

// **����Ч������**
[System.Serializable]
public class CardEffectData
{
    public string effectName; // Ч������
    public List<EffectParameter> parameters = new List<EffectParameter>(); // �ò����� Inspector ��ʾ

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
                    //Debug.Log($"�� {target.name} ��� {damage} ���˺�");
                }
                else
                {
                    //Debug.LogWarning("DealDamage ��Ҫһ�� int ����");
                }
            }
        },
        { "Heal", (target, args) =>
            {
                if (args.Length > 0 && args[0] is int healAmount)
                {
                    //target.GetComponent<Health>().Heal(healAmount);
                    //Debug.Log($"{target.name} �ظ��� {healAmount} ������ֵ");
                }
                else
                {
                    //Debug.LogWarning("Heal ��Ҫһ�� int ����");
                }
            }
        },
        { "ApplyDebuff", (target, args) =>
            {
                if (args.Length > 0 && args[0] is string debuffName)
                {
                    //target.GetComponent<DebuffManager>().ApplyDebuff(debuffName);
                    //Debug.Log($"{target.name} ��ʩ���� {debuffName} ״̬");
                }
                else
                {
                    //Debug.LogWarning("ApplyDebuff ��Ҫһ�� string ����");
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
            //Debug.LogWarning($"Ч�� {effectName} δע�ᣡ");
        }
    }
}

// **�ò��������л�**
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

// **֧�ֵĲ�������**
public enum ParameterType
{
    String,
    Int,
    Float
}
