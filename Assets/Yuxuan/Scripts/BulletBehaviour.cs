using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletBehaviour : MonoBehaviour
{
    public float LinearVelocity = 0;
    public float Acceleration = 0;
    public float AngularVelocity = 0;
    public float AngularAcceleration = 0;
    public float MaxVelocity = int.MaxValue;
    public float LifeTime = 5f;

    private Sender sender;

    public bool isTracker;
    public float damage;
    public float trackTime;
    public float trackRotationSpeed = 360f; // 每秒最大转向角度
    public Transform target;
    private bool isMove;
    [SerializeField] private Animator animator;
    [SerializeField] private string animName;

    private void Awake()
    {
        sender = FindObjectOfType<Sender>();
    }

    private void FixedUpdate()
    {
        if (gameObject == null) return;
        if (LifeTime <= 0)
        {
            Destroy(gameObject);
        }

        if (isTracker && target != null)
        {
            trackTime -= Time.fixedDeltaTime;
            if (trackTime <= 0)
            {
                Vector2 directionToTarget = (target.position - transform.position).normalized;
                float angleToTarget = Vector2.SignedAngle(transform.right, directionToTarget);

                float maxTurn = trackRotationSpeed * Time.fixedDeltaTime;
                float clampedAngle = Mathf.Clamp(angleToTarget, -maxTurn, maxTurn);

                transform.Rotate(Vector3.forward, clampedAngle);
            }
        }

        if (!isMove)
        {
            LinearVelocity = Mathf.Clamp(LinearVelocity + Acceleration * Time.fixedDeltaTime, -MaxVelocity, MaxVelocity);
            AngularVelocity += AngularAcceleration * Time.fixedDeltaTime;
            transform.Translate(LinearVelocity * Vector2.right * Time.fixedDeltaTime, Space.Self);
            transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 1) * AngularVelocity * Time.fixedDeltaTime);
            LifeTime -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            if (!collision.transform.parent.GetComponentInChildren<PlayerCharacter>().playerCharacterPause)
            {
                collision.transform.parent.GetComponentInChildren<PlayerCharacter>().TakeDamage(damage);
                Boom();
            }
        }
        else if (collision.gameObject.CompareTag("SumonCreature"))
        {
            collision.GetComponentInChildren<SumonCreature>().GetDamage(damage);
            Boom();
        }
    }

    private void Boom()
    {
        StopMovement();
        animator.Play(animName);
    }

    public void DestroyOneself()
    {
        Destroy(gameObject);
    }

    public void StopMovement()
    {
        LinearVelocity = 0;
        Acceleration = 0;
        AngularVelocity = 0;
        AngularAcceleration = 0;
        isMove = true;
    }
}
