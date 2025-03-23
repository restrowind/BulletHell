using System.Collections;
using UnityEngine;

public class SumonCreature : MonoBehaviour
{
    private Vector3 _targetPos;
    private Quaternion _targetRotation;
    private float _targetAngle; // Ŀ��Ƕ�
    [SerializeField] private float _radius; // Ŀ��뾶
    [SerializeField] private float _moveSpeed = 5f; // �ƶ��ٶ�
    [SerializeField] private float _rotateSpeed = 5f; // ��ת�ٶ�
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
        // **���Ƕ�ת��Ϊ����**
        float radian = _targetAngle * Mathf.Deg2Rad;

        // **����Ŀ��λ��**
        _targetPos.x = _radius * Mathf.Cos(radian);
        _targetPos.y = _radius * Mathf.Sin(radian);
        _targetPos.z = 0;

        // **����Ŀ����ת�Ƕ�**
        float targetEuler = _targetAngle + 90f;
        _targetRotation = Quaternion.Euler(0, 0, targetEuler);
    }

    private void Update()
    {
        // **ƽ���ƶ���Ŀ��λ��**
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos+ centerOffset, Time.deltaTime * _moveSpeed);

        // **ƽ����ת��Ŀ��Ƕ�**
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
