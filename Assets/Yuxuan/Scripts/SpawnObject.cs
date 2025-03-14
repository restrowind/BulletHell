using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundConfig
{
    public List<ObjectConfig> objects; // 当前回合的对象列表
}

[System.Serializable]
public class ObjectConfig
{
    public GameObject prefab;      // 要生成的预制体
    public BulletObject bullet;    // 子弹属性（需要自定义BulletObject类）
    public float spawnTime;        // 生成间隔时间
    public float destroyTime;      // 自动销毁时间
    public Vector3 spawnPosition;
    public float rotation;
}

public class SpawnObject : MonoBehaviour, IBattlePhaseDependent
{
    public List<RoundConfig> roundObjects; // 每个回合独立的对象配置列表
    private int currentRound = 0;
    private BossCountDown bossCountDown;
    private BattleState _currentState;

    private void Start()
    {
        bossCountDown = FindObjectOfType<BossCountDown>();
        if (bossCountDown != null)
        {
            currentRound = bossCountDown.roundCount;
        }
        StartRoundSpawn();
    }

    private void Update()
    {
        if (_currentState != BattleState.BulletPhase) return;

        if (bossCountDown != null && bossCountDown.roundCount != currentRound)
        {
            currentRound = bossCountDown.roundCount;
            StartRoundSpawn();
        }
    }

    private void StartRoundSpawn()
    {
        if (currentRound < roundObjects.Count)
        {
            foreach (ObjectConfig obj in roundObjects[currentRound].objects)
            {
                StartCoroutine(SpawnRoutine(obj));
            }
        }
    }

    private IEnumerator SpawnRoutine(ObjectConfig config)
    {
        yield return new WaitForSeconds(config.spawnTime);
        // 生成新对象
        GameObject newObj = Instantiate(config.prefab);
        newObj.GetComponent<Sender>().bullet = config.bullet;
        newObj.GetComponent<Sender>().IsAttack = true;
        newObj.transform.position = config.spawnPosition;
        newObj.transform.localEulerAngles = new Vector3(0, 0, config.rotation);

        // 设置自动销毁
        Destroy(newObj, config.destroyTime);
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;
    }

    // 可视化生成点（可选）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}