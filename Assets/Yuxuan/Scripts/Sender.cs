using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour
{
    public BulletObject bullet;

    public float CurrentAngle = 0;
    private float currentAngularSpeedVelocity = 0;
    private float currenTime = 0;

    public bool IsAttack;
    public Transform bulletsParent; // 子弹父节点
    public Transform target;

    private void Awake()
    {
        bulletsParent = GameObject.Find("Bullet").transform;
        target = GameObject.FindGameObjectWithTag("Player").transform;
      
        currentAngularSpeedVelocity = bullet.SenderAngularVelocity;

        // 删除对象池初始化代码
        if (bulletsParent == null)
        {
            bulletsParent = transform;
        }
    }

    private void FixedUpdate()
    {
        CurrentAngle = transform.eulerAngles.z;
        if (!IsAttack)
            return;

        currentAngularSpeedVelocity = Mathf.Clamp(
            currentAngularSpeedVelocity + bullet.SenderAccleration * Time.fixedDeltaTime,
            -bullet.SenderMaxAngularVelocity, bullet.SenderMaxAngularVelocity);
        CurrentAngle += currentAngularSpeedVelocity * Time.fixedDeltaTime;
        currenTime += Time.fixedDeltaTime;

        if (Mathf.Abs(CurrentAngle) > 720f)
        {
            CurrentAngle -= Mathf.Sign(CurrentAngle) * 360f;
        }

        if (currenTime > bullet.SendInterval)
        {
            currenTime -= bullet.SendInterval;
            SendByCount(bullet.Count, CurrentAngle);
        }
    }

    private void Send(float angle)
    {
        // 直接实例化新子弹
        GameObject go = Instantiate(bullet.Prefabs, bulletsParent);
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        var bh = go.GetComponent<BulletBehaviour>();
        bh.isTracker = bullet.isTrack;
        bh.trackTime = bullet.trackTime;
        bh.damage = bullet.Damage;
        bh.target = target;
        
        // 设置自动销毁
        Destroy(go, bullet.LiftCycle);
        
        InitBullet(bh);
    }

    private void SendByCount(int count, float angle)
    {
        float temp = count % 2 == 0 ? angle + bullet.LineAngle / 2 : angle;

        for (int i = 0; i < count; i++)
        {
            float modifiedAngle = temp + Mathf.Pow(-1, i) * i * bullet.LineAngle;
            Send(modifiedAngle);
        }
    }

    private void InitBullet(BulletBehaviour bh)
    {
        bh.LinearVelocity = bullet.LinearVelocity;
        bh.Acceleration = bullet.Acceleration;
        bh.AngularAcceleration = bullet.AngularAcceleration;
        bh.AngularVelocity = bullet.AngularVelocity;
        bh.LifeTime = bullet.LiftCycle;
        bh.MaxVelocity = bullet.MaxVelocity;
    }

    // 删除对象池相关方法
}