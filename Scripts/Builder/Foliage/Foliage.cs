using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foliage : Tile
{
    public float age = .2f;
    [SerializeField]
    float growthSpeed = .2f;
    [SerializeField]
    Item wood;
    int woodCount = 2;
    [SerializeField]
    int woodCountMax = 10;
    [SerializeField]
    int woodCountMin = 2;
    public float timeToChop { get; private set; } = 3;

    protected override void Awake ()
    {
        base.Awake();
        timeToBuild = 0.05f;
    }

    protected override void Start ()
    {
        base.Start();
        wood = ItemDictionary.instance.GetItemByName("Wood");
    }

    protected override void OnBuilt ()
    {
        base.OnBuilt();
        Plant();
    }

    public void Plant ()
    {
        woodCount = (int)(age * 10);
        transform.localScale = new Vector3(age, age, 1);
    }

    public override void DayUpdate ()
    {
        base.DayUpdate();
        age += growthSpeed;
        if (age > 1)
        {
            age = 1;
        }
        woodCount = (int)(age * 10);
        transform.localScale = new Vector3(age, age, 1);
    }

    public void ChopDown ()
    {
        Vector3 pos = transform.position;
        pos.z = -0.01f;
        Item dropWood = Instantiate(wood, pos, transform.rotation);
        dropWood.SetToBeLooted = true;
        dropWood.count = woodCount;
        TileGrid.GetGrid.ResetTile(this);
        //Destroy(gameObject);
    }
}
