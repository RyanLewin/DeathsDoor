using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI GetInvUI { get; private set; }
    public GameObject currentInv;
    public Transform invUI;
    public GameObject invItem;
    Transform inputField;
    int selectedItem;
    public bool open = false;

    private void Awake ()
    {
        GetInvUI = this;
        inputField = invUI.GetChild(2);
    }

    private void Update ()
    {
        if (!open)
            return;

        if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyDown)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
            Transform hit = Physics2D.Raycast(pos, Vector3.forward).transform;
            if (hit.tag == "Item" && hit.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                selectedItem = hit.GetSiblingIndex();
                for (int i = 0; i < invUI.GetChild(0).childCount; i++)
                {
                    if (i == selectedItem)
                        invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.blue;
                    else
                        if (invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>())
                        invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                }

                if (currentInv.GetComponent<Citizen>())
                {
                    inputField.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deposit";
                    inputField.GetChild(2).GetComponent<SVGImage>().color = new Color(221f / 255f, 5f / 255f, 0);
                }
                else
                {
                    inputField.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Pick Up";
                    inputField.GetChild(2).GetComponent<SVGImage>().color = new Color(74f / 255f, 221f / 255f, 0);
                }
                inputField.GetChild(1).GetComponent<TMP_InputField>().text = currentInv.GetComponent<Inventory>().items[selectedItem].count.ToString();
                inputField.gameObject.SetActive(true);
                return;
            }

            inputField.gameObject.SetActive(false);
            if (selectedItem >= 0)
            {
                if (currentInv.GetComponent<Inventory>().items[selectedItem] != null)
                    invUI.GetChild(0).GetChild(selectedItem).GetComponent<SpriteRenderer>().color = Color.white;
                selectedItem = -1;
            }
        }
    }

    public void EjectItem ()
    {
        int noToEject = int.Parse(inputField.GetChild(1).GetComponent<TMP_InputField>().text);
        Item item = currentInv.GetComponent<Inventory>().items[selectedItem];
        item.count = noToEject;
        if (currentInv.GetComponent<Citizen>())
        {
            currentInv.GetComponent<Citizen>().SetItemToReplace(item);
        }
        else
        {
            Citizen c = WorldController.GetWorldController.SelectedCitizen;
            if (c)
            {
                c.SetItemToLoot(item, true);
            }
            else
            {
                WorldController.GetWorldController.AddTask(new Task(TaskItems.Loot, currentInv.transform.position, item.gameObject, false));
            }
            currentInv.GetComponent<Inventory>().TakeItem(item, item.count);
        }
    }

    public void CheckValue ()
    {
        TMP_InputField txt = inputField.GetChild(1).GetComponent<TMP_InputField>();
        if (txt.text.EndsWith(@"a-zA-Z"))
        {
            txt.text.Remove(txt.text.Length - 1);
        }
        if (int.Parse(txt.text) > currentInv.GetComponent<Inventory>().items[selectedItem].count)
        {
            txt.text = currentInv.GetComponent<Inventory>().items[selectedItem].count.ToString();
        }
    }

    public void OpenInventory ()
    {

    }
}
