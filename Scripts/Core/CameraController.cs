using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    public float speed = 5;
    public float xMin, xMax, yMin, yMax;
    public float camWidth, camHeight;
    public int camMinZoom = 50, camMaxZoom = 10;
    KeyBindings keyBindings;
    PixelPerfectCamera cam;
    public List<ScaleWithCamera> scalables = new List<ScaleWithCamera>(); 

    void Awake ()
    {
        keyBindings = KeyBindings.GetKeyBindings;
        ResetCamSize();
        cam = Camera.main.GetComponent<PixelPerfectCamera>();
        speed = 5f / cam.assetsPPU;
    }

    void ResetCamSize ()
    {
        camHeight = 2 * Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
        camHeight /= 2;
        camWidth /= 2;
        foreach (ScaleWithCamera s in scalables)
        {
            s.UpdateScale();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Menus.GetMenus.IsOpen)
        {
            return;
        }


        //CustomBinding left = keyBindings.GetKey(BindingsNames.moveLeft);
        if (keyBindings.GetKey(BindingsNames.moveLeft).AnyInput)
        {
            transform.position = transform.position + new Vector3(-speed, 0, 0);
        }
        //if (Input.GetKey(keyBindings.moveLeft) || Input.GetKey(keyBindings.altMoveLeft))
        //{
        //    transform.position = transform.position + new Vector3(-speed, 0, 0);
        //}
        if (keyBindings.GetKey(BindingsNames.moveRight).AnyInput)
        {
            transform.position = transform.position + new Vector3(speed, 0, 0);
        }
        if (keyBindings.GetKey(BindingsNames.moveUp).AnyInput)
        {
            transform.position = transform.position + new Vector3(0, speed, 0);
        }
        if (keyBindings.GetKey(BindingsNames.moveDown).AnyInput)
        {
            transform.position = transform.position + new Vector3(0, -speed, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                cam.assetsPPU += 1;
            else
                cam.assetsPPU -= 1;
            //cam.assetsPPU -= Mathf.CeilToInt(Input.GetAxis("Mouse ScrollWheel"));
            if (cam.assetsPPU > camMaxZoom)
                cam.assetsPPU = camMaxZoom;
            if (cam.assetsPPU < camMinZoom)
                cam.assetsPPU = camMinZoom;
            speed = 5f / cam.assetsPPU;

            //Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
            //if (Camera.main.orthographicSize < 1)
            //    Camera.main.orthographicSize = 1;
            //if (Camera.main.orthographicSize > 10)
            //    Camera.main.orthographicSize = 10;
            ResetCamSize();
        }

        if (transform.position.x - camWidth < xMin)
        {
            transform.position = new Vector3(xMin + camWidth, transform.position.y, 0);
        }
        if (transform.position.x + camWidth > xMax)
        {
            transform.position = new Vector3(xMax - camWidth, transform.position.y, 0);
        }
        if (transform.position.y - camHeight < yMin)
        {
            transform.position = new Vector3(transform.position.x, yMin + camHeight, 0);
        }
        if (transform.position.y + camHeight > yMax)
        {
            transform.position = new Vector3(transform.position.x, yMax - camHeight, 0);
        }
    }
}
