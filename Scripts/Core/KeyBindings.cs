using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyBindings : MonoBehaviour
{
    public static KeyBindings GetKeyBindings { get; private set; }
    bool changeBind = false;
    [SerializeField]
    GameObject bindObject = default;
    [SerializeField]
    Transform keybindsObject = default;
    KeyCode newKey;

    List<CustomBinding> keyCodes = new List<CustomBinding>();

    public void Awake ()
    {
        GetKeyBindings = this;
        //PlayerPrefs.DeleteAll();
        Init();
    }

    void Init ()
    {
        //Debug.Log(PlayerPrefs.GetString(BindingsNames.moveLeft));
        //CustomBinding b = new CustomBinding(BindingsNames.moveLeft, new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveLeft, "A")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveLeft + "Alt", "LeftArrow")) });
        keyCodes = new List<CustomBinding>
        {
            new CustomBinding(BindingsNames.moveLeft, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveLeft, "A")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveLeft + "Alt", "LeftArrow")) }),
            new CustomBinding(BindingsNames.moveRight, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveRight, "D")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveRight + "Alt", "RightArrow")) }),
            new CustomBinding(BindingsNames.moveUp, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveUp, "W")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveUp + "Alt", "UpArrow")) }),
            new CustomBinding(BindingsNames.moveDown, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveDown, "S")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.moveDown + "Alt", "DownArrow")) }),
            new CustomBinding(BindingsNames.select, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.select, "Mouse0")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.select + "Alt", "None")) }),
            new CustomBinding(BindingsNames.interact, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.interact, "Mouse1")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.interact + "Alt", "None")) }),
            new CustomBinding(BindingsNames.addTask, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.addTask, "LeftShift")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.addTask + "Alt", "RightShift")) }),
            new CustomBinding(BindingsNames.citizenDetails, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.citizenDetails, "C")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.citizenDetails + "Alt", "None")) }),
            new CustomBinding(BindingsNames.bulldoze, new KeyCode[]{ (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.bulldoze, "B")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(BindingsNames.bulldoze + "Alt", "None")) })
        };

        foreach (Transform transform in keybindsObject)
        {
            Destroy(transform.gameObject);
        }

        for (int i = 0; i < keyCodes.Count; i++)
        {
            GameObject newBindObj = Instantiate(bindObject);
            newBindObj.transform.SetParent(keybindsObject);

            for (int j = 0; j < newBindObj.GetComponentsInChildren<Button>().Length; j++)
            {
                newBindObj.GetComponentsInChildren<Button>()[j].GetComponentInChildren<Text>().text = keyCodes[i].keyCode[j].ToString();
                GameObject go = newBindObj.transform.GetChild(j).gameObject;
                newBindObj.GetComponentsInChildren<Button>()[j].onClick.AddListener(() => ChangeBind(go));
            }
            keyCodes[i].btn = newBindObj.transform;
            newBindObj.GetComponentInChildren<Text>().text = keyCodes[i].name;
            newBindObj.name = keyCodes[i].name;
            newBindObj.transform.localScale = Vector3.one;
        }
    }

    public void ChangeBind (GameObject obj)
    {
        if (!changeBind)
        {
            StartCoroutine(AssignKey(obj));
        }
    }

    public bool IsChanging { get { return changeBind; } }

    private void OnGUI ()
    {
        if (Event.current.isKey && changeBind)
        {
            newKey = Event.current.keyCode;
            changeBind = false;
        }
        else if (Event.current.isMouse && changeBind)
        {
            newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + Event.current.button);
            changeBind = false;
        }
    }

    public IEnumerator AssignKey (GameObject bindObj)
    {
        changeBind = true;
        yield return new WaitUntil(() => Input.anyKey || changeBind == false);

        //newKey = Event.current.keyCode;
        if (newKey != KeyCode.None)
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                newKey = KeyCode.None;
            }

            if (!Input.GetKey(KeyCode.Escape) || !Input.GetKey(KeyCode.Delete))
            {
                CustomBinding bind = GetKey(bindObj.transform.parent.name);
                int t = bindObj.transform.GetSiblingIndex();
                if (t == 0)
                {
                    bind.newBind(new KeyCode[] { newKey, bind.keyCode[1] });
                }
                else if (t == 1)
                {
                    bind.newBind(new KeyCode[] { bind.keyCode[0], newKey });
                }

                bind.btn.GetChild(t).GetComponentInChildren<Text>().text = newKey.ToString();
            }
            changeBind = false;
        }
        EventSystem.current.SetSelectedGameObject(null);
        yield break;
    }

    public void ResetToDefault ()
    {
        foreach (CustomBinding b in keyCodes)
        {
            b.SetToDefault();
            //for (int i = 0; i < b.keyCode.Length; i++)
            //{
            //    b.btn.GetChild(i).GetComponentInChildren<Text>().text = b.keyCode[i].ToString();
            //}
        }
        Init();
    }

    public CustomBinding GetKey (string name)
    {
        return keyCodes.Find(control => control.name == name);
    }
}

public class CustomBinding
{
    public string name;
    public KeyCode[] keyCode;
    public KeyCode[] defaultKeyCode;
    public Transform btn;

    public CustomBinding (string _name, KeyCode[] _keyCode)
    {
        name = _name;
        keyCode = _keyCode;
        if (PlayerPrefs.GetString(_name) == "")
        {
            defaultKeyCode = _keyCode;
        }
        else
        {
            EditPlayerPrefs(_keyCode);
        }
    }

    public void newBind (KeyCode[] _keyCode)
    {
        keyCode = _keyCode;
        EditPlayerPrefs(_keyCode);
    }

    public void SetToDefault ()
    {
        //keyCode = defaultKeyCode;
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.DeleteKey(name + "Alt");
    }

    public bool AnyInput
    {
        get
        {
            foreach (KeyCode binding in keyCode)
            {
                if (Input.GetKey(binding))
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    public bool KeyUp
    {
        get
        {
            foreach (KeyCode binding in keyCode)
            {
                if (Input.GetKeyUp(binding))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool KeyDown
    {
        get
        {
            foreach (KeyCode binding in keyCode)
            {
                if (Input.GetKeyDown(binding))
                {
                    return true;
                }
            }
            return false;
        }
    }

    void EditPlayerPrefs (KeyCode[] _keyCode)
    {
        PlayerPrefs.SetString(name, _keyCode[0].ToString());
        PlayerPrefs.SetString(name + "Alt", _keyCode[1].ToString());
    }
}

public class BindingsNames
{
    public static string moveLeft = "MoveLeft";
    public static string moveRight = "MoveRight";
    public static string moveUp = "MoveUp";
    public static string moveDown = "MoveDown";
    public static string select = "Select";
    public static string interact = "Interact";
    public static string citizenDetails = "CitizenDetails";
    public static string bulldoze = "Bulldoze";
    public static string addTask = "Add Task";
}
