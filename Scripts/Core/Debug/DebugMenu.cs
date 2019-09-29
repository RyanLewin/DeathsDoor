using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
public class DebugMenu : MonoBehaviour
{
    bool IsActive = false;

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F2) && Input.GetKey(KeyCode.LeftShift))
        {
            IsActive = !IsActive;
        }
    }

    private void OnGUI ()
    {
        if (!IsActive)
            return;

        GUILayout.Window(442, new Rect(100, 100, 100, 100), DoMyWindow, "Debug Menu");
    }

    void DoMyWindow (int windowID)
    {
        if (GUILayout.Button("Add Wood"))
        {
            WorldController.GetWorldController.woodCount += 100;
        }
        if (GUILayout.Button("Add Stone"))
        {
            WorldController.GetWorldController.stoneCount += 100;
        }
        if (GUILayout.Button("Add Stone Bricks"))
        {
            WorldController.GetWorldController.stoneBricksCount += 100;
        }
        if (GUILayout.Button("Spawn Citizen"))
        {
            WorldController.GetWorldController.SpawnCitizen(Camera.main.transform.position);
        }
        if (GUILayout.Button("New World"))
        {
            TileGrid tileGrid = TileGrid.GetGrid;
            tileGrid.seed = "0";
            tileGrid.DestroyGrid();
            tileGrid.GenerateGrid();
        }
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}
#endif