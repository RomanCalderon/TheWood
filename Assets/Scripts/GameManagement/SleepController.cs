using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepController : MonoBehaviour
{
    public static SleepController Instance;

    public delegate void SleepHandler();
    public static event SleepHandler OnGoToSleep;
    public static event SleepHandler OnWakeUp;

    private const int SLEEP_TIME_RATE = 60;

    [HideInInspector] public int HourSleptAt;
    [HideInInspector] public int HoursSlept;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Sleep(int hours)
    {
        if (hours < 1)
            return;

        hours = Mathf.Clamp(hours, 0, 23);

        HoursSlept = hours;
        HourSleptAt = DayNightCycle.instance.GetCurrentHour();
        OnGoToSleep?.Invoke();
        StartCoroutine(SleepEnum(hours));
        StartCoroutine(SaveGameEnum());
    }

    private IEnumerator SleepEnum(int hours)
    {
        int targetHour = (DayNightCycle.instance.GetCurrentHour() + hours) % 24;

        DayNightCycle.instance.SetRate(SLEEP_TIME_RATE);

        // Wait until it's the correct hour to "wake up"
        while (DayNightCycle.instance.GetCurrentHour() != targetHour)
        {
            yield return null;
        }

        DayNightCycle.instance.SetRate(1);
        OnWakeUp?.Invoke();
        Bed.GetOutOfBed();
    }

    private IEnumerator SaveGameEnum()
    {
        yield return new WaitForSeconds(0.2f);

        // Invoke the SaveGame event
        SaveLoadController.SaveGame();
    }
}
