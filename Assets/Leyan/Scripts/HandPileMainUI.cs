using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class HandPileMainUI: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public event Action<CardState> NotifyCanPlay;


    private bool discardingCards = false;
    private int needDiscardCount;
    [SerializeField] private Vector3 discardedCardsCenter;
    private int hasDiscardedCount => discardedCardsIndexs.Count;
    public List<int> discardedCardsIndexs = new List<int>();
    private float discardAreaHalfWidthMax = 800f;
    private float discardGap = 500f;

    private bool directToNextState = true;
    [SerializeField] private BattleManager _battleManager;

    public bool canDiscardMore()
    {
        if(hasDiscardedCount< needDiscardCount)
        {
            return true;
        }
        else
            return false;
    }

    public void StartDiscard(int discardCount)
    {
        discardingCards = true;
        needDiscardCount = discardCount;
    }

    public static HandPileMainUI Instance { get; private set; }
    private void Awake()
    {
        // 确保单例模式
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardManager.forceDisplayHandPile = true;
    }

    // 鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        cardManager.forceDisplayHandPile = false;
    }

    public void CalculateCardsPosAndRot()
    {
        if (!discardingCards)
        {
            Vector3 basePos = Vector3.zero;
            float euler = 0;

            for (int i = 0; i < pileSize; i++)
            {

                if (pileSize % 2 == 0)
                {
                    euler = ((float)(pileSize - 1) / 2 - i) * deltaAngle + 90;
                }
                else if (pileSize % 2 == 1)
                {
                    euler = ((float)(pileSize - 1) / 2 - i) * deltaAngle + 90;
                }
                else
                {
                    Debug.Log("有问题了");
                }

                float minusRadius = Mathf.Abs(i - (float)(pileSize - 1) / 2) * deltaRadius;

                basePos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * euler), Mathf.Sin(Mathf.Deg2Rad * euler), 0) * (pileRadius - minusRadius) + pileCenterPosition;


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

                cardsUIList[i].SetTarget(basePos + hoverDeltaPos, (euler - 90) - unitAngle);


            }
        }

        else
        {
            if (discardedCardsIndexs.Count <= 4)
            {
                foreach (int index in discardedCardsIndexs)
                {
                    cardsUIList[index].SetTarget(new Vector3(discardGap / 2 * ((1 - hasDiscardedCount) 
                        + discardedCardsIndexs.IndexOf(index) * 2), 0, 0) + discardedCardsCenter, 0);
                }
            }
            else
            {
                float gap = discardAreaHalfWidthMax * 2 / (((float)discardedCardsIndexs.Count - 1));
                for(int i=0;i< discardedCardsIndexs.Count;i++)
                {
                    cardsUIList[discardedCardsIndexs[i]].SetTarget(new Vector3(-discardAreaHalfWidthMax + i * gap, 0, 0)
                        + discardedCardsCenter, 0);
                }
            }


            Vector3 basePos = Vector3.zero;
            float euler = 0;

            int deltaNum = 0;

            for (int i = 0; i < pileSize; i++)
            {
                if(discardedCardsIndexs.Contains(cardsUIList[i]._index))
                {
                    deltaNum++;
                    continue;
                }
                if ((pileSize - hasDiscardedCount) % 2 == 0)
                {
                    euler = ((float)(pileSize - hasDiscardedCount - 1) / 2 - i+ deltaNum) * deltaAngle + 90;
                }
                else if ((pileSize - hasDiscardedCount) % 2 == 1)
                {
                    euler = ((float)(pileSize- hasDiscardedCount - 1) / 2 - i+ deltaNum) * deltaAngle + 90;
                }
                else
                {
                    Debug.Log("有问题了");
                }

                float minusRadius = Mathf.Abs(i- deltaNum - (float)(pileSize - hasDiscardedCount - 1) / 2) * deltaRadius;

                basePos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * euler), Mathf.Sin(Mathf.Deg2Rad * euler), 0) * (pileRadius - minusRadius) + pileCenterPosition;


                Vector3 hoverDeltaPos = Vector3.zero;
                if (hoverOnCard)
                {
                    int deltaFlag = 0;
                    for(int j=0;j< currentHoverCardIndex;j++)
                    {
                        if(discardedCardsIndexs.Contains(j))
                        {
                            deltaFlag++;
                        }
                    }
                    if (i!= currentHoverCardIndex)
                    {
                        hoverDeltaPos = new Vector3(1f / Mathf.Pow((i- deltaNum - currentHoverCardIndex+ deltaFlag), 1) * hoverDeltaConst, 0, 0);
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

                cardsUIList[i].SetTarget(basePos + hoverDeltaPos, (euler - 90) - unitAngle);


            }
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

    public void SetCanPlayCards(CardState cardState)
    {
        NotifyCanPlay?.Invoke(cardState);
    }
    public void ConfirmDiscard()
    {
        if (needDiscardCount == hasDiscardedCount)
        {
            List<HandCardUIInstance> tmpList = new List<HandCardUIInstance>();
            foreach (int index in discardedCardsIndexs)
            {
                CardInstance tmpCardInstance = cardsUIList[index].counterpartCardInstance;
                cardManager.DiscardToDiscardPile(tmpCardInstance);
                tmpList.Add(cardsUIList[index]);
            }
            foreach (HandCardUIInstance card in tmpList)
            {
                cardsUIList.Remove(card);
                Destroy(card.gameObject);
            }
            pileSize = cardsUIList.Count;
            for (int i = 0; i < cardsUIList.Count; i++)
            {
                cardsUIList[i]._index = i;

            }
            discardedCardsIndexs.Clear();
            cardManager.DiscardOver();
            if (directToNextState)
            {
                _battleManager.MoveToNextState();
                cardManager.SetCardState(CardState.Other);
                discardingCards = false;
            }
        }
        else
        {
            string display = "Choose " + (needDiscardCount-hasDiscardedCount).ToString()+ " more cards to discard";
            cardManager.SpawnATipBoard(display);
        }
    }

}
