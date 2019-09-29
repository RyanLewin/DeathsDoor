using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AI : MonoBehaviour
{
    protected WorldController worldController;
    protected Pathfinder pathfinder;
    protected PathRequestManager requestManager;

    [SerializeField]
    public Task currTask { get; protected set; }

    protected Vector3[] path;
    protected int targetIndex;

    public Tile taskTile;

    [SerializeField]
    public AIType aiType { get; protected set; }
    [Header("Housekeeping")]
    public List<Stat> stats = new List<Stat>();
    public string citizenName = "First Last";
    public bool gender = true;
    public int age = 22;
    public int birthMonth = 9;
    public string dob = "Sep '96";
    public int happiness = 60;
    protected int maxHappiness = 100;
    public int health = 100;
    protected int maxHealth = 100;
    public bool alive = true;

    protected bool move = false;
    protected Inventory inventory;

    public int SetHealth
    {
        get => health;
        set
        {
            health = value;
            if (health > maxHealth)
                health = maxHealth;
            else if (health <= 0)
                Dead();
        }
    }

    protected virtual void Dead ()
    {
        alive = false;
    }

    protected virtual void EndTask ()
    {
        StopCoroutine(FollowPath());
        currTask = new Task();
        targetIndex = 0;
        path = new Vector3[0];
        move = false;
        taskTile = null;
    }

    public void OnPathFound (Vector3[] _path, bool _success)
    {
        if (_success)
        {
            path = _path;
            targetIndex = 0;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
        else
        {
            worldController.AddTask(currTask);
            EndTask();
        }
    }

    protected IEnumerator FollowPath ()
    {
        if (path.Length <= 0)
        {
            worldController.AddTask(currTask);
            path = new Vector3[0];
            targetIndex = 0;
            currTask = new Task();
            move = false;
            yield break;
        }
        Vector3 currWaypoint = path[0];

        while (true)
        {
            if (path.Length <= 0)
                yield break;
            if (transform.position == currWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    path = new Vector3[0];
                    move = false;
                    yield break;
                }
                currWaypoint = path[targetIndex];
            }
            transform.up = new Vector3(currWaypoint.x, currWaypoint.y, transform.position.z) - transform.position;
            float speed = Time.deltaTime * ((GetStat("Speed").value + 1) / 2);
            if (TileGrid.GetGrid.GetTileAtPos(transform.position).tileType == TileType.Water)
                speed *= .33f;
            transform.position = Vector3.MoveTowards(transform.position, currWaypoint, speed);
            yield return null;
        }
    }

    public Stat GetStat (string name)
    {
        return stats.Find(stat => stat.name == name);
    }

    protected virtual void SetStats ()
    {

    }
}

[System.Serializable]
public class Stat
{
    public string name;
    public float value;

    public Stat (string _name, float _value = 1)
    {
        name = _name;
        value = _value;
    }
}

public enum AIType { Human, Zombie, Other }
