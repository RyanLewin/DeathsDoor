using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Node))]
public class Tile : MonoBehaviour
{
    protected TileGrid grid;
    public int x, y;
    ///<summary> Should it update on each tick. </summary>
    public bool tickUpdate = false;
    ///<summary> Is the tile built. </summary>
    public bool built = false;
    ///<summary> Determines whether the tile can be replaced. </summary>
    public bool replacable = false;
    public TileType tileType;
    public Category category;
    /// <summary> The tile that this tile was built upon. </summary>
    public TileType tileUnderneath;
    /// <summary> Can you walk on this tile. </summary>
    public bool walkable;
    [SerializeField]
    public bool IsWalkable { get; protected set; }

    /// <summary> The placeholder that shows where the tile will be built. </summary>
    [SerializeField]
    GameObject placeholder = default;
    public GameObject sprite { get; protected set; }
    /// <summary> If the tile is a wall then a border is placed around it to show its raised.  </summary>
    SpriteRenderer wallVisual;

    /// <summary> How long it takes to build this time. </summary>
    public float timeToBuild = 2;
    /// <summary> Amount of time spent building so far </summary>
    public float buildTime = 0;
    public Citizen assignedCitizen;

    public GameObject backgroundTile;
    /// <summary> Should the background tile be destroyed after being built </summary>
    public bool destroyBackgroundTile = true;

    public bool doesRequireItem = false;
    public List<Requirements> itemsRequired = new List<Requirements>();

    protected virtual void Awake ()
    {
        grid = TileGrid.GetGrid;
        if (category == Category.Walls)
        {
            walkable = false;
        }
        IsWalkable = true;
        sprite = transform.GetChild(0).gameObject;

        //Add surrounding effect to make the wall stand out
        if (category == Category.Walls)
        {
            GameObject go = new GameObject("Wall");
            go.transform.position = transform.position;
            go.transform.SetParent(sprite.transform);
            go.AddComponent<SpriteRenderer>();
            wallVisual = go.GetComponent<SpriteRenderer>();
            wallVisual.sortingOrder = sprite.GetComponent<SpriteRenderer>().sortingOrder + 1;
            wallVisual.color = AverageColorFromTexture(sprite.GetComponent<SpriteRenderer>().sprite);
        }
        else if (category == Category.Foliage)
        {
            placeholder.GetComponent<SpriteRenderer>().sprite = grid.GetSpriteOfTile(tileUnderneath);
        }
    }

    protected virtual void Start ()
    {
        Built = built;
        if (!built)
        {
            backgroundTile = new GameObject("BackgroundTile");
            backgroundTile.transform.position = transform.position;
            backgroundTile.transform.SetParent(transform);
            //backgroundTile = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
            backgroundTile.AddComponent<SpriteRenderer>();
            backgroundTile.GetComponent<SpriteRenderer>().sprite = grid.GetSpriteOfTile(tileUnderneath);
            backgroundTile.GetComponent<SpriteRenderer>().sortingOrder = placeholder.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }

    public virtual void TickUpdate ()
    {

    }

    public virtual void DayUpdate ()
    {

    }
    
    Color AverageColorFromTexture (Sprite sprite)
    {
        Color texColors = sprite.texture.GetPixel((int)sprite.rect.center.x, (int)sprite.rect.center.y);

        //int total = texColors.Length;

        //float r = 0;
        //float g = 0;
        //float b = 0;

        //for (int i = 0; i < total; i++)
        //{
        //    r += texColors[i].r;
        //    g += texColors[i].g;
        //    b += texColors[i].b;
        //}

        Color colour = texColors;

        colour *= .4f;
        colour.a = 1;

        return colour/* new Color((byte)(r / total), (byte)(g / total), (byte)(b / total), 255)*/;
    }

    public virtual void UpdateWall ()
    {
        Tile left = grid.GetTileAtPos(new Vector2(x - 1, y));
        Tile right = grid.GetTileAtPos(new Vector2(x + 1, y));
        Tile up = grid.GetTileAtPos(new Vector2(x, y + 1));
        Tile down = grid.GetTileAtPos(new Vector2(x, y - 1));

        Tile leftUp = grid.GetTileAtPos(new Vector2(x - 1, y + 1));
        Tile rightUp = grid.GetTileAtPos(new Vector2(x + 1, y + 1));
        Tile rightDown = grid.GetTileAtPos(new Vector2(x + 1, y - 1));
        Tile leftDown = grid.GetTileAtPos(new Vector2(x - 1, y - 1));

        bool l = left?.category == Category.Walls;
        bool r = right?.category == Category.Walls;
        bool u = up?.category == Category.Walls;
        bool d = down?.category == Category.Walls;
        bool lu = leftUp?.category == Category.Walls;
        bool ru = rightUp?.category == Category.Walls;
        bool rd = rightDown?.category == Category.Walls;
        bool ld = leftDown?.category == Category.Walls;
        wallVisual.flipX = false;

        AdjacentWall(true);

        up?.AdjacentWall(u);
        down?.AdjacentWall(d);
        left?.AdjacentWall(l);
        right?.AdjacentWall(r);
        leftUp?.AdjacentWall(lu);
        rightUp?.AdjacentWall(ru);
        leftDown?.AdjacentWall(ld);
        rightDown?.AdjacentWall(rd);

    }

    protected void AdjacentWall (bool necessary)
    {
        if (!necessary)
            return;

        Tile left = grid.GetTileAtPos(new Vector2(x - 1, y));
        Tile right = grid.GetTileAtPos(new Vector2(x + 1, y));
        Tile up = grid.GetTileAtPos(new Vector2(x, y + 1));
        Tile down = grid.GetTileAtPos(new Vector2(x, y - 1));

        Tile leftUp = grid.GetTileAtPos(new Vector2(x - 1, y + 1));
        Tile rightUp = grid.GetTileAtPos(new Vector2(x + 1, y + 1));
        Tile rightDown = grid.GetTileAtPos(new Vector2(x + 1, y - 1));
        Tile leftDown = grid.GetTileAtPos(new Vector2(x - 1, y - 1));

        bool l = left?.category == Category.Walls;
        bool r = right?.category == Category.Walls;
        bool u = up?.category == Category.Walls;
        bool d = down?.category == Category.Walls;
        bool lu = leftUp?.category == Category.Walls;
        bool ru = rightUp?.category == Category.Walls;
        bool rd = rightDown?.category == Category.Walls;
        bool ld = leftDown?.category == Category.Walls;
        wallVisual.flipX = false;

        if (l)
        {
            if (r)
            {
                if (u)
                {
                    if (d)
                    {
                        if (lu)
                        {
                            if (ru)
                            {
                                if (rd)
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = null;
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[7];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                                    }
                                }
                                else
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[7];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[10];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                                    }
                                }
                            }
                            else
                            {
                                if (rd)
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[7];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[11];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                                    }
                                }
                                else
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[10];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[0];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ru)
                            {
                                if (rd)
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[7];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[10];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                                    }
                                }
                                else
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[11];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[0];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                                    }
                                }
                            }
                            else
                            {
                                if (rd)
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[10];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[0];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                }
                                else
                                {
                                    if (ld)
                                    {
                                        wallVisual.sprite = grid.wallSprites[0];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                                    }
                                    else
                                    {
                                        wallVisual.sprite = grid.wallSprites[3];
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                }
                            }
                        }
                        //wallVisual.sprite = grid.wallSprites[0];
                    }
                    else
                    {
                        if (lu)
                        {
                            if (ru)
                            {
                                wallVisual.sprite = grid.wallSprites[5];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                        else
                        {
                            if (ru)
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                                wallVisual.flipX = true;
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[9];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    if (d)
                    {
                        if (ld)
                        {
                            if (rd)
                            {
                                wallVisual.sprite = grid.wallSprites[5];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                                wallVisual.flipX = true;
                            }
                        }
                        else
                        {
                            if (rd)
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[9];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                            }
                        }
                    }
                    else
                    {
                        wallVisual.sprite = grid.wallSprites[6];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
            else
            {
                if (u)
                {
                    if (d)
                    {
                        if (lu)
                        {
                            if (ld)
                            {
                                wallVisual.sprite = grid.wallSprites[5];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                                wallVisual.flipX = true;
                            }
                        }
                        else
                        {
                            if (ld)
                            {
                                wallVisual.sprite = grid.wallSprites[12];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                            }
                            else
                            {
                                wallVisual.sprite = grid.wallSprites[9];
                                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                            }
                        }
                    }
                    else
                    {
                        if (lu)
                        {
                            wallVisual.sprite = grid.wallSprites[2];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        else
                        {
                            wallVisual.sprite = grid.wallSprites[1];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                        }
                    }
                }
                else
                {
                    if (d)
                    {
                        if (ld)
                        {
                            wallVisual.sprite = grid.wallSprites[2];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                        }
                        else
                        {
                            wallVisual.sprite = grid.wallSprites[1];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                    }
                    else
                    {
                        wallVisual.sprite = grid.wallSprites[4];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                }
            }
        }
        else if (r)
        {
            if (u)
            {
                if (d)
                {
                    if (ru)
                    {
                        if (rd)
                        {
                            wallVisual.sprite = grid.wallSprites[5];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                        }
                        else
                        {
                            wallVisual.sprite = grid.wallSprites[12];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                        }
                    }
                    else
                    {
                        if (rd)
                        {
                            wallVisual.sprite = grid.wallSprites[12];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                            wallVisual.flipX = true;
                        }
                        else
                        {
                            wallVisual.sprite = grid.wallSprites[9];
                            wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                        }
                    }
                }
                else
                {
                    if (ru)
                    {
                        wallVisual.sprite = grid.wallSprites[2];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else
                    {
                        wallVisual.sprite = grid.wallSprites[1];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
            else
            {
                if (d)
                {
                    if (rd)
                    {
                        wallVisual.sprite = grid.wallSprites[2];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        wallVisual.sprite = grid.wallSprites[1];
                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                }
                else
                {
                    wallVisual.sprite = grid.wallSprites[4];
                    wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }
        }
        else if (u)
        {
            if (d)
            {
                wallVisual.sprite = grid.wallSprites[6];
                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                wallVisual.sprite = grid.wallSprites[4];
                wallVisual.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
        else if (d)
        {
            wallVisual.sprite = grid.wallSprites[4];
            wallVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            wallVisual.sprite = grid.wallSprites[8];
        }
    }

    public bool Built
    {
        get { return built; }
        set
        {
            built = value;
            if (placeholder)
            {
                placeholder.SetActive(!built);
                sprite.SetActive(built);
            }
            if (built)
            {
                OnBuilt();
            }
        }
    }

    protected virtual void OnBuilt ()
    {
        if (backgroundTile)
        {
            if (destroyBackgroundTile)
                Destroy(backgroundTile);
        }

        if (tickUpdate)
            grid.tickTiles.Add(this);

        if (category == Category.Walls)
            UpdateWall();
        
        GetComponent<Node>().walkable = walkable;

        if (GetComponent<Inventory>())
        {
            WorldController.GetWorldController.inventoriesInScene.Add(GetComponent<Inventory>());
        }
    }

    private void OnDestroy ()
    {
        if (doesRequireItem && Built)
        {
            foreach (Requirements i in itemsRequired)
            {
                Item item = Instantiate(i.item, transform.position, transform.rotation);
                item.count = i.count;
                item.SetToBeLooted = true;
            }
        }

        if (category != Category.Walls)
            return;

        Tile left = grid.GetTileAtPos(new Vector2(x - 1, y));
        Tile right = grid.GetTileAtPos(new Vector2(x + 1, y));
        Tile up = grid.GetTileAtPos(new Vector2(x, y + 1));
        Tile down = grid.GetTileAtPos(new Vector2(x, y - 1));

        Tile leftUp = grid.GetTileAtPos(new Vector2(x - 1, y + 1));
        Tile rightUp = grid.GetTileAtPos(new Vector2(x + 1, y + 1));
        Tile rightDown = grid.GetTileAtPos(new Vector2(x + 1, y - 1));
        Tile leftDown = grid.GetTileAtPos(new Vector2(x - 1, y - 1));

        bool l = left?.category == Category.Walls;
        bool r = right?.category == Category.Walls;
        bool u = up?.category == Category.Walls;
        bool d = down?.category == Category.Walls;
        bool lu = leftUp?.category == Category.Walls;
        bool ru = rightUp?.category == Category.Walls;
        bool rd = rightDown?.category == Category.Walls;
        bool ld = leftDown?.category == Category.Walls;

        up?.AdjacentWall(u);
        down?.AdjacentWall(d);
        left?.AdjacentWall(l);
        right?.AdjacentWall(r);
        leftUp?.AdjacentWall(lu);
        rightUp?.AdjacentWall(ru);
        leftDown?.AdjacentWall(ld);
        rightDown?.AdjacentWall(rd);
    }
}

[System.Serializable]
public class Requirements
{
    public Item item;
    public int count;
}
