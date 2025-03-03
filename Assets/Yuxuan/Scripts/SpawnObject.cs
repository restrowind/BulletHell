using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object
{
    public GameObject prefab;      // 要生成的预制体
    public BulletObject bullet;    // 子弹属性（需要自定义BulletObject类）
    public float spawnTime;        // 生成间隔时间
    public float destroyTime;      // 自动销毁时间
    public Vector3 spawnPosition;
    public float rotation;
}

public class SpawnObject : MonoBehaviour
{
    public List<Object> objects;   // 对象配置列表

    private void Start()
    {
        // 为每个配置对象启动独立的生成协程
        foreach (Object obj in objects)
        {
            StartCoroutine(SpawnRoutine(obj));
        }
    }

    private IEnumerator SpawnRoutine(Object config)
    {
        yield return new WaitForSeconds(config.spawnTime);
        // 生成新对象
        GameObject newObj = Instantiate(
            config.prefab
        );
        newObj.GetComponent<Sender>().bullet = config.bullet;
        newObj.GetComponent<Sender>().IsAttack = true;
        newObj.transform.position = config.spawnPosition;
        newObj.transform.localEulerAngles = new Vector3(0,0,config.rotation);
       
        // 设置自动销毁
        Destroy(newObj, config.destroyTime);
        // 等待下次生成
    }
    // 可视化生成点（可选）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}