using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public static InteractionController instance;

    public delegate void InteractionHandler(string interactablePrompt);
    public static event InteractionHandler OnPreviewInteractable;

    public static bool CanInteract = true;

    [SerializeField] new Transform camera;
    [SerializeField] float interactDistance = 4f;
    [SerializeField] LayerMask interactableLayermask;
    [SerializeField] LayerMask buildingLayerMask;

    [SerializeField] Text interactionPromptText;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        OnPreviewInteractable += ShowInteractablePrompt;
        UIEventHandler.OnUIDisplayed += UIEventHandler_OnUIDisplayed;
        SleepController.OnGoToSleep += SleepController_OnGoToSleep;
        SleepController.OnWakeUp += SleepController_OnWakeUp;
        BuildingController.OnDisabledBuildMode += HideInteractablePrompt;
    }

    private void OnDestroy()
    {
        OnPreviewInteractable -= ShowInteractablePrompt;
        UIEventHandler.OnUIDisplayed -= UIEventHandler_OnUIDisplayed;
        SleepController.OnGoToSleep -= SleepController_OnGoToSleep;
        SleepController.OnWakeUp -= SleepController_OnWakeUp;
        BuildingController.OnDisabledBuildMode -= HideInteractablePrompt;
    }

    // Update is called once per frame
    void Update()
    {
        Interactions();
    }

    /// <summary>
    /// Allows the player to interact with interactable objects in the game.
    /// Only works when CanInteract is true, the player is not sleeping and is not in BuildMode
    /// </summary>
    void Interactions()
    {
        if (CanInteract && !PlayerSleepController.IsInBed)
        {
            Ray ray = new Ray(camera.position, camera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayermask))
            {
                Interactable interactableObject = hit.transform.GetComponent<Interactable>();

                // If the hit object doesn't have an Interactable component, return
                if (interactableObject == null)
                {
                    HideInteractablePrompt();
                    return;
                }
                
                // Shows an interaction prompt for Blueprints if InBuildMode
                // Shows an interaction prompt for non-Blueprint objects if not InBuildMode
                if (BuildingController.InBuildMode)
                {
                    if (interactableObject is Blueprint)
                        interactableObject.Preview();
                    else
                        HideInteractablePrompt();
                }
                else
                {
                    if (!(interactableObject is Blueprint))
                        interactableObject.Preview();
                }

                /*
                // Display an interaction preview for Blueprints
                if (interactableObject is Blueprint)
                {
                    // Only if in BuildMode
                    if (BuildingController.InBuildMode)
                        interactableObject.Preview();
                }
                else if (interactableObject is ItemStorage)
                {
                    if (!BuildingController.InBuildMode)
                        interactableObject.Preview();
                    else
                        HideInteractablePrompt();
                }
                // If the interactableObject is not a Blueprint or ItemStorage
                else
                {
                    // And not in BuildMode, preview the object
                    if (!BuildingController.InBuildMode)
                        interactableObject.Preview();
                    else
                        HideInteractablePrompt();
                }
                */

                // If the interactionObject is not a Blueprint, interact with the object
                if ((interactableObject is Blueprint) == false)
                    if (Input.GetKeyDown(KeyBindings.Interact) && !BuildingController.InBuildMode)
                    {
                        interactableObject.Interact();
                        HideInteractablePrompt();
                    }
            }
            else
                HideInteractablePrompt();
        }
    }

    /// <summary>
    /// If the player CanInteract and is not sleeping,
    /// get the current Blueprint object in front of the player.
    /// </summary>
    /// <returns>A Blueprint object</returns>
    public Blueprint GetBlueprint()
    {
        if (CanInteract && !PlayerSleepController.IsInBed)
        {
            Ray ray = new Ray(camera.position, camera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayermask))
                return hit.transform.GetComponent<Blueprint>();
        }

        return null;
    }

    /// <summary>
    /// If the player CanInteract and is not sleeping,
    /// get the current Building object in front of the player.
    /// </summary>
    /// <returns>A Building object</returns>
    public Building GetBuilding()
    {
        if (CanInteract && !PlayerSleepController.IsInBed)
        {
            Ray ray = new Ray(camera.position, camera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, buildingLayerMask))
                return hit.transform.GetComponent<Building>();
        }

        return null;
    }

    // Events
    public static void PreviewInteraction(string interactablePrompt)
    {
        OnPreviewInteractable?.Invoke(interactablePrompt);
    }

    // Listeners
    private void ShowInteractablePrompt(string interactablePrompt)
    {
        interactionPromptText.gameObject.SetActive(true);
        interactionPromptText.text = interactablePrompt;
    }

    private void UIEventHandler_OnUIDisplayed(bool state)
    {
        CanInteract = !state;

        // Hide the interaction prompt if a UI element is displayed (ie. Dialogue)
        if (!CanInteract)
            HideInteractablePrompt();
    }

    private void HideInteractablePrompt()
    {
        interactionPromptText.text = string.Empty;
        interactionPromptText.gameObject.SetActive(false);
    }

    private void SleepController_OnGoToSleep()
    {
        CanInteract = false;
        HideInteractablePrompt();
    }

    private void SleepController_OnWakeUp()
    {
        CanInteract = true;
    }
}
