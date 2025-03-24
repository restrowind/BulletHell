using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandCardUIInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardInstance counterpartCardInstance;
    public int _index;
    private HandPileMainUI mainUI;
    private float smoothSpeed;
    private Vector3 targetPosition;
    [SerializeField] private float targetEuler;
    private float currentEuler;
    private bool isHovering = false;
    [SerializeField] private float hoverY = 0f;
    [SerializeField] private float magnifyRate = 1.5f;

    private bool isDragging=false;
    private CardManager cardManager;

    [SerializeField] private Image cardArtwork;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;

    private CardState _cardState=CardState.Other;
    public bool isDiscarded = false;

    [SerializeField] private Image cardFrame;
    [SerializeField] private Image cardNameBoard;

    [SerializeField] private List<Sprite> frameList = new List<Sprite>();
    [SerializeField] private List<Sprite> fnameBoardList = new List<Sprite>();


    public void InitCardUI(HandPileMainUI handPileMainUI, CardInstance card,int index)
    {
        mainUI = handPileMainUI;
        counterpartCardInstance = card;
        _index = index;
        smoothSpeed = handPileMainUI.smoothSpeed;
        cardManager=FindAnyObjectByType<CardManager>();
        PrintCard();
        mainUI.NotifyCanPlay+=SetCardState;
        _cardState = cardManager.currentCardState;
    }
    // �����ͣ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDiscarded)
        {
            //Debug.Log("�����ͣ�� Image ��");
            mainUI.SetHoverInfo(true, _index);
            mainUI.CalculateCardsPosAndRot();
            //GlobalAudioPlayer.Instance.Play("CardHover");
        }
    }

    // ����뿪
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDiscarded)
        {
            //Debug.Log("����뿪 Image");
            mainUI.SetHoverInfo(false, _index);
            mainUI.CalculateCardsPosAndRot();
        }
    }

    // �����
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_cardState == CardState.Play)
            {
                SetDragging(true);

                if (Input.mousePosition.y > mainUI.playCardY)
                {
                    TryPlayCard();
                }
                //Debug.Log("�����϶�ģʽ��" + gameObject.name);
            }
            else if(_cardState == CardState.Discard)
            {
                if (Input.mousePosition.y > mainUI.playCardY)
                {
                    SetDiscard(false);
                }
                else
                {
                    if (mainUI.canDiscardMore())
                    {
                        SetDiscard(true);
                    }
                    else
                    {
                        cardManager.SpawnATipBoard("No need to discard more");
                    }
                }
                
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SetDragging(false);
            //Debug.Log("ȡ���϶���" + gameObject.name);

        }
    }

    private void SetDiscard(bool discarded)
    {
        isDiscarded = discarded;
        if(isDiscarded)
        {
            mainUI.discardedCardsIndexs.Add(_index);
            mainUI.SetHoverInfo(false, _index);
            SetHover(false);
            mainUI.CalculateCardsPosAndRot();
        }
        else
        {
            mainUI.discardedCardsIndexs.Remove(_index);
            mainUI.CalculateCardsPosAndRot();
        }
    }

    // ��ʼ�϶�
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        SetDragging(true);
    }

    // �϶���
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        transform.position = Input.mousePosition; // �ÿ��Ƹ������
    }

    // �����϶�
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        SetDragging(false);
        //Debug.Log("�϶�����");

        if (Input.mousePosition.y > mainUI.playCardY)
        {
            if (isDragging)
            {
                TryPlayCard();
            }
        }
        else
        {
            SetDragging(true);
        }
    }

    public void SetTarget(Vector3 pos,float euler)
    {
        targetPosition = pos;
        targetEuler = euler;
    }
    public void SetHover(bool hovering)
    {
        if (!isDiscarded)
        {
            isHovering = hovering;
            if (isHovering)
            {
                targetPosition = new Vector3(targetPosition.x, hoverY, 0);
                targetEuler = 0;
            }
        }
    }

    private void HandleSmoothStep()
    {
        if (!isDragging)
        {
            if (isHovering)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 3 * smoothSpeed*Time.deltaTime);
                currentEuler = Mathf.Lerp(currentEuler, targetEuler, 3 * smoothSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(0, 0, currentEuler);

                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * magnifyRate, 3 * smoothSpeed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothSpeed * Time.deltaTime);
                currentEuler = Mathf.Lerp(currentEuler, targetEuler, smoothSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(0, 0, currentEuler);

                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, smoothSpeed * Time.deltaTime);
            }
        }
    }

    private void Update()
    {
        HandleSmoothStep();

        if (isDragging)
        {
            transform.position = Input.mousePosition; // �ÿ��Ƹ������
        }
    }
    private void TryPlayCard()
    {
        if(cardManager.TryPlayCard(this))
        {
            mainUI.hoverOnCard = false;
            mainUI.Discard(this);

            //�����Ч����ʱֱ�����٣�
            Destroy(gameObject);
        }
    }

    private void SetDragging(bool dragging)
    {
        if (_cardState==CardState.Play)
        {
            isDragging = dragging;
            if (dragging)
            {
                transform.SetSiblingIndex(mainUI.pileSize);
            }
            else
            {
                transform.SetSiblingIndex(_index);
            }
        }
    }

    private void PrintCard()
    {
        cardArtwork.sprite= cardManager.GetCardByID(counterpartCardInstance.cardID).cardArtwork;
        cardName.text= cardManager.GetCardByID(counterpartCardInstance.cardID).cardName;
        cardDescription.text = cardManager.GetCardByID(counterpartCardInstance.cardID).description;


        cardFrame.sprite = cardManager.GetCardByID(counterpartCardInstance.cardID).cardArtwork;


        
        cardFrame.sprite = frameList[(int)cardManager.GetCardByID(counterpartCardInstance.cardID).elementType];
        cardNameBoard.sprite = fnameBoardList[(int)cardManager.GetCardByID(counterpartCardInstance.cardID).elementType];


        PrintCardCost();

    }

    [SerializeField] private Transform pivot;
    [SerializeField] private float interval;
    [SerializeField] private GameObject cosUnitPfb;
    private void PrintCardCost()
    {
        int flag = 0;
        CardDataSO cardData= cardManager.GetCardByID(counterpartCardInstance.cardID);
        int[] costList =new int[3];
        costList[0] = cardData.cost.aqua;
        costList[1] = cardData.cost.lumen;
        costList[2] = cardData.cost.vitality;
        for(int i=0;i< cardData.cost.aqua;i++)
        {
            CostUnit unit = Instantiate(cosUnitPfb, transform).GetComponent<CostUnit>();
            unit.OnInit(CostUnit.CostType.Aqua, pivot.position + Vector3.right * flag * interval);
            flag++;
        }
        for (int i = 0; i < cardData.cost.lumen; i++)
        {
            CostUnit unit = Instantiate(cosUnitPfb, transform).GetComponent<CostUnit>();
            unit.OnInit(CostUnit.CostType.Lumen, pivot.position + Vector3.right * flag * interval);
            flag++;
        }
        for (int i = 0; i < cardData.cost.vitality; i++)
        {
            CostUnit unit = Instantiate(cosUnitPfb, transform).GetComponent<CostUnit>();
            unit.OnInit(CostUnit.CostType.Vitality, pivot.position + Vector3.right * flag * interval);
            flag++;
        }

    }




    private void SetCardState(CardState cardState)
    {
        _cardState = cardState;
    }

}
