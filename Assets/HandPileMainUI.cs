using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class HandPileMainUI: MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    public int pileSize;
    [SerializeField] private Vector3 pileCenterPosition;
    [SerializeField] private float pileRadius=100;
    public bool hoverOnCard = false;
    [SerializeField] private int currentHoverCardIndex;

    [SerializeField] private Vector3 serveCardPoint;
    public float serveCardInterval = 0.5f;

    [SerializeField] private HandCardUIInstance cardUIPfb;
    [SerializeField] private List<HandCardUIInstance> cardsUIList = new List<HandCardUIInstance>();

    public float smoothSpeed = 0.4f;
    [SerializeField] private float deltaAngle = 4;
    [SerializeField] private float deltaRadius = 10;
    private float unitAngle = 0;

    [SerializeField] private float hoverDeltaConst = 30f;

    public float playCardY = 500f;

    private bool drawingCards = false;

    public void UpdateHandPile()
    {
        if (!drawingCards)
        {
            StartCoroutine(DrawCardEffect());
        }
    }
    public IEnumerator DrawCardEffect()
    {
        drawingCards = true;
        for (int i= pileSize; i < cardManager.HandPile.Count;i++)
        {
            HandCardUIInstance tmpCard = Instantiate(cardUIPfb, serveCardPoint, Quaternion.identity, transform);
            tmpCard.InitCardUI(this,cardManager.HandPile[i],i);
            cardsUIList.Add(tmpCard);
            pileSize = cardsUIList.Count;
            //暂时放在这里
            CalculateCardsPosAndRot();
            yield return new WaitForSecondsRealtime(serveCardInterval);
        }
        drawingCards = false;
        yield break;
    }
    public void SetHoverInfo(bool hover,int index)
    {
        hoverOnCard = hover;
        currentHoverCardIndex = index;
    }

    public void CalculateCardsPosAndRot()
    {
        
        Vector3 basePos = Vector3.zero;
        float euler = 0;

        for (int i=0;i< pileSize; i++)
        {
            
            if (pileSize % 2 == 0)
            {
                euler = ((float)(pileSize - 1) / 2 - i) * deltaAngle + 90;
            }
            else if(pileSize % 2 == 1)
            {
                euler = ((float)(pileSize - 1) / 2 - i) * deltaAngle + 90;
            }
            else
            {
                Debug.Log("有问题了");
            }

            float minusRadius = Mathf.Abs(i-(float)(pileSize - 1) / 2)*deltaRadius;

            basePos = new Vector3(Mathf.Cos(Mathf.Deg2Rad* euler), Mathf.Sin(Mathf.Deg2Rad * euler), 0) * (pileRadius- minusRadius) + pileCenterPosition;


            Vector3 hoverDeltaPos = Vector3.zero;
            if (hoverOnCard)
            {
                if (i != currentHoverCardIndex)
                {
                    hoverDeltaPos = new Vector3(1f / Mathf.Pow((i - currentHoverCardIndex), 1) * hoverDeltaConst, 0, 0);
                    cardsUIList[i].SetHover(false);
                }
                else
                {
                    euler = 90;
                    cardsUIList[i].SetTarget(basePos + hoverDeltaPos, (euler - 90) - unitAngle);
                    cardsUIList[i].SetHover(true);
                    continue;
                }
            }
            else
            {
                cardsUIList[i].SetHover(false);
            }

            cardsUIList[i].SetTarget(basePos+ hoverDeltaPos, (euler - 90)-unitAngle);


        }

        
    }
    public void Discard(HandCardUIInstance cardUIInstance)
    {
        bool flag = false;
        for(int i=0;i< cardsUIList.Count;i++)
        {
            if (cardsUIList[i]== cardUIInstance)
                flag = true;
            if(flag)
            {
                cardsUIList[i]._index--;
            }
        }
        cardsUIList.Remove(cardUIInstance);
        pileSize = cardsUIList.Count;
        CalculateCardsPosAndRot();
    }
    
}
