using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField]
    protected int health = 100;

    protected virtual void Start ()
    {
        SetAppearance();

        float rot = Random.Range(-180, 180);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
    }

    protected virtual void SetAppearance ()
    {
        throw new System.Exception("No appearance to set!");
    }
}
