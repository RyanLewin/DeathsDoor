using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ScaleWithCamera : MonoBehaviour
{
    PixelPerfectCamera cam;
    public Vector3 scale = Vector3.one;

    private void Awake ()
    {
        cam = Camera.main.GetComponent<PixelPerfectCamera>();
        cam.GetComponentInParent<CameraController>().scalables.Add(this);
        UpdateScale();
    }

    // Update is called once per frame
    public void UpdateScale()
    {
        Vector3 mult = scale / ((cam.assetsPPU / 10f) - 1);
        if (mult.x < 1)
            mult = Vector3.one;
        else if (mult.x > scale.x)
            mult = scale;
        transform.localScale = mult;
    }
}
