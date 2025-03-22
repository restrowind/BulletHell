using System.Collections;
using UnityEngine;

public class SumonCreature : MonoBehaviour
{
    private Vector3 _targetPos;
    private Quaternion _targetRotation;
    private float _targetAngle; // 目标角度
    [SerializeField] private float _radius; // 目标半径
    [SerializeField] private float _moveSpeed = 5f; // 移动速度
    [SerializeField] private float _rotateSpeed = 5f; // 旋转速度
    private int remainRound;
    [SerializeField] private Vector3 centerOffset;

    public void SetTargetRotation(float rotation)
    {
        _targetAngle = rotation;
        CalculateTargetPosRot();
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
        CalculateTargetPosRot();
    }

    private void CalculateTargetPosRot()
    {
        // **将角度转换为弧度**
        float radian = _targetAngle * Mathf.Deg2Rad;

        // **计算目标位置**
        _targetPos.x = _radius * Mathf.Cos(radian);
        _targetPos.y = _radius * Mathf.Sin(radian);
        _targetPos.z = 0;

        // **计算目标旋转角度**
        float targetEuler = _targetAngle + 90f;
        _targetRotation = Quaternion.Euler(0, 0, targetEuler);
    }

    private void Update()
    {
        // **平滑移动到目标位置**
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos+ centerOffset, Time.deltaTime * _moveSpeed);

        // **平滑旋转到目标角度**
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotateSpeed);
    }
    public void RoundOver()
    {
        remainRound--;
        if(remainRound<=0)
        {
            Destroy(gameObject);
        }
    }
    public void Init(int round)
    {
        remainRound=round;
        BattleManager.Instance.BulletOverResolution += RoundOver;
    }
    public void GetDamage(float damage)
    {
        CardManager.Instance._boss.DealDamage(CardManager.Instance.sumonGiveBackDamageRate * damage);
    }
}
