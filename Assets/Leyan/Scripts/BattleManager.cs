using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum BattleState
{
    Start,
    DrawPhase,
    BulletPhase,
    PlayCardPhase,
    DiscardPhase,
    Conversation,
    BattleEnd
}

public class BattleManager : MonoBehaviour
{
    private BattleState _currentState;
    [SerializeField] private CardManager _cardManager;

    public int fistDrawCardCount = 2;
    public int everyTimeDrawCardCount = 3;

    [SerializeField] private float startDuration = 0.5f;
    [SerializeField] private float drawCardDuration = 0.5f;
    [SerializeField] private float conversationDuration = 0.5f;

    [SerializeField] private float testBulletDuration = 5f;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Animator stateDisplayUI;

    [SerializeField] private Animator StopPlayButton;

    void Start()
    {
        Invoke("StartToStart", 0.8f);
    }

    void Update()
    {
        switch (_currentState)
        {
            case BattleState.Start:

                break;
            case BattleState.DrawPhase:

                break;
            case BattleState.BulletPhase:

                break;
            case BattleState.PlayCardPhase:

                break;
            case BattleState.DiscardPhase:

                break;
            case BattleState.Conversation:

                break;
            case BattleState.BattleEnd:

                break;
        }
    }

    public void StartToStart()
    {
        ChangeState(BattleState.Start);
    }

    public void ChangeState(BattleState newState)
    {
        Debug.Log("移动到" + newState.ToString() + "阶段");
        _currentState = newState;
        DisplayState();
        switch (_currentState)
        {
            case BattleState.Start:
                BattleStartUIEff();
                _cardManager.DrawCards(fistDrawCardCount);
                Invoke("MoveToNextState", startDuration);
                break;
            case BattleState.DrawPhase:
                _cardManager.SetCardState(CardState.Other);
                _cardManager.DrawCards(everyTimeDrawCardCount);
                Invoke("MoveToNextState", drawCardDuration);
                break;
            case BattleState.BulletPhase:
                _cardManager.SetCardState(CardState.Bullet);
                Invoke("MoveToNextState", testBulletDuration);
                break;
            case BattleState.PlayCardPhase:
                _cardManager.SetCardState(CardState.Play);
                StopPlayButton.Play("StopPlayButtonAppear");
                break;
            case BattleState.DiscardPhase:
                _cardManager.ExecuteDiscardPhase();
                _cardManager.SetCardState(CardState.Discard);
                break;
            case BattleState.Conversation:
                if (true)
                {
                    Invoke("MoveToNextState", conversationDuration);
                }
                break;
            case BattleState.BattleEnd:

                break;
        }
    }

    private void DisplayState()
    {
        switch (_currentState)
        {
            case BattleState.Start:
                stateText.text = "战斗开始";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.DrawPhase:
                stateText.text = "抽取卡牌";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.BulletPhase:
                stateText.text = "敌人回合";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.PlayCardPhase:
                stateText.text = "你的回合";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.DiscardPhase:
                //stateText.text = "弃牌";
                //stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.Conversation:

                break;


        }
    }

    private void BattleStartUIEff()
    {

    }


    public void MoveToNextState()
    {
        switch (_currentState)
        {
            case BattleState.Start:
                ChangeState(++_currentState);
                break;
            case BattleState.DrawPhase:
                ChangeState(++_currentState);
                break;
            case BattleState.BulletPhase:
                ChangeState(++_currentState);
                break;
            case BattleState.PlayCardPhase:
                ChangeState(++_currentState);
                break;
            case BattleState.DiscardPhase:
                ChangeState(++_currentState);
                break;
            case BattleState.Conversation:
                ChangeState((BattleState)1);
                break;
            case BattleState.BattleEnd:

                break;
        }

    }

    public void EndPlayCard()
    {
        ChangeState(BattleState.DiscardPhase);
        StopPlayButton.Play("StopPlayButtonFade");
    }

}