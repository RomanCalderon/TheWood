using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class PlayerSleepController : MonoBehaviour
{
    public static bool IsInBed { get; private set; }
    private bool IsSleeping;

    int hoursToSleep = 1;

    [SerializeField] GameObject sleepSelectionUI;
    [SerializeField] Text wakeUpTimeText;
    [SerializeField] Button increaseHourButton;
    [SerializeField] Button decreaseHourButton;
    [SerializeField] Button sleepButton;
    [SerializeField] Button cancelSleepButton;
    [Space]
    [SerializeField] Text currentTimeText;
    [Space]
    [SerializeField] CanvasGroup sleepCanvasGroup;

    private void Awake()
    {
        Bed.OnInBed += Bed_OnInBed;
        Bed.OnOutOfBed += Bed_OnOutOfBed;

        SleepController.OnGoToSleep += SleepController_OnGoToSleep;
        SleepController.OnWakeUp += SleepController_OnWakeUp;

        increaseHourButton.onClick.AddListener(delegate { ChangeHoursToSleep(1); });
        decreaseHourButton.onClick.AddListener(delegate { ChangeHoursToSleep(-1); });
        sleepButton.onClick.AddListener(delegate { Sleep(); });
        cancelSleepButton.onClick.AddListener(delegate { CancelSleep(); });
    }

    private void OnDestroy()
    {
        Bed.OnInBed -= Bed_OnInBed;
        Bed.OnOutOfBed -= Bed_OnOutOfBed;

        SleepController.OnGoToSleep -= SleepController_OnGoToSleep;
        SleepController.OnWakeUp -= SleepController_OnWakeUp;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the sleep canvas group
        sleepCanvasGroup.alpha = Mathf.Lerp(sleepCanvasGroup.alpha, (IsSleeping) ? 1f : 0f, Time.deltaTime * 4f);
        // Update the global audio volume
        AudioListener.volume = Mathf.Lerp(AudioListener.volume, (IsSleeping) ? 0f : 1f, Time.deltaTime * 2f);

        // Display the current time as the player sleeps
        if (IsSleeping)
            currentTimeText.text = DayNightCycle.instance.GetStandardTime((int)DayNightCycle.instance.Hour);
    }

    #region Event Listeners

    /// <summary>
    /// Called when the player gets in bed
    /// </summary>
    private void Bed_OnInBed()
    {
        IsInBed = true;

        // Enable the cursor
        Player.SetCursorActive(true);

        // Display a UI for the player to pick how long to sleep
        // and an option to cancel the sleep (get out of bed)
        sleepSelectionUI.SetActive(true);

        // Reset hoursToSleep back to 1, the minimun number of hours to sleep
        hoursToSleep = 1;

        // Update the hoursToSleep text to proper wake up time
        wakeUpTimeText.text = DayNightCycle.instance.GetStandardTime((int)DayNightCycle.instance.Hour + hoursToSleep);

        // For canceling sleep, call: Bed.GetOutOfBed()
    }

    /// <summary>
    /// Called when player is done sleeping OR
    /// canceled the sleep.
    /// </summary>
    private void Bed_OnOutOfBed()
    {
        Player.SetCursorActive(false);
        sleepSelectionUI.SetActive(false);
        IsInBed = false;
    }

    #endregion

    #region UI onClick Listeners

    /// <summary>
    /// Called by UI Button.
    /// Increments/Decrements hoursToSleep
    /// </summary>
    /// <param name="amount">Amount of hours increased/decreased.</param>
    private void ChangeHoursToSleep(int amount)
    {
        hoursToSleep += amount;

        hoursToSleep = Mathf.Clamp(hoursToSleep, 1, 23);

        // Update the hoursToSleep text to proper wake up time
        wakeUpTimeText.text = DayNightCycle.instance.GetStandardTime((int)DayNightCycle.instance.Hour + hoursToSleep);
    }
    
    /// <summary>
    /// Called by UI Button.
    /// The player goes to sleep for hoursToSleep
    /// </summary>
    private void Sleep()
    {
        SleepController.Instance.Sleep(hoursToSleep);
        sleepSelectionUI.SetActive(false);
    }

    /// <summary>
    /// Called by UI Button.
    /// Gets the player out of bed
    /// </summary>
    private void CancelSleep()
    {
        Bed.GetOutOfBed();
    }

    #endregion

    private void SleepController_OnGoToSleep()
    {
        currentTimeText.gameObject.SetActive(true);

        IsSleeping = true;

        //print("You fell asleep...");
    }

    private void SleepController_OnWakeUp()
    {
        IsSleeping = false;

        currentTimeText.gameObject.SetActive(false);

        print("You woke up.");
    }
}
