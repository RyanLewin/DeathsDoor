using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTransparancy : MonoBehaviour
{
    Renderer rend;
    List<Collider2D> collisionsList = new List<Collider2D>();

    private void Awake ()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        collisionsList.Add(collision);
        rend.material.color = new Color(1,1,1, .2f);
    }

    private void OnTriggerExit2D (Collider2D collision)
    {
        if (collisionsList.Contains(collision))
        {
            collisionsList.Remove(collision);
            if (collisionsList.Count <= 0)
                rend.material.color = new Color(1, 1, 1, 1);
        }
    }
}
