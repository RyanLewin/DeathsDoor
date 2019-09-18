using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;
    public static TileGrid GetGrid { get; private set; }
    WorldController worldController;
    public string seed = "00000000";
    public int xSize { get; private set; } = 100;
    public int ySize { get; private set; } = 100;
    Tile[,] grid;
    public List<Tile> tickTiles = new List<Tile>();
    [SerializeField]
    Tile emptyTile = default;
    Tile inactiveTile;

    
    int treeChance = 2; //out of 100
   
    [SerializeField]
    Texture2D spritesheet = default;
    public Sprite[] GetSprites { get; private set; }
    public Sprite[] wallSprites;

    //Dictionary<TileType, Tile> tiles = new Dictionary<TileType, Tile>();
    public AllTiles[] tiles;

    [System.Serializable]
    public struct AllTiles
    {
        public TileType type;
        public Category category;
        public Sprite sprite;
        public Tile tile;
    }

    private void Awake ()
    {
#if UNITY_EDITOR
        //seed = "0";
        seed = "25252490";
#endif
        xSize = TextureCreator.GetMap.resolution;
        ySize = TextureCreator.GetMap.resolution;
        GetSprites = Resources.LoadAll<Sprite>("Art/Buildings/Structures/" + spritesheet.name);
        GetGrid = this;
        //foreach (AllTiles t in tiles)
        //{
        //    t.tile.category = t.category;
        //    t.tile.tileType = t.type;
        //}
        GenerateGrid();
    }

    private void Start ()
    {
        worldController = WorldController.GetWorldController;
    }

    public void ResetTile (Tile t)
    {
        SetTile(GetTileOfType(t.tileUnderneath), t.x, t.y, true);
        worldController.GetTaskAtTile(t.transform.position);
    }

    public void SetTempTile (Tile newTempTile, ref GameObject instance, int x, int y)
    {
        Tile currTile = GetTileAtPos(new Vector2(x, y));
        if (currTile == null || !currTile.replacable)
        {
            Destroy(instance);
            ClearTempTile();
            return;
        }

        if (inactiveTile != currTile && inactiveTile != null)
        {
            inactiveTile.gameObject.SetActive(true);
        }
        currTile.gameObject.SetActive(false);
        inactiveTile = currTile;

        if (instance == null)
        {
            instance = Instantiate(newTempTile.gameObject, new Vector3(x, y, 0), newTempTile.transform.rotation);
            instance.GetComponent<Tile>().enabled = false;
            SpriteRenderer sprite = instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Color c = sprite.color;
            c.a = .5f;
            sprite.color = c;
            instance.transform.GetChild(0).gameObject.SetActive(true);
            instance.transform.GetChild(1).gameObject.SetActive(false);
        }
        instance.transform.position = new Vector3(x, y, 0);
    }

    public void ClearTempTile ()
    {
        if (inactiveTile != null)
        {
            inactiveTile.gameObject.SetActive(true);
            inactiveTile = null;
        }
    }

    public void SetTile (Tile newTile, int x, int y, bool groundTile = false)
    {
        Tile currTile = GetTileAtPos(new Vector2(x, y));
        if (currTile == null || (!currTile.replacable && !groundTile) || !currTile.Built)
        {
            return;
        }

        //Check if the build requires items and if these items are collected
        if (newTile.doesRequireItem)
        {
            foreach (Requirements items in newTile.itemsRequired)
            {
                Inventory inv = worldController.CheckInventories(items.item, items.count);
                if (inv)
                {
                    //remove items from inventory
                    Item item = inv.TakeItem(items.item, items.count);
                    Destroy(item.gameObject);
                }
                else
                {
                    //resources aren't available so stop builder
                    Builder.instance.SetBuild(null);
                    return;
                }
            }
        }

        Tile tile = Instantiate(newTile, new Vector3(x, y, 0), newTile.transform.rotation);
        tile.name = tile.name + " " + x + ":" + y;
        tile.transform.SetParent(transform);
        tile.x = x;
        tile.y = y;
        tile.built = groundTile;
        if (!groundTile)
        {
            tile.tileUnderneath = grid[x, y].tileType;
        }
        Destroy(grid[x, y].gameObject);
        grid[x, y] = tile;
        
        Node node = grid[x, y].GetComponent<Node>();
        node.x = x;
        node.y = y;
        node.walkable = true;
    }

    public void DestroyGrid ()
    {
        foreach (Tile tile in grid)
        {
            Destroy(tile.gameObject);
        }
        grid = null;
    }

    public void GenerateGrid ()
    {
        grid = new Tile[xSize, ySize];
        TextureCreator map = TextureCreator.GetMap;

        if (int.Parse(seed) == 0)
        {
            int xVal = Random.Range(1, 3) * 1000;
            int yVal = Random.Range(1, 3) * 1000;
            xVal += Random.Range(0, 1000);
            yVal += Random.Range(0, 1000);
            seed = xVal.ToString() + yVal.ToString();
        }
        seed.Substring(1, 3);
        float xPos = float.Parse(seed.Substring(1, 3)) / 10;
        float yPos = float.Parse(seed.Substring(5, 3)) / 10;
        if (seed[0] == '2')
            xPos *= -1;
        if (seed[4] == '2')
            yPos *= -1;
        map.transform.position = new Vector3(xPos, yPos, -1f);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Color color = map.GetTexture(new Vector2(x, y));

                //sand
                if (color.Equals(Color.white))
                    emptyTile = GetTileOfType(TileType.Sand);
                //deep water
                else if (color.Equals(Color.blue))
                    emptyTile = GetTileOfType(TileType.DeepWater);
                //grass
                else if (color.Equals(Color.green))
                {
                    emptyTile = GetTileOfType(TileType.Grass);
                    //spawn tree
                    int rand = Random.Range(0, 100);
                    if (rand < treeChance)
                    {
                        bool skip = false;
                        if (x > 0 && y > 0)
                        {
                            if (grid[x - 1, y - 1].GetComponent<Foliage>())
                                skip = true;
                        }
                        if (x > 0)
                        {
                            if (grid[x - 1, y].GetComponent<Foliage>())
                                skip = true;
                        }
                        if (x > 0 && y < ySize - 1)
                        {
                            if (grid[x - 1, y + 1].GetComponent<Foliage>())
                                skip = true;
                        }
                        if (y > 0)
                        {
                            if (grid[x, y - 1].GetComponent<Foliage>())
                                skip = true;
                        }

                        if (!skip)
                        {
                            emptyTile = GetTileOfType(TileType.Tree);
                            emptyTile.tileUnderneath = TileType.Grass;
                            emptyTile.GetComponent<Foliage>().age = 1;
                        }
                    }
                }
                //stone walls - can be used for caves
                else if (color.Equals(Color.black))
                    emptyTile = GetTileOfType(TileType.Stone, Category.Walls);
                //water
                else if (color.Equals(Color.cyan))
                    emptyTile = GetTileOfType(TileType.Water);

                Tile t = Instantiate(emptyTile, new Vector3(x, y, 0), emptyTile.transform.rotation);
                t.built = true;
                t.name = t.name + " " + x + ":" + y;
                t.transform.SetParent(transform);
                t.x = x;
                t.y = y;
                grid[x, y] = t;

                Node node = grid[x, y].GetComponent<Node>();
                node.x = x;
                node.y = y;
                node.walkable = true;
            }
        }
    }

    Vector3 ColorCompare (Color a, Color b)
    {
        Vector3 compare = new Vector3(a.r, a.g, a.b);
        compare.x -= b.r;
        compare.y -= b.g;
        compare.z -= b.b;
        return compare;
    }

    public List<Node> GetNeighbours (Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                if (CheckIfBlocked(node, x, y))
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize)
                {
                    neighbours.Add(grid[checkX, checkY].GetComponent<Node>());
                }
            }
        }

        return neighbours;
    }

    bool CheckIfBlocked (Node node, int x, int y)
    {
        if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
        {
            int newX = node.x + x;
            int newY = node.y + y;
            if (newX < 0 || newX >= xSize) return true;
            if (newY < 0 || newY >= ySize) return true;
            if (GetTileAtPos(new Vector2(node.x + x, node.y)).walkable == false ||
                GetTileAtPos(new Vector2(node.x, node.y + y)).walkable == false)
                return true;
        }
        return false;
    }

    public Sprite GetSpriteOfTile (TileType type)
    {
        foreach (AllTiles tile in tiles)
        {
            if (tile.type == type)
                return tile.sprite;
        }
        return GetSpriteOfTile(TileType.Grass);
    }

    public Tile GetTileAtPos (Vector2 pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= xSize || pos.y >= ySize)
            return null;
        return grid[pos.x == 0 ? 0 : Mathf.RoundToInt(pos.x), pos.y == 0 ? 0 : Mathf.RoundToInt(pos.y)];
    }

    public Vector2 GetPos (Vector3 pos)
    {
        Vector2 gridPos;
        gridPos.x = Mathf.RoundToInt(pos.x);
        gridPos.y = Mathf.RoundToInt(pos.y);
        return gridPos;
    }

    public Tile GetTileOfType (TileType type, Category category = Category.WorldGen)
    {
        foreach (AllTiles tile in tiles)
        {
            if (tile.type == type)
            {
                if (tile.category == category || category == Category.WorldGen)
                    return tile.tile;
            }
        }
        return GetTileOfType(TileType.Grass);
    }
}

public enum Category { WorldGen, Walls, Floors, Misc, Foliage }

public enum TileType
{
    Wood, Stone, Bricks, Grass, Dirt, Sand, Water, DeepWater, Inventory, Tree
}
