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
        // ȷ�� Start() ����ʱ���ҵ����� IBattlePhaseDependent ���
        StartCoroutine(FindAllPhaseDependentObjects());
    }

    private IEnumerator FindAllPhaseDependentObjects()
    {
        yield return new WaitForSeconds(0.1f); // �ȴ�һ֡��ȷ�����ж����Ѿ���ʼ��
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