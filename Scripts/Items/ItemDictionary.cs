using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public static ItemDictionary instance;
    public List<ItemValues> items = new List<ItemValues>();

    private void Awake ()
    {
        instance = this;
    }

    public Item GetItemByName (string name)
    {
        return items.Find(x => x.name == name).item;
    }
}

[System.Serializable]
public class ItemValues
{
    public string name;
    public Item item;
}
