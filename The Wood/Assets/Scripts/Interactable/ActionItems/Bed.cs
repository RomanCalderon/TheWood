using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    [SerializeField] Camera bedCamera;


    protected override void Awake()
    {
        base.Awake();

        SleepController.OnGoToSleep += SleepController_OnGoToSleep;
        SleepController.OnWakeUp += SleepController_OnWakeUp;
    }

    public override void Interact()
    {
        base.Interact();

        // Set up a UI so the player can choose how many hours to sleep
        SleepController.Instance.Sleep(8);
    }

    private void SleepController_OnGoToSleep()
    {
        if (!HasInteracted)
            return;
        
        bedCamera.enabled = true;
    }

    private void SleepController_OnWakeUp()
    {
        if (!HasInteracted)
            return;

        bedCamera.enabled = false;
        HasInteracted = false;
    }
}
