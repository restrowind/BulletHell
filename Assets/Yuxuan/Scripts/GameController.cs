using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBattlePhaseDependent
{
    void SetState(BattleState newState);
}

public class GameController : MonoBehaviour
{
    private List<IBattlePhaseDependent> phaseDependentObjects = new List<IBattlePhaseDependent>();

    void Awake()
    {
        phaseDependentObjects = FindObjectsOfType<MonoBehaviour>().OfType<IBattlePhaseDependent>().ToList();

        //≤‚ ‘”√
        //SetGameState(BattleState.BulletPhase);
    }

    public void SetGameState(BattleState newState)
    {
        foreach (var obj in phaseDependentObjects)
        {
            obj.SetState(newState);
        }
    }
}