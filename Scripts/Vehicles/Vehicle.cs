using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField]
    protected int carryAmount = default;
    int currentCarryAmount { get; set; }
    List<Item> itemsInInventory = new List<Item>();
    public bool inUse = false;

    public bool CanTakeControl { get => !inUse; }

    public bool Inventory (Item itemToAdd)
    {
        if (currentCarryAmount + itemToAdd.size >= carryAmount)
        {
            currentCarryAmount += itemToAdd.size;
            itemsInInventory.Add(itemToAdd);
            return true;
        }
        return false;
    }
}
