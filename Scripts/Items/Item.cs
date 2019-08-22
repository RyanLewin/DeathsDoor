using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    int id = 0;
    public int GetID { get => id; }
    public int size = 1;
    public int count = 1;
    public bool toBeLooted = true;

    private void Awake ()
    {
        if (toBeLooted)
        {
            WorldController.GetWorldController.itemsInScene.Add(this);
        }
    }

    protected void RanOut ()
    {
        Destroy(gameObject);
    }
}
