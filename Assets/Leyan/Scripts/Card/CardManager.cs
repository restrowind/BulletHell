using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;

public enum CardPiles
{
    Hand,
    Draw,
    Discard,
    Exhaust
}

public enum CardState
{
    Play,
    Discard,
    Bullet,
    Other
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
    [SerializeField] private BattleManager battleManager;
    public static CardManager Instance { get; private set; }

    private Dictionary<int, CardDataSO> cardDictionary = new Dictionary<int, CardDataSO>();

    [SerializeField] private string cardFolderPath = "CardDatabase"; // SO 文件存放的 Resources 文件夹路径


    [SerializeField] private List<CardInstance> initCards = new List<CardInstance>();
    [SerializeField] private List<CardInstance> handPile = new List<CardInstance>();
    public ReadOnlyCollection<CardInstance> HandPile => handPile.AsReadOnly();
    [SerializeField] private List<CardInstance> drawPile = new List<CardInstance>();
    [SerializeField] private List<CardInstance> discardPile = new List<CardInstance>();
    private List<CardInstance> exhaustPile = new List<CardInstance>();

    [SerializeField] private int maxKeepCount = 8;
    [SerializeField] private int maxHandCount = 12;

    private int allCardsSize => initCards.Count;
    private int handPileSize => handPile.Count;
    private int drawPileSize => drawPile.Count;
    private int discardPileSize => discardPile.Count;
    private int exhaustPileSize => exhaustPile.Count;

    [SerializeField] private HandPileMainUI HandPileMainUI;

    public int testDrawCard = 4;

    public CardState currentCardState=CardState.Other;

    [SerializeField] private Transform handPileArea;
    private Vector3 initHandPilePos;
    [SerializeField] private float hiddenHandPileY;
    private Vector3 hiddenHandPilePos;

    [SerializeField] private float handPileSmoothSpeed = 3f;

    public bool forceDisplayHandPile = false;

    private bool canPlayCard = false;

    [SerializeField] private GameObject tipBoardPfb;
    [SerializeField] private Transform cardSystemCanvas;
    [SerializeField] private Animator discardPage;

    public Boss _boss;
    public PlayerCharacter _player;

    [SerializeField] private ResourceCollection _collection;

    private int usedAqua, usedLumen, usedVitality;

    public List<BuffIDRoundsPair> roundBuffList= new List<BuffIDRoundsPair>();

    [SerializeField] private Sender _sender;

    [SerializeField] private bool nextCardDouble = false;
    public ActiveSkill _activeSkill;

    public struct BuffIDRoundsPair
    {
        public BuffIDRoundsPair(int id,int rounds,Action stop)
        {
            buffID = id;
            remainRounds= rounds;
            stopAction = stop;
        }
        public int buffID;
        public int remainRounds;
        public Action stopAction;
    }

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

    public void DrawCards(int n)
    {
        int drawNum = 0;
        if (handPileSize + n <= maxHandCount)
        {
            drawNum = n;
        }
        else
        {
            drawNum = maxHandCount - handPileSize;
            //手牌数量超过上限
            SpawnATipBoard("Hand is full, cannot draw more cards");

        }

        for (int i = 0; i < drawNum; i++)
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
        CardInstance card11 = new CardInstance();
        CardInstance card12 = new CardInstance();
        CardInstance card13 = new CardInstance();
        CardInstance card14 = new CardInstance();
        CardInstance card15 = new CardInstance();
        CardInstance card16 = new CardInstance();
        CardInstance card17 = new CardInstance();
        CardInstance card18 = new CardInstance();
        CardInstance card19 = new CardInstance();
        CardInstance card20 = new CardInstance();
        card1.InitCard(12);
        card2.InitCard(12);
        card3.InitCard(12);
        card4.InitCard(12);
        card5.InitCard(12);
        card6.InitCard(12);
        card7.InitCard(12);
        card8.InitCard(14);
        card9.InitCard(14);
        card10.InitCard(14);
        card11.InitCard(14);
        card12.InitCard(14);
        card13.InitCard(14);
        card14.InitCard(14);
        card15.InitCard(14);
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
        initCards.Add(card11);
        initCards.Add(card12);
        initCards.Add(card13);
        initCards.Add(card14);
        initCards.Add(card15);

        //测试
        initCardsManagerForCombat();
        initHandPilePos = handPileArea.position;
        hiddenHandPilePos = handPileArea.position + new Vector3(0, hiddenHandPileY - initHandPilePos.y, 0);

        handPileArea.position = hiddenHandPilePos;
        battleManager.PlayCardOver += OverNextDouble;
    }

    private void Update()
    {
        HandleHandPilePosition();
        /*if(Input.GetKeyDown(KeyCode.Backspace))
        {
            _player.AddSumon(1);
        }*/
    }
    public bool TryPlayCard(HandCardUIInstance card)
    {
        //判定是否能够打出
        if (TryChargeCollection(card.counterpartCardInstance))
        {
            handPile.Remove(card.counterpartCardInstance);
            discardPile.Add(card.counterpartCardInstance);

            //执行卡牌效果
            CardInstance executedCard = card.counterpartCardInstance;
            if (nextCardDouble)
            {
                StartCoroutine(ExecuteCardEffect(executedCard));
                StartCoroutine(ExecuteCardEffect(executedCard));
                nextCardDouble = false;
            }
            else
            {
                StartCoroutine(ExecuteCardEffect(executedCard));
            }
            return true;
        }
        else
        {
            SpawnATipBoard("Insufficient resources");
            return false;
        }


    }

    private bool TryChargeCollection(CardInstance card)
    {
        CardDataSO cardData = GetCardByID(card.cardID);
        if (ResourceCollection.aqua>= cardData.cost.aqua
            && ResourceCollection.lumen >= cardData.cost.lumen
            && ResourceCollection.vitality >= cardData.cost.vitality)
        {
            ResourceCollection.aqua -= cardData.cost.aqua;
            ResourceCollection.lumen -= cardData.cost.lumen;
            ResourceCollection.vitality -= cardData.cost.vitality;

            usedAqua += cardData.cost.aqua;
            usedLumen += cardData.cost.lumen;
            usedVitality += cardData.cost.vitality;
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
                //Debug.Log($"已加载卡牌: {card.cardID} - {card.cardName}");
            }
            else
            {
                //Debug.LogWarning($"卡牌 ID {card.cardID} 重复，未添加: {card.cardName}");
            }
        }

        //Debug.Log($" 总共加载了 {cardDictionary.Count} 张卡牌");
    }
    public CardDataSO GetCardByID(int cardID)
    {
        if (cardDictionary.TryGetValue(cardID, out var cardData))
        {
            return cardData;
        }
        //Debug.LogWarning($"未找到 ID 为 {cardID} 的卡牌！");
        return null;
    }

    public void ExecuteDiscardPhase()
    {
        if(handPileSize>maxKeepCount)
        {
            Discard(handPileSize - maxKeepCount);
        }
        else
        {
            battleManager.MoveToNextState();
            SpawnATipBoard("No need to discard");
        }
    }
    public void SetCardState(CardState state)
    {
        currentCardState = state;
        HandPileMainUI.SetCanPlayCards(state);
        
    }
    private void Discard(int count)
    {
        SetCardState(CardState.Discard);
        discardPage.Play("DiscardPageFadeIn");
        HandPileMainUI.StartDiscard(count);
    }
    public void DiscardOver()
    {
        discardPage.Play("DiscardPageFadeOut");
    }

    public void ConfirmDiscard()
    {
        HandPileMainUI.ConfirmDiscard();

    }
    public void DiscardToDiscardPile(CardInstance card)
    {
        handPile.Remove(card);
        discardPile.Add(card);
    }

    private void HandleHandPilePosition()
    {
        if(currentCardState== CardState.Bullet)
        {
            if (forceDisplayHandPile)
            {
                handPileArea.position = Vector3.Lerp(handPileArea.position, initHandPilePos, Time.deltaTime * handPileSmoothSpeed);
            }
            else
            {
                handPileArea.position = Vector3.Lerp(handPileArea.position, hiddenHandPilePos, Time.deltaTime * handPileSmoothSpeed);
            }
            canPlayCard = false;
        }
        else
        {
            handPileArea.position = Vector3.Lerp(handPileArea.position, initHandPilePos, Time.deltaTime * handPileSmoothSpeed);
            canPlayCard = true;
        }
    }

    public void SpawnATipBoard(string text)
    {
        Transform tipBoard= Instantiate(tipBoardPfb, cardSystemCanvas).transform;
        tipBoard.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        Destroy(tipBoard.gameObject,1.5f);
    }

    private void Buff6Action()
    {
        if(_player.damageTakeThisTurn<=0)
        {
            _boss.DealDamage(50);
        }
    }

    IEnumerator ExecuteCardEffect(CardInstance executedCard)
    {
        //GetCardByID(executedCard.cardID).ExecuteEffects(battleManager.gameObject);
        yield return null;
        switch (executedCard.cardID)
        {
            case 1:
                _boss.DealDamage(30);
                break;
            case 2:
                DrawCards(2);
                break;
            case 3:
                LoadARoundsBuff(() =>
                {
                    _boss.bossCountDown.MultipleLengthReduceRate(0.8f);
                    _player.MultipleCollectEnhance(1.25f);
                }, () =>
                {
                    _boss.bossCountDown.MultipleLengthReduceRate(1.25f);
                    _player.MultipleCollectEnhance(0.8f);
                }, 1, 1);
                break;
            case 4:
                nextCardDouble = true;
                break;
            case 5:
                LoadARoundsBuff(() =>
                {
                    _sender.MultipleIntervalRate(1.25f);
                }, () =>
                {
                    _sender.MultipleIntervalRate(0.8f);
                }, 5, 1);
                break;

            case 6:
                LoadARoundsBuff(() =>
                {
                    _player.MultiplaeInvincibleTime(2f);
                }, () =>
                {
                    _player.MultiplaeInvincibleTime(0.5f);
                }, 4, 2);
                break;
            case 7:
                _boss.DealDamage(10f * usedAqua);
                break;
            case 8:
                LoadARoundsBuff(() =>
                {
                    _boss.MultiplaDamageRate(2f);
                }, () =>
                {
                    _boss.MultiplaDamageRate(0.5f);
                }, 2, 1);
                break;
            case 9:
                LoadARoundsBuff(() =>
                {
                    battleManager.BulletOverResolution += Buff6Action;
                }, () =>
                {
                    battleManager.BulletOverResolution -= Buff6Action;
                }, 6, 1);
                break;
            case 10:
                _boss.DealDamage(10f * ResourceCollection.aqua);
                usedAqua += ResourceCollection.aqua;
                ResourceCollection.aqua = 0;
                break;
            case 11:
                _player.HealByAbs(20);
                break;
            case 12:
                _activeSkill.LoadActiveSkill(1, 1, () =>
                {
                    _sender.ClearAllBullets();
                    _player.HealByAbs(20);
                });
                break;
            case 13:
                _activeSkill.LoadActiveSkill(2, 1, () =>
                {
                    _player.InvincibleForAWhile(10f);
                });
                break;
            case 14:
                _player.AddSumon(ResourceCollection.vitality);
                usedLumen += ResourceCollection.vitality;
                ResourceCollection.vitality = 0;
                break;
            case 15:

                LoadARoundsBuff(() =>
                {
                    sumonGiveBackDamageRate = 1f;
                }, () =>
                {
                    sumonGiveBackDamageRate=0f;
                }, 8, 1);
                break;
            case 16:
                _boss.DealDamage(10f * _player.sumonCreatureCount);
                break;
            case 17:

                LoadARoundsBuff(() =>
                {
                    _player.SetgiveBackDamageRate(2);
                }, () =>
                {
                    _player.SetgiveBackDamageRate(0);
                }, 7, 1);
                break;
            case 18:
                _player.HealByWoundPercentage(0.5f);
                break;
            case 19:
                LoadARoundsBuff(() =>
                {
                    _player.MultipleTakeDamage(2f);
                    _player.MultipleCollectEnhance(2f);
                }, () =>
                {
                    _player.MultipleTakeDamage(0.5f);
                    _player.MultipleCollectEnhance(0.5f);
                }, 3, 1);
                break;
            case 20:
                _player.HealByAbs(10f * ResourceCollection.lumen);
                usedLumen += ResourceCollection.lumen;
                ResourceCollection.lumen = 0;
                break;
        }
        yield break;
    }

    public float sumonGiveBackDamageRate = 0;

    public void ClearUsedElement()
    {
        usedAqua = 0;
        usedLumen = 0;
        usedVitality = 0;
    }

    private void LoadARoundsBuff(Action start,Action stop,int rounds,int buffID)
    {
        roundBuffList.Add(new BuffIDRoundsPair(rounds,buffID, stop));
        start?.Invoke();
    }

    public void UpdateRoundBuffList()
    {
        for(int i=0;i< roundBuffList.Count();i++)
        {
            BuffIDRoundsPair tmp = roundBuffList[i];
            tmp.remainRounds--;
            roundBuffList[i]=tmp;
            if (tmp.remainRounds <= 0)
            {
                tmp.stopAction?.Invoke();
                roundBuffList.RemoveAt(i);
            }
        }
        
    }

    public void OverNextDouble()
    {
        nextCardDouble = false;
    }

}
