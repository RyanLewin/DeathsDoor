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

    public Vector3 offset = new Vector3(0, 0, -0.06f);

    public bool toBeLooted = true;
    public bool SetToBeLooted
    {
        get => toBeLooted;
        set
        {
            toBeLooted = value;
            if (toBeLooted)
                WorldController.GetWorldController.itemsInScene.Add(this);
            else
            {
                if (WorldController.GetWorldController.itemsInScene.Contains(this))
                    WorldController.GetWorldController.itemsInScene.Remove(this);
            }
        }
    }

    public Item ()
    {
    }

    public Item (int _id, int _size, int _count, bool _toBeLooted)
    {
        id = _id;
        size = _size;
        count = _count;
        SetToBeLooted = _toBeLooted;
    }

    public Item (Item _copyFrom)
    {
        id = _copyFrom.GetID;
        size = _copyFrom.size;
        count = _copyFrom.count;
        SetToBeLooted = _copyFrom.toBeLooted;
    }

    private void Awake ()
    {
        SetToBeLooted = toBeLooted;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            offset.y = renderer.bounds.extents.y;
        }
        else
        {
            if (GetComponentInChildren<Renderer>())
                offset.y = GetComponentInChildren<Renderer>().bounds.extents.y;
        }
    }

    protected void RanOut ()
    {
        Destroy(gameObject);
    }
}
