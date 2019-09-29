using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    public TaskItems task;
    //public bool possible;
    public Vector3 location;
    public GameObject obj;
    public bool personal;

    public Task (TaskItems _task = TaskItems.None, Vector3 _loc = default, GameObject _obj = default, bool _personal = false)
    {
        task = _task;
        //possible = _possible;
        location = _loc;
        obj = _obj;
        personal = _personal;
    }
}

public enum TaskItems
{
    Move,
    Vehicle,
    None,
    Build,
    Hunt,
    Cook,
    Defend,
    Loot,
    Mine,
    Harvest,
    Wander
}