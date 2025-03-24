using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;

[CreateAssetMenu(menuName = "BulletAsset")]
public class BulletObject : ScriptableObject
{
   [Header("子弹初始配置")]
   public float LiftCycle = 5;
   public float LinearVelocity = 0;
   public float Acceleration = 0;
   public float AngularVelocity = 0;
   public float AngularAcceleration = 0;
   public float MaxVelocity = int.MaxValue;

    [Header("子弹朝向配置")]
    public bool FacePlayerOnFire = false; // 发射时朝向玩家

    [Header("发射器初始配置")] 
   //public float InitRotation = 0;

   public float SenderAngularVelocity = 0;
   public float SenderMaxAngularVelocity = int.MaxValue;
   public float SenderAccleration = 0;
   public int Count = 0;
   public float LineAngle = 30;
   public float SendInterval = 0.1f;
   public float Damage;
   public bool isTrack;
   public float trackTime;
   [Header("预制体")] public GameObject Prefabs;

    [Header("追踪设置")]
    public float trackRotationSpeed = 360f;


    [Header("发射器平移配置")]
    public Vector2 SenderMoveDirection = Vector2.zero;   // 发射器移动方向
    public float SenderMoveSpeed = 0f;                   // 初始速度
    public float SenderMoveAcceleration = 0f;            // 线性加速度
    public float SenderMaxMoveSpeed = Mathf.Infinity;    // 最大速度
}
