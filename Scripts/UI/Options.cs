using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Options : MonoBehaviour
{
    public static Options GetOptions { get; private set; }
    WorldController worldController;

    GameObject owner;
    [SerializeField]
    GameObject[] boxes;
    List<GameObject> options = new List<GameObject>();
    int counter = 0;

    // Update is called once per frame
    void Awake()
    {
        GetOptions = this;
        worldController = WorldController.GetWorldController;
        CloseOptions();
    }

    public void SetOwner (GameObject _owner)
    {
        owner = _owner;
        OpenOptions();
    }

    public void CloseOptions ()
    {
        owner = null;
        gameObject.SetActive(false);
    }

    void OpenOptions ()
    {
        //Remove all old child Objects
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        if (owner.GetComponent<Item>())
        {
            PickUp();
            if (owner.GetComponent<Food>())
            {
                Eat();
            }
        }
        transform.position = owner.transform.position + Vector3.up;
        gameObject.SetActive(true);

        counter = 0;
    }

    void GetInVehicle ()
    {

    }

    void PickUp ()
    {
        bool bottom = counter % 2 == 0;
        GameObject option = Instantiate(boxes[bottom ? 0 : 1], transform);
        option.GetComponentInChildren<TextMeshProUGUI>().text = "Pick Up";
        if (bottom)
        {
            option.GetComponent<RectTransform>().localPosition = Vector3.zero;
        } else
        {
            option.GetComponent<RectTransform>().localPosition = new Vector3(0, .344f, 0);
        }

        Citizen c = worldController.SelectedCitizen;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((eventData) => { InputController.instance.GiveTask(new Task(TaskItems.Loot, owner.transform.position, owner, c)); });
        option.GetComponent<EventTrigger>().triggers.Add(entry);

        //if (c)
        //{
        //    worldController.NoLongerNeedsTask(c);
        //    c.item = owner.GetComponent<Item>();
        //}

        counter++;

        //InputController.instance.GiveTask(new Task(TaskItems.Loot, owner.transform.position, true));
        //Citizen c = WorldController.GetWorldController.SelectedCitizen;
        //if (c)
        //{
        //    c.GiveTask(new Task(TaskItems.Loot, owner.transform.position, true));
        //}
    }

    void Eat ()
    {
        bool bottom = counter % 2 == 0;
        GameObject option = Instantiate(boxes[bottom ? 0 : 1], transform);
        option.GetComponentInChildren<TextMeshProUGUI>().text = "Eat";
        if (bottom)
        {
            option.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        else
        {
            option.GetComponent<RectTransform>().localPosition = new Vector3(0, .344f, 0);
        }

        counter++;
    }
}
