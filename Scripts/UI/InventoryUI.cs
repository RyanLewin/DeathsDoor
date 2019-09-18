using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI GetInvUI { get; private set; }
    GameObject previousInv;
    public GameObject currentInv;
    public Transform invUI;
    public GameObject invItem;
    Transform inputField;
    int selectedItem = -1;
    public bool open = false;

    private void Awake ()
    {
        GetInvUI = this;
        inputField = invUI.GetChild(2);
        if (invUI.gameObject.activeSelf)
        {
            CloseInventory();
        }
        invUI.GetChild(2).gameObject.SetActive(false);
    }

    private void Update ()
    {
        if (!open)
        {
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.select).KeyDown)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
                Transform hit = Physics2D.Raycast(pos, Vector3.forward).transform;
                if (hit.GetComponent<Inventory>())
                {
                    previousInv = currentInv;
                    currentInv = hit.gameObject;
                    if (currentInv != previousInv)
                    {
                        if (previousInv)
                        {
                            previousInv.GetComponent<Inventory>().open = false;
                        }
                        if (hit.GetComponent<Citizen>())
                        {
                            return;
                        }
                    }
                    currentInv = hit.gameObject;
                    OpenInventory(currentInv.GetComponent<Inventory>());
                }
            }
        }
        else
        {
            UpdatePosition(currentInv.GetComponent<Inventory>().offset);

            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.select).KeyDown)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1));
                Transform hit = Physics2D.Raycast(pos, Vector3.forward).transform;
                //checks if an inventory item is selected
                if (hit.tag == "Item" && hit.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    selectedItem = hit.GetSiblingIndex();

                    //meant to change the selected items colour
                    for (int i = 0; i < invUI.GetChild(0).childCount; i++)
                    {
                        if (i == selectedItem)
                        {
                            invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.blue;
                        }
                        else
                        {
                            if (invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>())
                            {
                                invUI.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                            }
                        }
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
                else if (hit.gameObject != currentInv.gameObject && hit.gameObject != invUI.gameObject && hit.gameObject != inputField.gameObject)
                {
                    CloseInventory();
                    //turn of the input field and deselect all inv items
                    inputField.gameObject.SetActive(false);
                    if (selectedItem >= 0)
                    {
                        if (currentInv.GetComponent<Inventory>().items[selectedItem] != null)
                            invUI.GetChild(0).GetChild(selectedItem).GetComponent<SpriteRenderer>().color = Color.white;
                        selectedItem = -1;
                    }
                    currentInv = null;
                }
                else if (hit.gameObject != inputField.gameObject)
                {
                    //turn of the input field and deselect all inv items
                    inputField.gameObject.SetActive(false);
                    if (selectedItem >= 0)
                    {
                        if (currentInv.GetComponent<Inventory>().items[selectedItem] != null)
                            invUI.GetChild(0).GetChild(selectedItem).GetComponent<SpriteRenderer>().color = Color.white;
                        selectedItem = -1;
                    }
                }
            }
        }
    }

    public void EjectItem ()
    {
        //The number of items to "Eject"
        int noToEject = int.Parse(inputField.GetChild(1).GetComponent<TMP_InputField>().text);
        Item baseItem = currentInv.GetComponent<Inventory>().items[selectedItem];
        Item item;

        //if all of this item is being ejected, then close the input field else create
        //a new object of item with the same values so changing the count wont affect the inventory
        if (baseItem.count == noToEject)
        {
            item = baseItem;
            inputField.gameObject.SetActive(false);
            selectedItem = -1;
        }
        else
        {
            item = Instantiate(baseItem, baseItem.transform.parent);
        }

        item.count = noToEject;

        //if there is a selected citizen, this task is only for them so give it straight to them
        //else add it to the task list
        //remove the correct number of the item from the inventory
        Item ejectedItem = currentInv.GetComponent<Inventory>().TakeItem(item, item.count);
        ejectedItem.transform.SetParent(null);

        //If in a citizens inventory then it will be removing from their inventory
        if (currentInv.GetComponent<Citizen>())
        {
            currentInv.GetComponent<Citizen>().SetItemToReplace(ejectedItem);
        }
        else
        {
            ejectedItem.gameObject.SetActive(true);
            Citizen c = WorldController.GetWorldController.SelectedCitizen;
            ejectedItem.SetToBeLooted = true;
            if (c)
            {
                c.SetItemToLoot(ejectedItem, true);
            }
        }
    }

    public void CheckValue ()
    {
        TMP_InputField txt = inputField.GetChild(1).GetComponent<TMP_InputField>();
        //if null text then return
        if (txt.text == "")
            return;
        if (txt.text.EndsWith("\n"))
        {
            EjectItem();
            return;
        }
        //if non numeric value entered then delete it
        if (!txt.text.EndsWith(@"0-9"))
        {
            txt.text.Remove(txt.text.Length - 1);
        }
        //if the value is larger than the item count then lower it
        if (int.Parse(txt.text) > currentInv.GetComponent<Inventory>().items[selectedItem].count)
        {
            txt.text = currentInv.GetComponent<Inventory>().items[selectedItem].count.ToString();
        }
    }

    void CloseInventory ()
    {
        invUI.gameObject.SetActive(false);
        open = false;
        if (currentInv)
        {
            currentInv.GetComponent<Inventory>().open = false;
        }
    }

    public void OpenInventory (Inventory inv)
    {
        open = true;
        inv.open = true;
        inv.UpdateInventory();
        invUI.GetComponent<RectTransform>().sizeDelta = new Vector2(invUI.GetComponent<RectTransform>().sizeDelta.x, 1 + ((Mathf.Ceil(inv.InvSize / 4) - 1) * .93f));
        invUI.GetComponent<BoxCollider2D>().size = invUI.GetComponent<RectTransform>().sizeDelta;
        invUI.GetComponent<BoxCollider2D>().offset = new Vector2(0, invUI.GetComponent<RectTransform>().sizeDelta.y / 2);
        UpdatePosition(inv.offset);
        invUI.gameObject.SetActive(true);
    }

    public void UpdatePosition (Vector3 offset)
    {
        invUI.position = currentInv.transform.position + offset;
    }
}
