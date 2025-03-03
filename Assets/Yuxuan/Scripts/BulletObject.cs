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

}
