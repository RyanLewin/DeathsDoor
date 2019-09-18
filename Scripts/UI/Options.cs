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

        Vector3 offset = Vector3.up;
        if (owner.GetComponent<Item>())
        {
            offset = owner.GetComponent<Item>().offset;
            PickUp();
            if (owner.GetComponent<Food>())
            {
                Eat();
            }
        }
        else if (owner.GetComponentInParent<Foliage>())
        {
            offset = new Vector3(0, 0, -0.06f);
            ChopDown();
        }

        transform.position = owner.transform.position + offset;
        gameObject.SetActive(true);

        counter = 0;
    }

    void GetInVehicle ()
    {

    }

    void PickUp ()
    {
        //determine which button it is, top or bottom and then instantiate and position it
        bool bottom = counter % 2 == 0;
        GameObject option = Instantiate(boxes[bottom ? 0 : 1], transform);
        option.GetComponentInChildren<TextMeshProUGUI>().text = "Pick Up";
        if (bottom)
        {
            option.GetComponent<RectTransform>().localPosition = Vector3.zero;
        } else {
            option.GetComponent<RectTransform>().localPosition = new Vector3(0, .344f, 0);
        }

        //if there is a selected citizen, it will be a personal task.
        Citizen c = worldController.SelectedCitizen;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        //add listener to the button which will call the input controller
        entry.callback.AddListener((eventData) => { InputController.instance.GiveTask(new Task(TaskItems.Loot, owner.transform.position, owner, c)); });
        option.GetComponent<EventTrigger>().triggers.Add(entry);

        //used for checking top or bottom button
        counter++;
    }

    void Eat ()
    {
        bool bottom = counter % 2 == 0;
        GameObject option = Instantiate(boxes[bottom ? 0 : 1], transform);
        option.GetComponentInChildren<TextMeshProUGUI>().text = "Eat";
        if (bottom)
        {
            option.GetComponent<RectTransform>().localPosition = Vector3.zero;
        } else {
            option.GetComponent<RectTransform>().localPosition = new Vector3(0, .344f, 0);
        }

        counter++;
    }

    void ChopDown ()
    {
        bool bottom = counter % 2 == 0;
        GameObject option = Instantiate(boxes[bottom ? 0 : 1], transform);
        option.GetComponentInChildren<TextMeshProUGUI>().text = "Harvest";
        if (bottom)
        {
            option.GetComponent<RectTransform>().localPosition = Vector3.zero;
        } else {
            option.GetComponent<RectTransform>().localPosition = new Vector3(0, .344f, 0);
        }

        Citizen c = worldController.SelectedCitizen;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((eventData) => { InputController.instance.GiveTask(new Task(TaskItems.Mine, owner.transform.position - transform.up, owner, c)); });
        option.GetComponent<EventTrigger>().triggers.Add(entry);

        counter++;
    }
}
