using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostUnit : MonoBehaviour
{

    public enum CostType
    {
        Aqua,
        Lumen,
        Vitality
    }

    private CostType costType;
    public List<Sprite> spriteList = new List<Sprite>();
    private Image img;
    public void OnInit(CostType type,Vector3 position)
    {
        img=GetComponent<Image>();
        costType = type;
        switch (costType)
        {
            case CostType.Aqua:
                img.sprite = spriteList[0];
                break;
            case CostType.Lumen:
                img.sprite = spriteList[1];
                break;
            case CostType.Vitality:
                img.sprite = spriteList[2];
                break;
        }
        transform.localPosition = position;
    }
}
