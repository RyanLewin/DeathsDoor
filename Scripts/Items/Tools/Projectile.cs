using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 0;
    public float range;
    public int force = 10;
    Vector2 startPoint;

    private void Awake ()
    {
        startPoint = transform.position;
        Destroy(gameObject, 5);
    }

    private void FixedUpdate ()
    {
        if (Vector2.Distance(startPoint, transform.position) > range)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.GetComponent<AI>())
        {
            AI ai = collision.GetComponent<AI>();
            ai.SetHealth -= Mathf.RoundToInt(damage);
            Destroy(gameObject);
        }
    }
}
