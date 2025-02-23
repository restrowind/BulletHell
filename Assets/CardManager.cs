using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public enum CardPiles
{
    Hand,
    Draw,
    Discard,
    Exhaust
}

public static class ListExtensions
{
    private static readonly System.Random rng = new System.Random();

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1); // 生成 [0, i] 之间的随机索引
            (list[i], list[j]) = (list[j], list[i]); // 交换两张牌
        }
    }
}

public class CardManager : MonoBehaviour
{

    public static CardManager Instance { get; private set; }

    private Dictionary<int, CardDataSO> cardDictionary = new Dictionary<int, CardDataSO>();

    [SerializeField] private string cardFolderPath = "CardDatabase"; // SO 文件存放的 Resources 文件夹路径


    [SerializeField] private List<CardInstance> initCards = new List<CardInstance>();
    [SerializeField] private List<CardInstance> handPile = new List<CardInstance>();
    public ReadOnlyCollection<CardInstance> HandPile => handPile.AsReadOnly();
    [SerializeField] private List<CardInstance> drawPile = new List<CardInstance>();
    [SerializeField] private List<CardInstance> discardPile = new List<CardInstance>();
    private List<CardInstance> exhaustPile = new List<CardInstance>();

    private int allCardsSize => initCards.Count;
    private int handPileSize => handPile.Count;
    private int drawPileSize => drawPile.Count;
    private int discardPileSize => discardPile.Count;
    private int exhaustPileSize => exhaustPile.Count;

    [SerializeField] private HandPileMainUI HandPileMainUI;

    public int testDrawCard = 4;

    private void Awake()
    {
        // 确保单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCards();
    }

    /// <summary>
    /// 初始化CardManager
    /// </summary>
    private void initCardsManagerForCombat()
    {
        //根据玩家拥有的卡牌装填牌堆
        LoadInitCards();

        //洗出抽牌堆
        drawPile = initCards.ToList();
        drawPile.Shuffle();

        //清空弃牌堆、耗牌堆、手牌堆
        handPile.Clear();
        discardPile.Clear();
        exhaustPile.Clear();
    }

    private void LoadInitCards()
    {
        //initCards =;
    }

    private void ShuffleCardsToDrawPile()
    {
        //可能的洗牌特效？
        drawPile.AddRange(discardPile);
        drawPile.Shuffle();
        discardPile.Clear();
    }

    private void DrawCards(int n)
    {
        for (int i = 0; i < n; i++)
        {
           
            if (drawPile.Count > 0)
            {
                handPile.Add(drawPile[0]);
                drawPile.RemoveAt(0);
            }
            else
            {
                ShuffleCardsToDrawPile();
                handPile.Add(drawPile[0]);
                drawPile.RemoveAt(0);
            }
        }
        HandPileMainUI.UpdateHandPile();
    }

    private void Start()
    {
        //测试
        CardInstance card1 = new CardInstance();
        CardInstance card2 = new CardInstance();
        CardInstance card3 = new CardInstance();
        CardInstance card4 = new CardInstance();
        CardInstance card5 = new CardInstance();
        CardInstance card6 = new CardInstance();
        CardInstance card7 = new CardInstance();
        CardInstance card8 = new CardInstance();
        CardInstance card9 = new CardInstance();
        CardInstance card10 = new CardInstance();
        card1.InitCard(1);
        card2.InitCard(1);
        card3.InitCard(1);
        card4.InitCard(2);
        card5.InitCard(2);
        card6.InitCard(2);
        card7.InitCard(2);
        card8.InitCard(101);
        card9.InitCard(101);
        card10.InitCard(101);
        initCards.Add(card1);
        initCards.Add(card2);
        initCards.Add(card3);
        initCards.Add(card4);
        initCards.Add(card5);
        initCards.Add(card6); 
        initCards.Add(card7);
        initCards.Add(card8);
        initCards.Add(card9);
        initCards.Add(card10);

        //测试
        initCardsManagerForCombat();
    }

    private void Update()
    {
        //测试
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            DrawCards(testDrawCard);
        }
    }
    public bool TryPlayCard(HandCardUIInstance card)
    {
        //判定是否能够打出
        if (true)
        {
            handPile.Remove(card.counterpartCardInstance);
            discardPile.Add(card.counterpartCardInstance);

            //执行卡牌效果
            return true;
        }
        else
        {
            return false;
        }


    }
    private void LoadCards()
    {
        cardDictionary.Clear();

        CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(cardFolderPath);

        foreach (var card in allCards)
        {
            if (!cardDictionary.ContainsKey(card.cardID))
            {
                cardDictionary.Add(card.cardID, card);
                Debug.Log($"已加载卡牌: {card.cardID} - {card.cardName}");
            }
            else
            {
                Debug.LogWarning($"卡牌 ID {card.cardID} 重复，未添加: {card.cardName}");
            }
        }

        Debug.Log($" 总共加载了 {cardDictionary.Count} 张卡牌");
    }
    public CardDataSO GetCardByID(int cardID)
    {
        if (cardDictionary.TryGetValue(cardID, out var cardData))
        {
            return cardData;
        }
        Debug.LogWarning($"未找到 ID 为 {cardID} 的卡牌！");
        return null;
    }
}
