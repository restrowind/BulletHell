using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

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
    public BattleState _currentState;

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

    [SerializeField] private GameController gameController;
    [SerializeField] private PlayerCharacter player;

    private int roundCount = 0;

    public static BattleManager Instance { get; private set; }

    public event Action PlayCardOver;
    public event Action BulletOverResolution;
    [SerializeField] private Boss _boss;


    [SerializeField] private Vector3 hiddenPos;
    [SerializeField] private Vector3 showPos;
    [SerializeField] private bool hidden = false;
    [SerializeField] private float smoothSpeed = 10f;
    private Transform mainCamera;
    [SerializeField] private Vector3 playerShowPos;
    [SerializeField] private MapLoader mapLoader;

    void Start()
    {
        Invoke("StartToStart", 0.8f);
        mainCamera = Camera.main.transform;
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

                _cardManager._sender.ClearAllBullets();
                break;
            case BattleState.DiscardPhase:
                _cardManager._sender.ClearAllBullets();
                break;
            case BattleState.Conversation:

                break;
            case BattleState.BattleEnd:

                break;
        }
        MoveToTarget();
    }

    public void StartToStart()
    {
        ChangeState(BattleState.Start);
    }

    public void ChangeState(BattleState newState)
    {
        SetHidden(false);
        Debug.Log("移动到" + newState.ToString() + "阶段");
        _currentState = newState;
        DisplayState();
        switch (_currentState)
        {
            case BattleState.Start:
                BattleStartUIEff();
                _cardManager.DrawCards(fistDrawCardCount);
                player.SetPlayerPause(true);
                Invoke("MoveToNextState", startDuration);
                break;
            case BattleState.DrawPhase:
                _cardManager.SetCardState(CardState.Other);
                _cardManager.DrawCards(everyTimeDrawCardCount);
                Invoke("MoveToNextState", drawCardDuration);
                _cardManager.ClearUsedElement();
                break;
            case BattleState.BulletPhase:
                _cardManager.SetCardState(CardState.Bullet);
                //Invoke("MoveToNextState", testBulletDuration);
                player.SetPlayerPause(false);
                SetHidden(true);
                if (roundCount == 0)
                {
                    gameController.SetGameState(BattleState.BulletPhase);
                }
                else
                {
                    gameController.SetGameState(BattleState.BulletPhase);
                    gameController._bossCountDown.ReStart();
                }
                break;
            case BattleState.PlayCardPhase:
                BulletOverResolution?.Invoke();
                _cardManager.UpdateRoundBuffList();
                _cardManager.SetCardState(CardState.Play);
                player.SetPlayerPause(true);
                StopPlayButton.Play("StopPlayButtonAppear");
                SetHidden(false);
                break;
            case BattleState.DiscardPhase:
                _cardManager.ExecuteDiscardPhase();
                _cardManager.SetCardState(CardState.Discard);
                break;
            case BattleState.Conversation:
                if(true)
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
                stateText.text = "Battle Start";
                stateDisplayUI.Play("StateDisplay",0,0);
                break;
            case BattleState.DrawPhase:
                stateText.text = "Draw Your Cards";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.BulletPhase:
                stateText.text = "Bullet Stage";
                stateDisplayUI.Play("StateDisplay", 0, 0);

                break;
            case BattleState.PlayCardPhase:
                stateText.text = "Your Turn";
                stateDisplayUI.Play("StateDisplay", 0, 0);
                break;
            case BattleState.DiscardPhase:
                PlayCardOver?.Invoke();
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
        switch(_currentState)
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
                roundCount++;
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

    private void MoveToTarget()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main.transform;
        }
        if (hidden)
        {
            mainCamera.position = Vector3.Lerp(mainCamera.position, hiddenPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            mainCamera.position = Vector3.Lerp(mainCamera.position, showPos, Time.deltaTime * smoothSpeed);
        }
    }
    public void SetHidden(bool isHidden)
    {
        hidden = isHidden;
        if (player)
        {
            if (hidden)
            {
                player.transform.position = mapLoader.bornPos;
            }
            else
            {
                //player.transform.position = playerShowPos;
            }
        }
    }

}