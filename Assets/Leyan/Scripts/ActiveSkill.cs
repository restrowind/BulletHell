using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ActiveSkill : MonoBehaviour
{
    private int currentSkillID;
    private int remainTimes;
    [SerializeField] private List<Sprite> activeSkillSpriteList= new List<Sprite>();
    [SerializeField] private Image spriteUI;
    [SerializeField] private TextMeshProUGUI remainTimesUI;
    [SerializeField] private TextMeshProUGUI enterUI;
    private Action skillEffect;

    public void ActivateSkill()
    {
        if (remainTimes > 0)
        {
            remainTimes--;
            if (remainTimes <= 0)
            {
                currentSkillID = 0;
            }

            skillEffect?.Invoke();
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch (currentSkillID)
        {
            case 0:
                spriteUI.color = new Color(1, 1, 1, 0);
                enterUI.text = "";
                remainTimesUI.text = "";
                break;
            case 1:
            case 2:
                spriteUI.color = new Color(1, 1, 1, 1);
                enterUI.text = "Enter";
                remainTimesUI.text = remainTimes.ToString();
                break;
        }
        spriteUI.sprite = activeSkillSpriteList[currentSkillID];
        
    }

    public void LoadActiveSkill(int skillID,int times,Action effect)
    {
        currentSkillID=skillID;
        remainTimes=times;
        skillEffect=effect;
        UpdateSprite();
    }
    private void Start()
    {
        UpdateSprite();
    }
}
