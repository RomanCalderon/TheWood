using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    public delegate void BedHandler();
    public static event BedHandler OnInBed;
    public static event BedHandler OnOutOfBed;

    [SerializeField] Camera bedCamera;


    protected override void Awake()
    {
        base.Awake();
        
        OnOutOfBed += Bed_OnOutOfBed;
    }

    private void OnDestroy()
    {
        OnOutOfBed -= Bed_OnOutOfBed;
    }

    public override void Interact()
    {
        base.Interact();

        GetInBed();
    }

    private void GetInBed()
    {
        OnInBed?.Invoke();

        bedCamera.enabled = true;
    }

    // Events
    public static void GetOutOfBed()
    {
        OnOutOfBed?.Invoke();
    }

    // Event Listeners
    private void Bed_OnOutOfBed()
    {
        bedCamera.enabled = false;
        HasInteracted = false;
    }
}
