using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    /// <summary> Amount of damage it will deal </summary>
    public int damage = 10;
    /// <summary> The maximum distance that this tool can reach; </summary>
    public float range = 1f;
    public ToolTypes toolType = ToolTypes.Axe;

}

public enum ToolTypes { Axe, Dagger, Pick, RangedWeapon }