using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class Menus : MonoBehaviour
{
    public static Menus GetMenus { get; private set; }
    public bool IsOpen { get; private set; }
    WorldController worldController;

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

    [SerializeField]
    Transform scheduleUI = null;
    [SerializeField]
    TMP_Dropdown dropdown = null;
    [SerializeField]
    Button button = null;

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
        worldController = WorldController.GetWorldController;
        canvas.SetActive(true);
        GetMenus = this;
        IsOpen = mainMenu.activeSelf;
        stats = statsUI.GetChild(0).GetChild(1);
        worldController.SelectedCitizen = null;
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

    private void Start ()
    {
        ShowCitizenStats(0);
        ShowSchedule(0);
    }

    private void Update ()
    {
        if (!KeyBindings.GetKeyBindings.IsChanging)
        {
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown ||
                KeyBindings.GetKeyBindings.GetKey(BindingsNames.select).KeyDown)
            {
                if (statsUI.GetChild(0).gameObject.activeSelf && !IsOverUI())
                    ShowCitizenStats(0);
                if (scheduleUI.GetChild(0).gameObject.activeSelf && !IsOverUI())
                    ShowSchedule(0);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMenu(!IsOpen);
            }
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.citizenDetails).KeyDown)
            {
                ShowCitizenStats();
            }
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.showSchedule).KeyDown)
            {
                ShowSchedule();
            }
        }
    }

    public void ShowSideBars (bool stats = false, bool schedule = false)
    {
        statsUI.GetChild(1).gameObject.SetActive(stats);
        scheduleUI.GetChild(1).gameObject.SetActive(schedule);
    }

    public void ShowSchedule (int open = 2)
    {
        HideToolTip();
        Transform openUI = scheduleUI.GetChild(0);
        Transform closedUI = scheduleUI.GetChild(1);

        ShowCitizenStats(0);

        Schedule schedule = null;
        bool c = false;
        if (worldController.SelectedCitizen)
        {
            schedule = worldController.SelectedCitizen.GetComponent<Schedule>();
            c = true;
        }
        else if (worldController.SelectedTile)
        {
            schedule = worldController.SelectedTile.GetComponent<Schedule>();
        }
        else
        {
            openUI.gameObject.SetActive(false);
            closedUI.gameObject.SetActive(false);
            return;
        }

        if (open == 2)
        {
            if (openUI.gameObject.activeSelf)
                open = 0;
        }
        if (open == 0)
        {
            openUI.gameObject.SetActive(false);
            closedUI.gameObject.SetActive(true);
            return;
        }

        ScheduleDetails(schedule, c);

        if (c)
        {
            PlayerDetails(scheduleUI.GetChild(0).GetChild(1), worldController.SelectedCitizen);
        }
        else
        {
            TileDetails(scheduleUI.GetChild(0).GetChild(1), worldController.SelectedTile);
        }
        openUI.gameObject.SetActive(true);
        closedUI.gameObject.SetActive(false);
    }

    public void ScheduleDetails (Schedule schedule, bool c)
    {
        Transform daysUI = scheduleUI.GetChild(0).GetChild(2).GetChild(0);
        for (int x = 0; x < schedule.days.Count; x++)
        {
            foreach (Transform t in daysUI.GetChild(x))
            {
                Destroy(t.gameObject);
            }

            Day day = schedule.days[x];
            for (int y = 0; y < day.citizenPeriods.Count; y++)
            {
                if (c)
                {
                    Button btn = Instantiate(button, daysUI.GetChild(x));
                    if (day.tilePeriods[y] == null)
                    {
                        btn.interactable = false;
                        continue;
                    }
                    else
                        btn.interactable = true;

                    btn.onClick.AddListener(() => schedule.SetSchedule(x, y, day.tilePeriods[y]));
                }
                else
                {
                    TMP_Dropdown drp = Instantiate(dropdown, daysUI.GetChild(x));
                    drp.options = new List<TMP_Dropdown.OptionData>();
                    List<Citizen> citizens = worldController.citizensList;
                    foreach (Citizen citizen in citizens)
                    {
                        if (citizen.alive)
                            drp.options.Add(new TMP_Dropdown.OptionData(citizen.name));
                    }

                    drp.onValueChanged.AddListener(delegate { schedule.SetSchedule(x, y, drp.value); });
                }
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
        HideToolTip();
        Citizen c = worldController.SelectedCitizen;
        if (c == null)
        {
            statsUI.GetChild(1).gameObject.SetActive(false);
            statsUI.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            if (set >= 1)
            {
                ShowSchedule(0);
                PlayerDetails(stats, c);
                PlayerStats(c);
            }
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
        {
            hover = 0;
            ToolTip.GetToolTip.gameObject.SetActive(false);
        }
    }

    public void ChangeCitizenFocus (int value)
    {
        Citizen c = worldController.SelectedCitizen;
        if (c)
        {
            c.SetFocus(value, (TaskItems)(focus[value].value + (int)TaskItems.None));
            UpdateFocuses(c);
        }
    }

    void TileDetails (Transform ui, Tile tile)
    {
        ui.GetChild(0).GetComponent<TextMeshProUGUI>().text = tile.name;
        ui.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        ui.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
    }

    void PlayerDetails (Transform ui, Citizen citizen)
    {
        ui.GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.citizenName;
        ui.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Age: " + citizen.age.ToString();
        ui.GetChild(2).GetComponent<TextMeshProUGUI>().text = "DoB: " + citizen.dob;
        ui.GetChild(3).GetComponent<TextMeshProUGUI>().text = citizen.gender ? "M" : "F";
    }

    public void PlayerStats (Citizen citizen)
    {
        //WorldController.GetWorldController.SelectedCitizen = citizen;
        Transform statSection = stats.GetChild(4);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Building").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Cooking").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Cleanliness").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Artistic").value.ToString("##");
        statSection = stats.GetChild(5);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("HandToHand").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Strength").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Accuracy").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Agility").value.ToString("##");
        statSection.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Speed").value.ToString("##");
        statSection.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Aggression").value.ToString("##");
        statSection = stats.GetChild(6);
        statSection.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Composure").value.ToString("##");
        statSection.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Mundanity").value.ToString("##");
        statSection.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Logic").value.ToString("##");
        statSection.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Speech").value.ToString("##");
        statSection.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Intelligence").value.ToString("##");
        statSection.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = citizen.GetStat("Loyalty").value.ToString("##");
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
        worldController.SelectedCitizen = null;
        ShowCitizenStats(open ? 0 : 1);
        ShowSchedule(0);
        foreach (GameObject menu in otherMenus)
            menu.SetActive(false);
    }

    public void KeybindsMenu (bool open)
    {
        mainMenu.SetActive(!open);
        otherMenus[0].SetActive(open);
    }
}
