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

    public void InitCardUI(HandPileMainUI handPileMainUI, CardInstance card,int index)
    {
        mainUI = handPileMainUI;
        counterpartCardInstance = card;
        _index = index;
        smoothSpeed = handPileMainUI.smoothSpeed;
        cardManager=FindAnyObjectByType<CardManager>();
        PrintCard();
    }
    // 鼠标悬停
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("鼠标悬停在 Image 上");
        mainUI.SetHoverInfo(true, _index);
        mainUI.CalculateCardsPosAndRot();
    }

    // 鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("鼠标离开 Image");
        mainUI.SetHoverInfo(false, _index);
        mainUI.CalculateCardsPosAndRot();
    }

    // 鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetDragging(true);

            if (Input.mousePosition.y > mainUI.playCardY)
            {
                TryPlayCard();
            }
            //Debug.Log("进入拖动模式：" + gameObject.name);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SetDragging(false);
            //Debug.Log("取消拖动：" + gameObject.name);

        }
    }

    // 开始拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        SetDragging(true);
    }

    // 拖动中
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        transform.position = Input.mousePosition; // 让卡牌跟随鼠标
    }

    // 结束拖动
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        SetDragging(false);
        //Debug.Log("拖动结束");

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
        isHovering = hovering;
        if (isHovering)
        {
            targetPosition = new Vector3(targetPosition.x, hoverY, 0);
            targetEuler = 0;
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
            transform.position = Input.mousePosition; // 让卡牌跟随鼠标
        }
    }
    private void TryPlayCard()
    {
        if(cardManager.TryPlayCard(this))
        {
            mainUI.hoverOnCard = false;
            mainUI.Discard(this);

            //打出特效（暂时直接销毁）
            Destroy(gameObject);
        }
    }

    private void SetDragging(bool dragging)
    {
        isDragging = dragging;
        if(dragging)
        {
            transform.SetSiblingIndex(mainUI.pileSize);
        }
        else
        {
            transform.SetSiblingIndex(_index);
        }
    }

    private void PrintCard()
    {
        cardArtwork.sprite= cardManager.GetCardByID(counterpartCardInstance.cardID).cardArtwork;
        cardName.text= cardManager.GetCardByID(counterpartCardInstance.cardID).cardName;
        cardDescription.text = cardManager.GetCardByID(counterpartCardInstance.cardID).description;
    }


}
