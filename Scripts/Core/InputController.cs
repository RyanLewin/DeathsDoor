using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }
    WorldController worldController;
    KeyBindings keyBindings;
    Builder builder;

    private void Awake ()
    {
        instance = this;
        worldController = WorldController.GetWorldController;
        keyBindings = KeyBindings.GetKeyBindings;
    }

    private void Start ()
    {
        builder = Builder.instance;
    }

    private void Update ()
    {
        if (!KeyBindings.GetKeyBindings.IsChanging)
        {
            if (Menus.GetMenus.IsOverUI())
                return;

            if (builder.building || builder.destroy)
                return;

            if (keyBindings.GetKey(BindingsNames.interact).AnyInput)
            {
                Interact();
            }
            if (keyBindings.GetKey(BindingsNames.select).AnyInput)
            {
                Select();
            }
        }
    }

    void Interact ()
    {
        //Interact key down
        if (keyBindings.GetKey(BindingsNames.interact).KeyDown)
        {
            Transform hitObject = GetHitObject();
            if (!hitObject)
                return;
            Options.GetOptions.CloseOptions();

            Task t = new Task();
            Citizen c = worldController.SelectedCitizen;
                
            //if the hit object is a vehicle
            if (hitObject.GetComponent<Vehicle>())
            {
                Vehicle v = hitObject.GetComponent<Vehicle>();
                if (v.CanTakeControl)
                {
                    t = new Task(TaskItems.Vehicle, v.transform.position - v.transform.up, v.gameObject, c);
                }
                else if (c)
                {
                    if (v == c.vehicle)
                        c.GetOutOfVehicle();
                    else
                        Debug.Log("Already in Control");
                }
            }
            else if (hitObject.GetComponentInParent<Foliage>())
            {
                t = new Task(TaskItems.Mine, hitObject.position - transform.up, hitObject.gameObject, c);
            }
            //if the hit object is a tile
            else if (hitObject.GetComponent<Tile>())
            {
                Tile hitTile = hitObject.GetComponent<Tile>();
                worldController.NoLongerNeedsTask(c);
                if (hitTile.built)
                {
                    t = new Task(TaskItems.Move, hitTile.transform.position, hitTile.gameObject, true);
                }
                else
                {
                    t = new Task(TaskItems.Build, hitTile.transform.position, hitTile.gameObject, c);
                }
            }
            else if (hitObject.GetComponent<Item>())
            {
                t = new Task(TaskItems.Loot, hitObject.position, hitObject.gameObject, c);
            }

            if (t.task == TaskItems.None)
                return;

            GiveTask(t);
        }
    }

    void Select ()
    {
        Transform hitObject = GetHitObject();
        if (!hitObject)
        { 
            worldController.SelectedCitizen = null;
            return;
        }

        if (keyBindings.GetKey(BindingsNames.select).KeyDown)
        {
            Options.GetOptions.CloseOptions();

            if (hitObject.GetComponent<Citizen>())
            {
                worldController.SelectedTile = null;
                worldController.SelectedCitizen = hitObject.GetComponent<Citizen>();
            }
            else if (hitObject.GetComponent<Schedule>())
            {
                worldController.SelectedCitizen = null;
                if (hitObject.GetComponent<Tile>())
                {
                    worldController.SelectedTile = hitObject.GetComponent<Tile>();
                }
                else
                {
                    worldController.SelectedTile = null;
                }
            }
            else if (hitObject.GetComponent<Item>() || hitObject.GetComponentInParent<Foliage>())
            {
                Options.GetOptions.SetOwner(hitObject.gameObject);
            }
            else
            {
                worldController.SelectedTile = null;
                worldController.SelectedCitizen = null;
            }
        }
    }

    public void GiveTask (Task t)
    {
        Citizen c = worldController.SelectedCitizen;
        Options.GetOptions.CloseOptions();

        if (c)
        {
            switch (t.task)
            {
                case (TaskItems.Loot):
                    c.item = t.obj.GetComponent<Item>();
                    break;
                case (TaskItems.Vehicle):
                    t.obj.GetComponent<Vehicle>().inUse = true;
                    c.SetVehicle(t.obj.GetComponent<Vehicle>());
                    break;
            }

            if (worldController.taskList.Contains(t))
                worldController.taskList.Remove(t);

            if (keyBindings.GetKey(BindingsNames.addTask).AnyInput)
            {
                c.taskList.Add(t);
            }
            else
            {
                if (c.currTask.personal)
                {
                    if (c.currTask.task != TaskItems.None && c.currTask.task != TaskItems.Move)
                        c.taskList.Insert(0, c.currTask);
                }
                else
                {
                    worldController.AddTask(c.currTask);
                }
                worldController.NoLongerNeedsTask(c);
                c.GiveTask(t);
            }
        }
        else
        {
            if (!t.personal)
            {
                worldController.AddTask(t);
            }
        }
    }

    Transform GetHitObject ()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
        RaycastHit2D hitObject = Physics2D.Raycast(pos, Vector3.forward);
        return hitObject.transform;
    }
}
