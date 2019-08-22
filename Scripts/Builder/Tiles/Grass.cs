using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Tile
{
    int startSprite;

    protected override void Awake ()
    {
        base.Awake();
    }

    protected override void Start ()
    {
        base.Start();
        for (int i = 0; i < grid.GetSprites.Length; i++)
        {
            if (grid.GetSprites[i].name == sprite.GetComponent<SpriteRenderer>().sprite.name)
                startSprite = i;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = TileGrid.GetGrid.GetSprites[startSprite + 1];
    }
}
