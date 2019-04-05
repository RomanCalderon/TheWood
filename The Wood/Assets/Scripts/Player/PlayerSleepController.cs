using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSleepController : MonoBehaviour
{
    public static bool IsSleeping { get; private set; }

    [SerializeField] CanvasGroup sleepCanvasGroup;

    private void Awake()
    {
        SleepController.OnGoToSleep += SleepController_OnGoToSleep;
        SleepController.OnWakeUp += SleepController_OnWakeUp;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the sleep canvas group
        sleepCanvasGroup.alpha = Mathf.Lerp(sleepCanvasGroup.alpha, (IsSleeping) ? 1f : 0f, Time.deltaTime * 4f);
        // Update the global audio volume
        AudioListener.volume = Mathf.Lerp(AudioListener.volume, (IsSleeping) ? 0f : 1f, Time.deltaTime * 2f);
    }

    // Event listeners
    private void SleepController_OnGoToSleep()
    {
        StartCoroutine(SleepFadeDelay());
    }

    private void SleepController_OnWakeUp()
    {
        IsSleeping = false;
        print("You woke up.");
    }

    IEnumerator SleepFadeDelay()
    {
        yield return new WaitForSeconds(1f);

        IsSleeping = true;
        print("You fell asleep...");
    }
}
