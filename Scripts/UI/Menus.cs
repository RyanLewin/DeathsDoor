using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    public static Menus GetMenus { get; private set; }
    public bool IsOpen { get; private set; }

    [SerializeField]
    GameObject canvas = default;
    [SerializeField]
    GameObject mainMenu = default;
    [SerializeField]
    GameObject[] otherMenus = default;
    
    public Transform statsUI;
    Transform stats;
    int hover;
    [SerializeField]
    TMP_Dropdown[] focus = default;

    public Citizen invCitizen;
    public Transform invUI;
    public GameObject invItem;

    Dictionary<string, string> Descriptions = new Dictionary<string, string>
    {
        { "Building", "Affects how efficient this citizen can build buldings and furniture."},
        { "Cooking", "Affects the quality of food this citizen can make." },
        { "Cleanliness", "Affects how efficiently this citizen can clean." },
        { "Artistic", "Affects the quality of art pieces made by this citizen." },
        { "HandToHand", "Affects how good this citizen is in hand to hand combat, fists/swords - not recommended vs zombies." },
        { "Strength", "Limit how much this citizen can carry and the size of weapons carryable." },
        { "Accuracy", "Affects how accurate this citizen is, longer range weapons require higher accuracy." },
        { "Agility", "Affects how well this citizen can dodge attacks." },
        { "Speed", "Affects how fast this citizen can run." },
        { "Aggression", "Affects how easily this citizen becomes aggressive, even towards friends." },
        { "Composure", "Affects how often this citizen will panic, e.g. whilst reloading, stumbling when running etc." },
        { "Mundanity", "Affects how much they prefer mundane tasks - lower values will dislike cleaning, cooking etc." },
        { "Logic", "Affects this citizens positioning and choices." },
        { "Speech", "Affects how well this citizen can talk, diffusing situations, making deals - not recommended with zombies." },
        { "Intelligence", "Affects how smart this citizen is, affects their crafting abilities." },
        { "Loyalty", "Affects how easy this citizen will defect groups, either to or from yours." }
    };

    // Start is called before the first frame update
    void Awake ()
    {
        canvas.SetActive(true);
        GetMenus = this;
        IsOpen = mainMenu.activeSelf;
        stats = statsUI.GetChild(0).GetChild(1);
        WorldController.GetWorldController.SelectedCitizen = null;
        ShowCitizenStats(0);
        if (!IsOpen)
        {
            foreach (GameObject go in otherMenus)
            {
                if (go.activeSelf)
                {
                    IsOpen = true;
                    break;
                }
            }
        }
    }

    private void Update ()
    {
        if (!KeyBindings.GetKeyBindings.IsChanging)
        {
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown ||
                KeyBindings.GetKeyBindings.GetKey(BindingsNames.select).KeyDown)
            {
                if (statsUI.GetChild(0).gameObject.activeSelf && !IsOverUI())
                {
                    ShowCitizenStats(0);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMenu(!IsOpen);
            }
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.citizenDetails).KeyDown)
            {
                ShowCitizenStats();
            }
        }
    }

    public void CitizensInventory ()
    {
        int val = invUI.GetChild(1).GetComponent<TMP_Dropdown>().value;
        //Citizen c = WorldController.GetWorldController.SelectedCitizen;
        invCitizen.invSetting = val;
        if (val == 1)
            invCitizen.GiveTask(new Task(TaskItems.Loot));
    }

    // 0 == Closed; 1 == Open; 2 == opposite of current
    public void ShowCitizenStats (int set = 2)
    {
        Citizen c = WorldController.GetWorldController.SelectedCitizen;
        if (c == null)
        {
            statsUI.GetChild(1).gameObject.SetActive(false);
            statsUI.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            if (set >= 1)
                PlayerDetails(c);
            statsUI.GetChild(0).gameObject.SetActive(set == 2 ? !statsUI.GetChild(0).gameObject.activeSelf : set == 0 ? false : true);
            statsUI.GetChild(1).gameObject.SetActive(!statsUI.GetChild(0).gameObject.activeSelf);
        }
    }

    public bool IsOverUI ()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void ShowToolTip (Transform name)
    {
        string text;
        hover++;
        if (Descriptions.TryGetValue(name.name, out text))
        {
            ToolTip.GetToolTip.gameObject.SetActive(true);
            ToolTip.GetToolTip.SetText(text);
        }
        else
        {
            Debug.Log("Failed to get description");
        }
    }

    public void HideToolTip ()
    {
        hover--;
        if (hover <= 0)
            ToolTip.GetToolTip.gameObject.SetActive(false);
    }

    public void ChangeCitizenFocus (int value)
    {
        Citizen c = WorldController.GetWorldController.SelectedCitizen;
        if (c)
        {
            c.SetFocus(value, (TaskItems)(focus[value].value + (int)TaskItems.None));
            UpdateFocuses(c);
        }
    }

    public void PlayerDetails (Citizen citizen)
    {
        //WorldController.GetWorldController.SelectedCitizen = citizen;
        stats.GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.citizenName;
        stats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Age: " + citizen.age.ToString();
        stats.GetChild(2).GetComponent<TextMeshProUGUI>().text = "DoB: " + citizen.dob;
        Transform statSection = stats.GetChild(3);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Building").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Cooking").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Cleanliness").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Artistic").value.ToString("##");
        statSection = stats.GetChild(4);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("HandToHand").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Strength").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Accuracy").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Agility").value.ToString("##");
        statSection.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Speed").value.ToString("##");
        statSection.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Aggression").value.ToString("##");
        statSection = stats.GetChild(5);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Composure").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Mundanity").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Logic").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Speech").value.ToString("##");
        statSection.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Intelligence").value.ToString("##");
        statSection.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Loyalty").value.ToString("##");
        stats.GetChild(6).GetComponent<TextMeshProUGUI>().text = citizen.gender ? "M" : "F";
        //stats.parent.gameObject.SetActive(true);
        UpdateFocuses(citizen);
    }

    public void UpdateFocuses (Citizen citizen)
    {
        focus[0].value = (int)citizen.GetFocus[0] - (int)TaskItems.None;
        focus[1].value = (int)citizen.GetFocus[1] - (int)TaskItems.None;
        focus[2].value = (int)citizen.GetFocus[2] - (int)TaskItems.None;
    }

    public void SetMenu (bool open)
    {
        IsOpen = open;
        mainMenu.SetActive(open);
        WorldController.GetWorldController.SelectedCitizen = null;
        ShowCitizenStats(open ? 0 : 1);
        foreach (GameObject menu in otherMenus)
            menu.SetActive(false);
    }

    public void KeybindsMenu (bool open)
    {
        mainMenu.SetActive(!open);
        otherMenus[0].SetActive(open);
    }
}
