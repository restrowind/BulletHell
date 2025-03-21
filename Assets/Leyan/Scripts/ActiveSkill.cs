using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveSkill : MonoBehaviour
{
    private int currentSkillID;
    private int remainTimes;
    [SerializeField] private List<Sprite> activeSkillSpriteList= new List<Sprite>();
    [SerializeField] private Image spriteUI;
    [SerializeField] private TextMeshProUGUI remainTimesUI;
    [SerializeField] private Image enterUI;

    public void ActivateSkill()
    {
        if (remainTimes > 0)
        {
            remainTimes--;
            if (remainTimes <= 0)
            {
                currentSkillID = 0;
            }

            switch (currentSkillID)
            {
                case 1:

                    break;

                case 2:

                    break;
            }
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch (currentSkillID)
        {
            case 0:
                spriteUI.color = new Color(1, 1, 1, 0);
                enterUI.color = new Color(1, 1, 1, 0);
                remainTimesUI.text = "";
                break;
            case 1:
            case 2:
                spriteUI.color = new Color(1, 1, 1, 1);
                enterUI.color = new Color(1, 1, 1, 1);
                remainTimesUI.text = remainTimes.ToString();
                break;
        }
        spriteUI.sprite = activeSkillSpriteList[currentSkillID];
        
    }

    public void LoadActiveSkill(int skillID,int times)
    {
        currentSkillID=skillID;
        remainTimes=times;
        UpdateSprite();
    }
    private void Start()
    {
        UpdateSprite();
    }
}
