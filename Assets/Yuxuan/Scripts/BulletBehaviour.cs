using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float LinearVelocity = 0;
    public float Acceleration = 0;
    public float AngularVelocity = 0;
    public float AngularAcceleration = 0;
    public float MaxVelocity = int.MaxValue;
    public float LifeTime = 5f;

    private Sender sender; // Reference to Sender to return bullets to the pool

    public bool isTracker;
    public float damage;
    public float trackTime;
    public Transform target;
    private bool isMove;
    private void Awake()
    {
        sender = FindObjectOfType<Sender>(); // You could also set this manually in the Inspector
        
    }

    private void FixedUpdate()
    {
        if (gameObject == null) return; // Check to avoid errors if the object was destroyed
        if (LifeTime <= 0)
        {
            // Return the bullet to the pool instead of destroying it
           Destroy(gameObject);
        }
        if (isTracker)
        {
            trackTime -= Time.fixedDeltaTime;
            if (trackTime <= 0)
            {
               transform .position = Vector2.MoveTowards(transform.position, target.position, 0.1f);
                //Debug.Log("moveToTarget");
                isMove = true;
            }
            
        }

        if (!isMove)
        {
            LinearVelocity = Mathf.Clamp(LinearVelocity + Acceleration * Time.fixedDeltaTime,
                -MaxVelocity, MaxVelocity);
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
            if (!collision.GetComponent<PlayerCharacter>().playerCharacterPause)
            {
                Destroy(gameObject);
                collision.GetComponent<PlayerCharacter>().TakeDamage(damage);
                //Debug.Log("Take Damage");
            }
        }
    }
}