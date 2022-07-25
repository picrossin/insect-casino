using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask projDestroyers;
    [SerializeField] private int dmg = 1;
    [SerializeField] private bool isSlime;

    private Hand _goal;
    private Vector2 _dir;

    private void Start()
    {
        // Find nearest hand
        _goal = null;
        float minDist = Single.MaxValue;
        foreach (Hand hand in GameManager.Instance.HandsInPlay)
        {
            float dist = Vector2.Distance(hand.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                _goal = hand;
            }
        }

        if (_goal == null)
        {
            Destroy(gameObject);
        }
        else
        {
            _dir = (_goal.transform.position - transform.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(_dir.x, _dir.y, 0f) * speed;

        Collider2D coll = Physics2D.OverlapCircle(transform.position, 0.25f, projDestroyers);

        if (coll != null)
        {
            if (coll.CompareTag("Hand"))
            {
                if (coll.GetComponent<Hand>().State == Hand.HandState.Retracting)
                {
                    return;
                }
                
                if (isSlime)
                {
                    coll.GetComponent<Hand>().QueueDamage = dmg;
                    coll.GetComponent<Hand>().QueueSlowdown = true;
                }
                else
                {
                    coll.GetComponent<Hand>().QueueDamage = dmg;
                }
            }
            
            Destroy(gameObject);
        }
    }
}
