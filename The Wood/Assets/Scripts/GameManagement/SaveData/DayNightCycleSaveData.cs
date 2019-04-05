using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayNightCycleSaveData
{
    public float hour;

    public DayNightCycleSaveData(DayNightCycle dayNightCycle)
    {
        hour = (dayNightCycle.GetCurrentHour() + SleepController.Instance.HoursSlept) % 24;
    }
}
