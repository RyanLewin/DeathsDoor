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
        camHeight = 2 * Camera.main.orthographicSize - 1;
        camWidth = camHeight * Camera.main.aspect;
        camHeight /= 2;
        camWidth /= 2;
        foreach (ScaleWithCamera s in scalables)
        {
            s.UpdateScale();
        }
    }
    
    void Update()
    {
        if (Menus.GetMenus.IsOpen)
        {
            return;
        }
        
        if (keyBindings.GetKey(BindingsNames.moveLeft).AnyInput)
        {
            MoveCamera(new Vector3(-speed, 0, 0));
        }
        if (keyBindings.GetKey(BindingsNames.moveRight).AnyInput)
        {
            MoveCamera(new Vector3(speed, 0, 0));
        }
        if (keyBindings.GetKey(BindingsNames.moveUp).AnyInput)
        {
            MoveCamera(new Vector3(0, speed, 0));
        }
        if (keyBindings.GetKey(BindingsNames.moveDown).AnyInput)
        {
            MoveCamera(new Vector3(0, -speed, 0));
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                cam.assetsPPU += 1;
            else
                cam.assetsPPU -= 1;

            if (cam.assetsPPU > camMaxZoom)
                cam.assetsPPU = camMaxZoom;
            if (cam.assetsPPU < camMinZoom)
                cam.assetsPPU = camMinZoom;
            speed = 5f / cam.assetsPPU;

            ResetCamSize();
        }
    }

    private void LateUpdate ()
    {
        if (transform.position.x < xMin + camWidth)
        {
            transform.position = new Vector3(xMin + camWidth, transform.position.y, 0);
        }
        else if (transform.position.x > xMax - camWidth)
        {
            transform.position = new Vector3(xMax - camWidth, transform.position.y, 0);
        }
        if (transform.position.y < yMin + camHeight)
        {
            transform.position = new Vector3(transform.position.x, yMin + camHeight, 0);
        }
        else if (transform.position.y > yMax - camHeight)
        {
            transform.position = new Vector3(transform.position.x, yMax - camHeight, 0);
        }
    }

    void MoveCamera (Vector3 translate)
    {
        transform.position += translate;
        ResetCamSize();
    }
}
