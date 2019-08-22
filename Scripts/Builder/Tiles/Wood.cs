using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Tile
{
    int startSprite;

    protected override void OnBuilt ()
    {
        base.OnBuilt();
        UpdateSprite();
    }

    public void UpdateSprite ()
    {
        for (int i = 0; i < grid.GetSprites.Length; i++)
        {
            if (grid.GetSprites[i].name == grid.GetSpriteOfTile(tileType).name)
                startSprite = i;
        }
        Tile left = grid.GetTileAtPos(new Vector2(x - 1, y));
        Tile right = grid.GetTileAtPos(new Vector2(x + 1, y));
        if (left?.tileType == tileType)
        {
            if (right?.tileType == tileType)
            {
                left.GetComponent<Wood>().AdjacentSprite();
                sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 2];
                right.GetComponent<Wood>().AdjacentSprite();
            }
            else
            {
                left.GetComponent<Wood>().AdjacentSprite();
                sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 3];
            }
        }
        else if (right?.tileType == tileType)
        {
            right.GetComponent<Wood>().AdjacentSprite();
            sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite];
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 1];
        }
    }

    protected void AdjacentSprite ()
    {
        for (int i = 0; i < grid.GetSprites.Length; i++)
        {
            if (grid.GetSprites[i].name == grid.GetSpriteOfTile(tileType).name)
                startSprite = i;
        }

        Tile left = grid.GetTileAtPos(new Vector2(x - 1, y));
        Tile right = grid.GetTileAtPos(new Vector2(x + 1, y));
        if (left?.tileType == tileType)
        {
            if (right?.tileType == tileType)
            {
                sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 2];
            }
            else
            {
                sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 3];
            }
        }
        else if (right?.tileType == tileType)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite];
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().sprite = grid.GetSprites[startSprite + 1];
        }
    }
}
