using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBattlePhaseDependent
{
    void SetState(BattleState newState);
}

public class GameStateManager : MonoBehaviour
{
    private List<IBattlePhaseDependent> phaseDependentObjects = new List<IBattlePhaseDependent>();

    void Awake()
    {
        // 确保 Start() 运行时能找到所有 IBattlePhaseDependent 组件
        StartCoroutine(FindAllPhaseDependentObjects());
    }

    private IEnumerator FindAllPhaseDependentObjects()
    {
        yield return new WaitForSeconds(0.1f); // 等待一帧，确保所有对象已经初始化
        phaseDependentObjects = FindObjectsOfType<MonoBehaviour>().OfType<IBattlePhaseDependent>().ToList();
    }

    public void SetGameState(BattleState newState)
    {
        foreach (var obj in phaseDependentObjects)
        {
            obj.SetState(newState);
        }
    }
}