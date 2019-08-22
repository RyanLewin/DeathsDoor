using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    WorldController worldController;
    public static Builder instance;
    TileGrid grid;
    public Tile[] walls;
    [SerializeField]
    Tile wood, stone, brick;
    Tile toBuild;
    public bool destroy { get; private set; }
    public bool fill = false;
    public bool drawLine = false;
    Vector2 lineStart = new Vector2(-1, 0);
    GameObject tempTile;

    private void Awake ()
    {
        instance = this;
        tempTile = null;
    }
    private void Start ()
    {
        grid = TileGrid.GetGrid;
        worldController = WorldController.GetWorldController;
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetBuild(null);
        else if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.bulldoze).KeyDown)
        {
            SetDestroy(!destroy);
        }

        if (toBuild)
        {
            Vector2 pos = grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (pos.x < 0 || pos.x >= grid.xSize) return;
            if (pos.y < 0 || pos.y >= grid.ySize) return;

            if (lineStart.x <= 0)
            {
                if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).AnyInput)
                {
                    if (Menus.GetMenus.IsOverUI())
                        return;

                    Destroy(tempTile);
                    grid.ClearTempTile();

                    if (fill)
                    {
                        inumertions = 0;
                        Tile old = grid.GetTileAtPos(pos);
                        Fill((int)pos.x, (int)pos.y, old);
                    }
                    else if (drawLine)
                    {
                        lineStart = pos;
                    }
                    else
                    {
                        SetTile((int)pos.x, (int)pos.y);
                    }
                    return;
                }
                grid.SetTempTile(toBuild, ref tempTile, (int)pos.x, (int)pos.y);
            }
            else
            {
                if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).KeyUp)
                {
                    SetLine(pos);
                    lineStart = new Vector2(-1, 0);
                }
            }
        }
        else if (destroy)
        {
            if (KeyBindings.GetKeyBindings.GetKey(BindingsNames.interact).AnyInput)
            {
                if (Menus.GetMenus.IsOverUI())
                    return;
                Vector2 pos = grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                Tile t = grid.GetTileAtPos(pos);
                if (t)
                {
                    if (!t.replacable)
                    {
                        if (fill)
                        {
                            FillDestroy(t.x, t.y, t);
                        }
                        else
                        {
                            grid.ResetTile(t);
                        }
                    }
                }
            }
        }
    }

    void FillDestroy (int x, int y, Tile old)
    {
        if ((x < 0) || (x >= grid.xSize)) return;
        if ((y < 0) || (y >= grid.ySize)) return;

        Tile currTile = grid.GetTileAtPos(new Vector2(x, y));
        if (currTile.tileType == old.tileType)
        {
            grid.ResetTile(currTile);

            FillDestroy(x + 1, y, old);
            FillDestroy(x, y + 1, old);
            FillDestroy(x - 1, y, old);
            FillDestroy(x, y - 1, old);
        }
    }

    int inumertions;
    void Fill (int x, int y, Tile old)
    {
        inumertions++;
        if (inumertions > 100)
            return;
        if ((x < 0) || (x >= grid.xSize)) return;
        if ((y < 0) || (y >= grid.ySize)) return;
      
        Tile currTile = grid.GetTileAtPos(new Vector2(x, y));
        if (currTile.tileType == old.tileType)
        {
            SetTile(x, y);

            Fill(x + 1, y, old);
            Fill(x, y + 1, old);
            Fill(x - 1, y, old);
            Fill(x, y - 1, old);
        }
    }

    void SetTile (int x, int y)
    {
        grid.SetTile(toBuild, x, y);
        Task task = new Task(TaskItems.Build, new Vector3(x, y, 0));
        worldController.AddTask(task);
        grid.GetTileAtPos(new Vector2(x, y)).assignedTask = task;
    }

    void SetLine (Vector2 lineEnd)
    {
        int diffX, diffY;
        diffX = (int)lineStart.x - (int)lineEnd.x;
        diffY = (int)lineStart.y - (int)lineEnd.y;
        bool dir = Mathf.Abs(diffX) > Mathf.Abs(diffY);
        int start, end;
        if ((dir && lineEnd.x > lineStart.x) || (!dir && lineEnd.y > lineStart.y))
        {
            start = dir ? (int)lineStart.x : (int)lineStart.y;
            end = dir ? (int)lineEnd.x : (int)lineEnd.y;
            for (int i = start; i <= end; i++)
            {
                SetTile(dir ? i : (int)lineStart.x, dir ? (int)lineStart.y : i);
            }
        }
        else
        {
            start = dir ? (int)lineEnd.x : (int)lineEnd.y;
            end = dir ? (int)lineStart.x : (int)lineStart.y;
            for (int i = end; i >= start; i--)
            {
                SetTile(dir ? i : (int)lineStart.x, dir ? (int)lineStart.y : i);
            }
        }
    }

    public void SetDestroy (bool _destroy)
    {
        worldController.SelectedCitizen = null;
        toBuild = null;
        destroy = _destroy;
    }

    public void SetBuild (Tile t)
    {
        Destroy(tempTile);
        grid.ClearTempTile();
        WorldController.GetWorldController.SelectedCitizen = null;
        toBuild = t;
        destroy = false;
    }
}
