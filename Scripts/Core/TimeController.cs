using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    WorldController worldController;
    float timeTick = 0;
    [SerializeField]
    float timeForDay = 600;

    private void Awake ()
    {
        worldController = WorldController.GetWorldController;
    }

    private void Update ()
    {
        timeTick += Time.deltaTime;

        foreach (Tile tile in TileGrid.GetGrid.tickTiles)
        {
            tile.TickUpdate();
        }

        if (timeTick >= timeForDay)
        {
            timeTick = 0;
            NewDay();
        }
    }

    void NewDay ()
    {
        int day = worldController.GetDay;
        int month = worldController.GetMonth;
        int year = worldController.GetYear;

        day += 1;

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.DayUpdate();
        }

        switch (month)
        {
            case (0):
            case (2):
            case (4):
            case (6):
            case (7):
            case (9):
            case (11):
                if (day > 30)
                {
                    IncrementDay(ref month, ref day, ref year);
                }
                break;
            case (1):
                //Leap Year
                if (year % 4 == 0)
                {
                    if (day > 28)
                    {
                        IncrementDay(ref month, ref day, ref year);
                    }
                }
                //not leap year
                else
                {
                    if (day > 27)
                    {
                        IncrementDay(ref month, ref day, ref year);
                    }
                }
                break;
            default:
                if (day > 29)
                {
                    IncrementDay(ref month, ref day, ref year);
                }
                break;
        }

        worldController.GetDay = day;
        worldController.GetMonth = month;
        worldController.GetYear = year;
    }

    void IncrementDay (ref int month, ref int day, ref int year)
    {
        month += 1;
        day = 0;
        if (month > 11)
        {
            year += 1;
            month = 0;
        }
    }
}
