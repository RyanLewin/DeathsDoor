using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule : MonoBehaviour
{
    public List<Day> days { get; private set; } = new List<Day>( 3 );

    public void InstantiateSchedule ()
    {
        days = new List<Day>();
        for (int i = 0; i < 7; i++)
        {
            days.Add(new Day(new Citizen[3], new Tile[3]));
        }
    }

    public void SetSchedule (int day, int slot, int value)
    {
        List<Citizen> citizens = WorldController.GetWorldController.citizensList;
        foreach (Citizen citizen in WorldController.GetWorldController.citizensList)
        {
            if (!citizen.alive)
                citizens.Remove(citizen);
        }
        Citizen c = citizens[value];
        days[day].citizenPeriods[slot] = c;
        c.GetComponent<Schedule>().days[day].tilePeriods[slot] = GetComponent<Tile>();
    }

    public void SetSchedule (int day, int slot, Tile t)
    {
        t.GetComponent<Schedule>().days[day].citizenPeriods[slot] = null;
        days[day].tilePeriods[slot] = null;
        Menus.GetMenus.ScheduleDetails(this, true);
    }

}

public class Day
{
    public List<Citizen> citizenPeriods = new List<Citizen>( 3 );
    public List<Tile> tilePeriods = new List<Tile>( 3 );

    public Day (Citizen[] citizens, Tile[] tile)
    {
        citizenPeriods.AddRange(citizens);
        tilePeriods.AddRange(tile);
    }
}
