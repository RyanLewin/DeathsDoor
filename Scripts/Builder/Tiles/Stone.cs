using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Tile
{
    int startSprite;

    protected override void Awake ()
    {
        base.Awake();
        //IsWalkable = false;
    }

    protected override void Start ()
    {
        base.Start();
        for (int i = 0; i < grid.GetSprites.Length; i++)
        {
            if (grid.GetSprites[i].name == sprite.GetComponent<SpriteRenderer>().sprite.name)
                startSprite = i;
        }
        int rand = Random.Range(startSprite, startSprite + 2);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = TileGrid.GetGrid.GetSprites[rand];
    }
}
