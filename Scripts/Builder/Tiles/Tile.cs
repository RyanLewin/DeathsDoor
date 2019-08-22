using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Node))]
public class Tile : MonoBehaviour
{
    protected TileGrid grid;
    public int x, y;
    public bool built = false;
    public bool replacable = false;
    public TileType tileType;
    public Category category;
    public TileType tileUnderneath;
    public bool walkable;
    [SerializeField]
    public bool IsWalkable { get; protected set; }

    [SerializeField]
    GameObject placeholder = default;
    public GameObject sprite { get; protected set; }
    SpriteRenderer wallVisual;

    public float timeToBuild = 2;
    public float buildTime = 0;
    public Citizen assignedCitizen;
    public Task assignedTask;

    public GameObject backgroundTile;

    protected virtual void Awake ()
    {
        grid = TileGrid.GetGrid;
        if (category == Category.Walls)
        {
            walkable = false;
        }
        IsWalkable = true;
    }

    protected virtual void Start ()
    {
        placeholder.SetActive(!built);
        sprite = transform.GetChild(0).gameObject;
        sprite.SetActive(built);
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
                                        wallVisual.transform.rotation = Quaternion.Euler(0, 0, 90);
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
            placeholder.SetActive(!built);
            GetComponent<Node>().walkable = walkable;
            sprite.SetActive(built);
            if (built)
            {
                OnBuilt();
            }
        }
    }

    protected virtual void OnBuilt ()
    {
        if (backgroundTile)
            Destroy(backgroundTile);

        if (category == Category.Walls)
            UpdateWall();
    }

    private void OnDestroy ()
    {
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
