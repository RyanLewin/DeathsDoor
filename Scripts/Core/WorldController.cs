using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController GetWorldController { get; private set; }
    
    public int GetDay { get; set; } = 27;
    public int GetMonth { get; set; } = 9;
    public int GetYear { get; set; } = 19;

    [SerializeField]
    List<Animal> animals = new List<Animal>();
    List<Animal> animalsInScene = new List<Animal>();

    [SerializeField]
    List<Item> items = new List<Item>();
    public List<Item> itemsInScene = new List<Item>();

    public List<Inventory> inventoriesInScene = new List<Inventory>();

    [SerializeField]
    Citizen citizen = default;
    List<Citizen> citizensList = new List<Citizen>();
    [SerializeField]
    List<Citizen> lookingForTask = new List<Citizen>();
    [SerializeField]
    public Color[] skinTones = default;
    [SerializeField]
    public Color[] hairColours = default;
    public List<Task> taskList = new List<Task>();
    bool newLevel = true;

    public Citizen selectedCitizen;
    public Citizen SelectedCitizen
    {
        get => selectedCitizen;
        set
        {
            selectedCitizen = value;
            Menus.GetMenus.statsUI.GetChild(1).gameObject.SetActive(selectedCitizen); 
        }
    }

    public void NeedsTask (Citizen c)
    {
        if (!lookingForTask.Contains(c))
        {
            lookingForTask.Add(c);
        }
    }

    public void NoLongerNeedsTask (Citizen c)
    {
        if (lookingForTask.Contains(c))
        {
            lookingForTask.Remove(c);
        }
    }

    public int woodCount = 0;
    public int stoneCount = 0;
    public int stoneBricksCount = 0;

    private void Awake ()
    {
        GetWorldController = this;
        newLevel = false;
    }

    private void Update ()
    {
        if (!KeyBindings.GetKeyBindings.IsChanging)
        {
            HandOutTasks();
        }
    }

    public void SpawnCitizen (Vector3 pos)
    {
        Citizen c = Instantiate(citizen, new Vector3(pos.x, pos.y, -0.01f), citizen.transform.rotation);
        citizensList.Add(c);
    }

    void HandOutTasks ()
    {
        if (lookingForTask.Count > 0 && taskList.Count > 0)
        {
            for (int iter = 0; iter < 3; iter++)
            {
                foreach (Citizen c in lookingForTask)
                {
                    if (c.currTask.task != TaskItems.None)
                        continue;

                    if (c.GetFocus[iter] == TaskItems.Loot)
                    {
                        Item item = FindClosestItem(c.transform.position);
                        if (item)
                        {
                            c.SetItemToLoot(item, false);
                            return;
                        }
                    }

                    if (c.GetFocus[iter] == taskList[0].task)
                    {
                        c.GiveTask(taskList[0]);
                        lookingForTask.Remove(c);
                        taskList.RemoveAt(0);
                        return;
                    }
                }
            }

            //Add task to end of taskList
            Task t = taskList[0];
            taskList.RemoveAt(0);
            taskList.Add(t);
        }
        else if (lookingForTask.Count > 0)
        {
            for (int iter = 0; iter < 3; iter++)
            {
                foreach (Citizen c in lookingForTask)
                {
                    if (c.currTask.task != TaskItems.None)
                        continue;

                    if (c.GetFocus[iter] == TaskItems.Loot)
                    {
                        Item item = FindClosestItem(c.transform.position);
                        if (item)
                        {
                            c.SetItemToLoot(item, false);
                            return;
                        }
                    }
                }
            }
        }
    }
   
    public Inventory CheckInventories (Item item, int count)
    {
        foreach (Inventory inv in inventoriesInScene)
        {
            if (inv.GetComponent<Tile>())
                if (!inv.GetComponent<Tile>().Built)
                    continue;
            if (inv.CheckForItem(item, count))
            {
                return inv;
            }
        }
        return null;
    }

    public Inventory CheckInventories (Item item, Vector3 pos)
    {
        foreach (Inventory inv in inventoriesInScene)
        {
            if (inv.GetComponent<Citizen>())
                continue;
            if (inv.GetComponent<Tile>())
                if (!inv.GetComponent<Tile>().Built)
                    continue;
            if (inv.CheckForItem(item))
            {
                return inv;
            }
        }
        return CheckForClosestInventory(pos);
    }

    public Inventory CheckForClosestInventory (Vector3 pos)
    {
        float dist = float.MaxValue;
        Inventory closest = null;
        foreach (Inventory inv in inventoriesInScene)
        {
            if (inv.GetComponent<Citizen>())
                continue;
            if (inv.GetComponent<Tile>())
                if (!inv.GetComponent<Tile>().Built)
                    continue;
            if (inv.CheckIfFull())
                continue;
            if (Vector3.Distance(inv.transform.position, pos) < dist)
            {
                dist = Vector3.Distance(inv.transform.position, pos);
                closest = inv;
            }
        }
        return closest;
    }

    public Item FindClosestItem (Vector3 pos, Item item = null)
    {
        float dist = float.MaxValue;
        Item closest = null;
        foreach (Item i in itemsInScene)
        {
            if (!i.toBeLooted)
                continue;
            if (Vector3.Distance(i.transform.position, pos) < dist)
            {
                dist = Vector3.Distance(i.transform.position, pos);
                closest = i;
            }
        }
        return closest;
    }

    public Animal FindClosestAnimal (Vector3 pos)
    {
        float dist = float.MaxValue;
        Animal closest = null;
        foreach(Animal a in animalsInScene)
        {
            if (Vector3.Distance(a.transform.position, pos) < dist)
            {
                dist = Vector3.Distance(a.transform.position, pos);
                closest = a;
            }
        }
        return closest;
    }

    public Task GetTaskAtTile (Vector3 pos)
    {
        foreach (Task t in taskList)
        {
            if (t.location == pos)
            {
                taskList.Remove(t);
                return t;
            }
        }
        return new Task();
    }

    public Task GetTaskOfType (List<TaskItems> type)
    {
        List<TaskItems> items = type;
        foreach (Task t in taskList)
        {
            if (t.task == items[0])
            {
                taskList.Remove(t);
                return t;
            }
        }
        items.RemoveAt(0);
        if (items.Count > 0)
            return GetTaskOfType(items);
        else
            return new Task();
        //return taskList[0];
    }

    public bool AddTask (Task task)
    {
        if (taskList.Contains(task))
            return false;
        if (task.task == TaskItems.None || task.task == TaskItems.Move)
            return false;
        if (task.personal)
            return false;
        foreach (Task t in taskList)
        {
            if (t.location == task.location)
                return false;
        }
        taskList.Add(task);
        return true;
    }

    /// <summary>
    /// If _month 
    /// </summary>
    /// <param name="_month">If 12 then pick a random month</param>
    /// <returns></returns>
    public string GetMonthString (bool random = false)
    {
        string date = "";
        int _month;
        if (random)
            _month = Random.Range(0, 12);
        else
            _month = GetMonth;

        switch (_month)
        {
            case (0):
                date = "Jan ";
                break;
            case (1):
                date = "Feb ";
                break;
            case (2):
                date = "Mar ";
                break;
            case (3):
                date = "Apr ";
                break;
            case (4):
                date = "May ";
                break;
            case (5):
                date = "Jun ";
                break;
            case (6):
                date = "Jul ";
                break;
            case (7):
                date = "Aug ";
                break;
            case (8):
                date = "Sep ";
                break;
            case (9):
                date = "Oct ";
                break;
            case (10):
                date = "Nov ";
                break;
            case (11):
                date = "Dec ";
                break;
        }
        return date;
    }
}
