using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Citizen : MonoBehaviour
{
    WorldController worldController;
    Pathfinder pathfinder;
    PathRequestManager requestManager;
    Vector3[] path;
    int targetIndex;
    public Task currTask { get; private set; }
    public List<Task> taskList { get; set; } = new List<Task>();

    bool newBorn = false;
    bool newCitizen = true;

    public string citizenName = "First Last";
    public bool gender = true;
    public int age = 30;
    public string dob = "Sep '96";
    public int happiness = 60;
    int maxHappiness = 100;
    public int health = 100;
    int maxHealth = 100;

    int ability = 1;
    int potential = 100;

    public Vector2 target;
    bool move = false;

    [Header("Housekeeping")]
    public List<Stat> stats = new List<Stat>();
    
    public Tile taskTile;
    float buildTimer;

    public Vehicle vehicle { get; private set; }
    bool inVehicle = false;

    public Item item;
    Item itemToLoot;
    Item itemToReplace;
    bool pickUp = true;
    public int invSetting = 2;
    public List<Item> inventory = new List<Item>();
    
    private void Start ()
    {
        worldController = WorldController.GetWorldController;
        pathfinder = worldController.GetComponent<Pathfinder>();
        requestManager = worldController.GetComponent<PathRequestManager>();
        if (newCitizen)
            SetStats(newBorn);
        currTask = new Task(TaskItems.None, transform.position);
        GetComponent<Appearance>().SetAppearance();
    }

    private void Update ()
    {
        if (worldController.SelectedCitizen == this)
        {
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown)
            {
                if (Menus.GetMenus.IsOverUI())
                    return;
            }
        }

        if (currTask.task == TaskItems.None)
        {
            if (taskList.Count > 0)
            {
                GiveTask(taskList[0]);
                taskList.RemoveAt(0);
            }
            else
            {
                worldController.NeedsTask(this);
            }
        }
        else
        {
            if (!taskTile)
                EndTask();
        }

        switch (currTask.task)
        {
            case (TaskItems.None):
                break;
            case (TaskItems.Build):
                if (currTask.obj.GetComponent<Tile>().Built)
                {
                    currTask = new Task();
                    break;
                }
                GetOutOfVehicle();
                SetToBuild();
                break;
            case (TaskItems.Move):
                if (!move)
                {
                    Move();
                    move = true;
                }
                break;
            case (TaskItems.Vehicle):
                GetInVehicle();
                break;
            case (TaskItems.Loot):
                Loot();
                break;
            case (TaskItems.Mine):
                SetToMine();
                break;
            case (TaskItems.Harvest):
            case (TaskItems.Hunt):
            case (TaskItems.Cook):
            case (TaskItems.Defend):
            default:
                GetOutOfVehicle();
                currTask = new Task();
                break;
        }
    }

    void SetToMine ()
    {
        if (Vector2.Distance(currTask.location, transform.position) < 1f)
        {
            Foliage foliage = currTask.obj.GetComponentInParent<Foliage>();
            if (foliage)
            {
                if (buildTimer < foliage.timeToChop)
                {
                    buildTimer += Time.deltaTime * (GetStat("Strength").value / 5);
                }
                else
                {
                    foliage.ChopDown();
                    EndTask();
                }
            }
        }
        else
        {
            if (!move)
            {
                Move();
                move = true;
            }
        }
    }

    public void SetItemToLoot (Item item, bool personal)
    {
        itemToLoot = item;
        pickUp = true;
        taskTile = TileGrid.GetGrid.GetTileAtPos(itemToLoot.transform.position);
        taskList.Add(new Task(TaskItems.Loot, itemToLoot.transform.position, itemToLoot.gameObject, personal));
        worldController.NoLongerNeedsTask(this);
    }
    
    void Loot ()
    {
        if (pickUp && invSetting != 1)
        {
            if (itemToLoot != null)
            {
                if (Vector2.Distance(currTask.location, transform.position) < 1f)
                {
                    if (!inVehicle)
                    {
                        GetComponent<Inventory>().AddItem(itemToLoot, itemToLoot.count);
                        itemToLoot.transform.SetParent(transform);
                    }
                    else
                    {
                        vehicle.GetComponent<Inventory>().AddItem(itemToLoot, itemToLoot.count);
                        itemToLoot.transform.SetParent(vehicle.transform);
                    }
                    //inventory.Add(itemToLoot);
                    itemToLoot.gameObject.SetActive(false);
                    itemToLoot.SetToBeLooted = false;
                    itemToLoot = null;
                    currTask = new Task();
                }
                else
                {
                    if (!move)
                    {
                        Move();
                        move = true;
                    }
                }
            }
            else
            {
                if (GetComponent<Inventory>().CheckIfFull())
                {
                    if (invSetting != 0)
                        pickUp = false;
                    else
                        EndTask();
                }
                else
                {
                    itemToLoot = worldController.FindClosestItem(transform.position);
                    if (itemToLoot != null)
                    {
                        currTask = new Task(TaskItems.Loot, itemToLoot.transform.position);
                        worldController.NoLongerNeedsTask(this);
                    }
                    else
                    {
                        EndTask();
                    }
                }
            }
        }
        else
        {
            if (itemToReplace != null)
            {
                if (Vector2.Distance(currTask.location, transform.position) < 1f)
                {
                    //Item replacedItem = GetComponent<Inventory>().TakeItem(itemToReplace, itemToReplace.count);
                    Inventory inv = worldController.CheckInventories(itemToReplace, transform.position);
                    itemToReplace.transform.SetParent(inv.transform);
                    itemToReplace.transform.localPosition = Vector3.zero;
                    inv.AddItem(itemToReplace, itemToReplace.count);
                    EndTask();
                }
                else
                {
                    if (!move)
                    {
                        Move();
                        move = true;
                    }
                }
            }
        }
    }

    public void SetItemToReplace (Item item)
    {
        pickUp = false;
        itemToReplace = item;
        Inventory inv = worldController.CheckInventories(itemToReplace, transform.position);
        if (inv)
        {
            Vector3 pos = inv.transform.position;
            if (vehicle)
            {
                if (vehicle.GetComponent<Inventory>() == inv)
                    pos = transform.position;
            }
            taskTile = TileGrid.GetGrid.GetTileAtPos(pos);
            currTask = new Task(TaskItems.Loot, pos, item.gameObject, true);
            worldController.NoLongerNeedsTask(this);
        }
        else
        {
            itemToReplace = null;
        }
    }

    public void SetVehicle (Vehicle v)
    {
        vehicle = v;
    }

    public void GetOutOfVehicle ()
    {
        if (!inVehicle)
            return;
        vehicle.inUse = false;
        inVehicle = false;
        vehicle.transform.SetParent(null);
    }

    void GetInVehicle ()
    {
        if (Vector2.Distance(currTask.location, transform.position) < 0.5f)
        {
            inVehicle = true;
            vehicle.transform.SetParent(transform);
            vehicle.transform.position = transform.position + transform.up;
            vehicle.transform.rotation = transform.rotation;
            currTask = new Task();
        }
        else
        {
            if (!move)
            {
                Move();
                move = true;
            }
        }
    }

    void SetToBuild ()
    {
        if (Vector2.Distance(currTask.location, transform.position) < 0.2f)
        {
            //move = false;
            if (buildTimer < taskTile.timeToBuild)
            {
                buildTimer += Time.deltaTime * (GetStat("Building").value / 5);
            }
            else
            {
                taskTile.Built = true;
                buildTimer = 0;
                currTask = new Task();
                taskTile = null;
            }
        }
        else
        {
            if (!move)
            {
                Move();
                move = true;
            }
        }
    }

    public List<TaskItems> GetFocus { get; } = new List<TaskItems>() { TaskItems.Build, TaskItems.None, TaskItems.None };

    public void SetFocus (int value, TaskItems focus)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == value)
                continue;
            if (GetFocus[i] == TaskItems.None)
                continue;

            if (focus == GetFocus[i])
            {
                GetFocus[i] = GetFocus[value];
            }
        }
        GetFocus[value] = focus;
        ReorderFocuses();
    }

    void ReorderFocuses ()
    {
        if (GetFocus[0] == TaskItems.None)
        {
            if (GetFocus[1] != TaskItems.None)
            {
                GetFocus[0] = GetFocus[1];
                GetFocus[1] = TaskItems.None;
            }
            else if (GetFocus[2] != TaskItems.None)
            {
                GetFocus[0] = GetFocus[2];
                GetFocus[2] = TaskItems.None;
            }
            else return;
        }
        if (GetFocus[1] == TaskItems.None)
        {
            if (GetFocus[2] != TaskItems.None)
            {
                GetFocus[1] = GetFocus[2];
                GetFocus[2] = TaskItems.None;
            }
        }
    }

    void Move ()
    {
        requestManager.RequestPath(transform.position, currTask.location, OnPathFound);
    }

    void EndTask ()
    {
        StopCoroutine(FollowPath());
        currTask = new Task();
        targetIndex = 0;
        path = new Vector3[0];
        move = false;
        taskTile = null;
        buildTimer = 0;
        itemToLoot = null;
        itemToReplace = null;
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

    IEnumerator FollowPath ()
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

    public void GiveTask (Task task)
    {
        if (currTask.task == TaskItems.Move)
            EndTask();
        currTask = task;
        taskTile = TileGrid.GetGrid.GetTileAtPos(currTask.location);
        taskTile.assignedCitizen = this;
        if (currTask.task == TaskItems.Loot)
        {
            if (task.obj)
            {
                itemToLoot = task.obj.GetComponent<Item>();
                pickUp = true;
            }
        }

        if (currTask.task == TaskItems.Build)
        {
            buildTimer = taskTile.buildTime;
        }
    }

    void GetTask ()
    {
        if (worldController.taskList.Count > 0)
        {
            currTask = worldController.GetTaskOfType(GetFocus);
            taskTile = TileGrid.GetGrid.GetTileAtPos(currTask.location);
            taskTile.assignedCitizen = this;
            buildTimer = taskTile.buildTime;
        }
    }

    public Stat GetStat (string name)
    {
        return stats.Find(stat => stat.name == name);
    }

    void SetStats (bool isNewBorn)
    {
        stats = new List<Stat>
        {
            new Stat("Building"),
            new Stat("Cooking"),
            new Stat("Cleanliness"),
            new Stat("Artistic"),
            new Stat("HandToHand"),
            new Stat("Strength"),
            new Stat("Accuracy"),
            new Stat("Agility"),
            new Stat("Speed"),
            new Stat("Aggression"),
            new Stat("Composure"),
            new Stat("Mundanity"),
            new Stat("Logic"),
            new Stat("Speech"),
            new Stat("Intelligence"),
            new Stat("Loyalty")
        };
        
        gender = Random.Range(0, 2) != 0;
        string[] lines;
        int randomLineNumber = 0; 
        if (gender)
        {
            lines = File.ReadAllLines("Assets/Scripts/AI/MaleNames.txt");
            randomLineNumber = Random.Range(0, lines.Length);
        }
        else
        {
            lines = File.ReadAllLines("Assets/Scripts/AI/FemaleNames.txt");
            randomLineNumber = Random.Range(0, lines.Length);
        }
        citizenName = lines[randomLineNumber];

        SetAge(isNewBorn);
        potential = Random.Range(17, 101);

        if (age >= 7)
        {
            ability = Random.Range(16, potential);

            int pointsToPick = ability;
            do
            {
                foreach (Stat stat in stats)
                {
                    int weight = Random.Range(0, 5);

                    int rand;
                    if (weight <= 1)
                        continue;
                    else if (weight <= 3)
                        rand = 1;
                    else
                        rand = Random.Range(2, 6);

                    if (rand > pointsToPick)
                    {
                        rand = pointsToPick;
                    }

                    stat.value += rand;
                    if (stat.value > 10)
                    {
                        int v = (int)stat.value - 10;
                        pointsToPick += v;
                        stat.value = 10;
                    }
                    pointsToPick -= rand;
                    if (pointsToPick <= 0)
                        break;
                }
            } while (pointsToPick > 0);

#if UNITY_EDITOR
            stats[8].value = 10;
#endif
        }
        else
        {
            ability = stats.Count;
        }
    }

    public void SetAge (bool isNewBorn)
    {
        if (newBorn)
        {
            age = 0;
            int birthYear = worldController.GetYear;
            dob = worldController.GetMonthString() + "'" + birthYear.ToString("00");
        }
        else
        {
            age = Random.Range(14, 65);
            int birthYear = worldController.GetYear - age;
            if (birthYear < 0)
                birthYear = 100 + birthYear;
            dob = worldController.GetMonthString(true) + "'" + birthYear.ToString("00");
        }
    }
}

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
