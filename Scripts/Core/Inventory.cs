using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    InventoryUI inventoryUI;
    [SerializeField]
    int invSize = 4;
    public List<Item> items = new List<Item>();
    List<int> itemsCount = new List<int>(); 
    public Vector3 offset = new Vector3(0, 1, -.04f);
    Transform invUI;
    GameObject invItem;
    Transform inputField;
    bool open = false;
    public bool full = false;
    int selectedItem = -1;

    private void Awake ()
    {
        inventoryUI = InventoryUI.GetInvUI;
        invUI = inventoryUI.invUI;
        invItem = inventoryUI.invItem;
        inputField = invUI.GetChild(2);
        WorldController.GetWorldController.inventoriesInScene.Add(this);
        for (int i = 0; i < invSize; i++)
            items.Add(null);
        if (invUI.gameObject.activeSelf)
        {
            CloseInventory();
        }
        invUI.GetChild(2).gameObject.SetActive(false);
    }

    private void Update ()
    {
        if (!open)
            return;

        UpdatePosition();

        if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
            Transform hit = Physics2D.Raycast(pos, Vector3.forward).transform;
            if (hit.tag == "Item" && hit.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            else if (hit.gameObject != transform.gameObject && hit.gameObject != invUI.gameObject)
            {
                CloseInventory();
            }
        }
    }

    private void LateUpdate ()
    {
        if (open)
            return;

        if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
            Transform hit = Physics2D.Raycast(pos, Vector3.forward).transform;
            if (hit == transform)
            {
                inventoryUI.currentInv = gameObject;
                OpenInventory();
            }
        }
    }

    public bool CheckIfFull ()
    {
        foreach (Item i in items)
        {
            if (i == null)
            {
                full = false;
                return full;
            }
        }
        full = true;
        return full;
    }

    public bool AddItem (Item item, int count)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i])
                continue;
            if (item.GetID == items[i].GetID)
            {
                items[i].count += count;
                UpdateInventory();
                return true;
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                CheckIfFull();
                UpdateInventory();
                return true;
            }
        }
        return false;
    }

    public bool CheckForEmptySlot ()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckForItem (Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i])
                continue;
            if (item.GetID == items[i].GetID)
            {
                return true;
            }
        }
        return false;
    }

    public Item TakeItem (Item item, int count)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i])
                continue;
            if (item.GetID == items[i].GetID)
            {
                Item it;
                if (count < items[i].count)
                {
                    it = items[i];
                    it.count = count;
                    items[i].count -= count;
                    UpdateInventory();
                    return it;
                }

                it = items[i];
                it.count = items[i].count;
                items[i] = null;
                full = false;
                UpdateInventory();
                return it;
            }
        }
        return null;
    }

    void CloseInventory ()
    {
        invUI.gameObject.SetActive(false);
        open = false;
        inventoryUI.open = false;
    }
    
    void UpdateInventory ()
    {
        if (!open)
            return;
        for (int j = invUI.GetChild(0).childCount - 1; j >= 0; j--)
        {
            Destroy(invUI.GetChild(0).GetChild(j).gameObject);
        }

        //foreach (Transform t in invUI.GetChild(0))
        //{
        //    Destroy(t.gameObject);
        //}
        
        for (int i = 0; i < invSize; i++)
        {
            GameObject go = Instantiate(invItem, invUI.GetChild(0));
            go.transform.position += new Vector3(0, 0, -.01f);
            if (items[i] != null)
            {
                go.AddComponent<BoxCollider2D>();
                go.GetComponent<BoxCollider2D>().size = new Vector2(.9f, .9f);
                go.AddComponent<SpriteRenderer>();
                go.GetComponent<SpriteRenderer>().sprite = items[i].GetComponent<SpriteRenderer>().sprite;
                go.GetComponent<SpriteRenderer>().sortingLayerID = invUI.GetComponent<Canvas>().sortingLayerID;
                go.GetComponent<SpriteRenderer>().sortingOrder = invUI.GetComponent<Canvas>().sortingOrder + 1;
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = items[i].count.ToString();
            }
            else
            {
                go.AddComponent<Image>();
                go.GetComponent<Image>().color = Color.gray;
                Destroy(go.transform.GetChild(0).gameObject);
            }
        }
    }

    void UpdatePosition ()
    {
        invUI.position = transform.position + offset;
    }

    void OpenInventory ()
    {
        //GameObject invSettingsDropdown = invUI.GetChild(1).gameObject;
        //if (GetComponent<Citizen>())
        //{
        //    Menus.GetMenus.invCitizen = GetComponent<Citizen>();
        //    invSettingsDropdown.GetComponent<TMP_Dropdown>().value = GetComponent<Citizen>().invSetting;
        //    invSettingsDropdown.SetActive(true);
        //}
        //else
        //{
        //    invSettingsDropdown.SetActive(false);
        //}

        open = true;
        inventoryUI.open = true;
        UpdateInventory();
        invUI.GetComponent<RectTransform>().sizeDelta = new Vector2(invUI.GetComponent<RectTransform>().sizeDelta.x, 1 + ((Mathf.Ceil(invSize / 4) - 1) * .93f));
        invUI.GetComponent<BoxCollider2D>().size = invUI.GetComponent<RectTransform>().sizeDelta;
        invUI.GetComponent<BoxCollider2D>().offset = new Vector2(0, invUI.GetComponent<RectTransform>().sizeDelta.y / 2);
        offset = new Vector3(0, .5f, -.4f);
        UpdatePosition();
        invUI.gameObject.SetActive(true);
    }
}
