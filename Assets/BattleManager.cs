using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    DrawPhase,
    BulletPhase,
    CardPhase,
    Conversation,
    BattleEnd
}

public class BattleManager : MonoBehaviour
{
    private BattleState _currentState;

    void Start()
    {
        ChangeState(BattleState.Start);
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
            case BattleState.CardPhase:

                break;
            case BattleState.Conversation:

                break;
            case BattleState.BattleEnd:

                break;
        }
    }

    public void ChangeState(BattleState newState)
    {
        _currentState = newState;
        switch (_currentState)
        {
            case BattleState.Start:

                break;
            case BattleState.DrawPhase:

                break;
            case BattleState.BulletPhase:

                break;
            case BattleState.CardPhase:

                break;
            case BattleState.Conversation:

                break;
            case BattleState.BattleEnd:

                break;
        }
    }

}