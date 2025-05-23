﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour
{
    public BulletObject bullet;

    private float currentAngularSpeedVelocity = 0;
    private float currentSpeed = 0;
    private float currenTime = 0;

    public bool IsAttack;
    public Transform bulletsParent; // 子弹父节点
    public Transform target;

    [SerializeField] private float baseIntervalRate = 1f;  // 基础倍率（默认为1）
    private float currentIntervalRate = 1f;  // 当前发射倍率
    [SerializeField] private float extraIntervalRate = 1f; // 额外倍率，供外部调整

    private float currentMoveSpeed = 0f;

    private void Awake()
    {
        bulletsParent = GameObject.Find("BulletController")?.transform;
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (bullet == null)
        {
            Debug.LogError("❌ BulletObject 未赋值，Sender 无法正常工作！");
            return;
        }

        Debug.Log($"✅ Sender 读取 BulletObject 成功: {bullet.name}");

        currentAngularSpeedVelocity = bullet.SenderAngularVelocity;

        if (bulletsParent == null)
        {
            bulletsParent = transform;
        }
        UpdateIntervalRate();
        currentMoveSpeed = bullet.SenderMoveSpeed;
    }

    private void FixedUpdate()
    {
        if (bullet == null) return;

        if (bullet.SenderMoveDirection != Vector2.zero)
        {
            // 加速度影响
            currentMoveSpeed += bullet.SenderMoveAcceleration * Time.fixedDeltaTime;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, -bullet.SenderMaxMoveSpeed, bullet.SenderMaxMoveSpeed);

            // 移动
            Vector3 offset = (Vector3)(bullet.SenderMoveDirection.normalized * currentMoveSpeed * Time.fixedDeltaTime);
            transform.position += offset;
        }

        // --- 1️⃣ 处理自旋转（确保生效）---
        if (bullet.SenderAngularVelocity != 0 || bullet.SenderAccleration != 0)
        {
            currentAngularSpeedVelocity = Mathf.Clamp(
                currentAngularSpeedVelocity + bullet.SenderAccleration * Time.fixedDeltaTime,
                -bullet.SenderMaxAngularVelocity, bullet.SenderMaxAngularVelocity);

            transform.Rotate(0, 0, currentAngularSpeedVelocity * Time.fixedDeltaTime);
        }

        // --- 2️⃣ 处理发射间隔（修正 SendInterval 计算） ---
        if (!IsAttack) return;

        currenTime += Time.fixedDeltaTime;
        if (currenTime > bullet.SendInterval * currentIntervalRate)
        {
            currenTime -= bullet.SendInterval * currentIntervalRate;
            float currentAngle = transform.eulerAngles.z;
            SendByCount(bullet.Count, currentAngle);
        }
    }

    // --- 3️⃣ 发送弹幕（修正角度均匀问题）---
    private void SendByCount(int count, float angle)
    {
        float totalAngle = (count - 1) * bullet.LineAngle;
        float startAngle = angle - totalAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float modifiedAngle = startAngle + i * bullet.LineAngle;
            Send(modifiedAngle);
        }
    }

    private void Send(float angle)
    {
        // 实例化新子弹
        GameObject go = Instantiate(bullet.Prefabs, bulletsParent);
        go.transform.position = transform.position;

        // 设置初始朝向
        if (bullet.FacePlayerOnFire && target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            float playerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0, 0, playerAngle);
        }
        else
        {
            go.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        var bh = go.GetComponent<BulletBehaviour>();
        if (bh != null)
        {
            bh.isTracker = bullet.isTrack;
            bh.trackTime = bullet.trackTime;
            bh.trackRotationSpeed = bullet.trackRotationSpeed; // ✅ 新增同步拐弯速度
            bh.damage = bullet.Damage;
            bh.target = target;

            Destroy(go, bullet.LiftCycle);
            InitBullet(bh);
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

    // --- 4️⃣ 允许外部修改发射间隔倍率（可用于难度动态调整） ---
    public void MultipleIntervalRate(float rate)
    {
        extraIntervalRate *= rate;
        UpdateIntervalRate();
    }

    private void UpdateIntervalRate()
    {
        currentIntervalRate = extraIntervalRate * baseIntervalRate;
    }

    public void ClearAllBullets()
    {
        // 找到场景中所有带 `BulletBehaviour` 的子弹
        BulletBehaviour[] bullets = FindObjectsOfType<BulletBehaviour>();

        // 遍历所有子弹并销毁
        foreach (BulletBehaviour bullet in bullets)
        {
            Destroy(bullet.gameObject); // ⚠️ 如果有对象池，需要改成回收
        }

        Debug.Log($"✅ 清除 {bullets.Length} 颗子弹！");
    }
}
