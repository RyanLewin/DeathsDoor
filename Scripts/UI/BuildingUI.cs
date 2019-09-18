using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BuildingUI : MonoBehaviour
{
    TileGrid tileGrid;
    Builder builder;
    [SerializeField]
    GameObject btnBuildMenu = default;
    [SerializeField]
    GameObject categories = default;
    [SerializeField]
    GameObject buildableTiles = default;
    [SerializeField]
    GameObject button = default;
    [SerializeField]
    Toggle toggleFill = default;
    Category openCategory = Category.WorldGen;

    Dictionary<int, Tile[]> tileCategories = new Dictionary<int, Tile[]>();

    private void Start ()
    {
        builder = Builder.instance;
        tileGrid = TileGrid.GetGrid;
        OpenMenu(false);

        int count = System.Enum.GetNames(typeof(Category)).Length;
        List<Tile[]> categories = new List<Tile[]>();
        for (int i = 1; i < count; i++)
        {
            List<Tile> category = new List<Tile>();
            foreach (TileGrid.AllTiles tile in tileGrid.tiles)
            {
                if ((int)tile.category == i)
                    category.Add(tile.tile);
            }
            categories.Add(category.ToArray());
        }
        for (int i = 0; i < categories.Count; i++)
        {
            tileCategories.Add(i, categories[i]);
        }
    }

    public void ToggleFillRoom (bool t)
    {
        if (openCategory == Category.Floors || builder.destroy)
            builder.fill = !builder.fill;
        else
            builder.drawLine = !builder.drawLine;   
    }

    public void OpenMenu (bool open)
    {
        btnBuildMenu.SetActive(!open);
        buildableTiles.SetActive(false);
        categories.SetActive(open);
        toggleFill.transform.parent.gameObject.SetActive(false);

        if (!open)
        {
            builder.SetBuild(null);
        }
    }

    public void OpenTiles (int i)
    {
        ShowCategory(i);
        buildableTiles.SetActive(true);
    }

    public void SetDestroy (bool destroy)
    {
        builder.SetDestroy(destroy);
        toggleFill.transform.parent.gameObject.SetActive(destroy);
        buildableTiles.SetActive(false);
        toggleFill.isOn = false;
        builder.fill = false;
    }

    public void ShowCategory (int i)
    {
        btnBuildMenu.SetActive(false);
        openCategory = (Category)(i + 1);
        toggleFill.transform.parent.gameObject.SetActive(true);
        toggleFill.GetComponentInChildren<TextMeshProUGUI>().text = openCategory == Category.Floors ? "Fill" : "Line";
        toggleFill.isOn = false;
        builder.fill = false;
        builder.drawLine = false;
        Tile[] tiles = new Tile[0];
        tileCategories.TryGetValue(i, out tiles);
        foreach (Transform t in buildableTiles.transform)
            Destroy(t.gameObject);
        if (tiles.Length > 0)
        {
            foreach (Tile tile in tiles)
            {
                GameObject btn = Instantiate(button, buildableTiles.transform);
                btn.name = "btn" + tile.name;
                btn.GetComponentInChildren<TextMeshProUGUI>().text = tile.name;
                btn.GetComponent<Button>().onClick.AddListener(() => builder.SetBuild(tile));
            }
        }
    }
}
