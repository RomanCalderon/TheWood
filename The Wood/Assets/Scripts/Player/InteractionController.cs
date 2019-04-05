using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public delegate void InteractionHandler(string interactablePrompt);
    public static event InteractionHandler OnPreviewInteractable;

    [HideInInspector] public bool CanInteract = true;

    [SerializeField] new Transform camera;
    [SerializeField] float interactDistance = 4f;
    [SerializeField] LayerMask interactableLayermask;

    [SerializeField] Text interactionPromptText;

    private void Awake()
    {
        OnPreviewInteractable += ShowInteractablePrompt;
        UIEventHandler.OnUIDisplayed += UIEventHandler_OnUIDisplayed;
        SleepController.OnGoToSleep += SleepController_OnGoToSleep;
        SleepController.OnWakeUp += SleepController_OnWakeUp;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Interactions();
    }

    void Interactions()
    {
        if (CanInteract && !PlayerSleepController.IsSleeping)
        {
            Ray ray = new Ray(camera.position, camera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayermask))
            {
                Interactable interactableObject = hit.transform.GetComponent<Interactable>();

                if (!interactableObject.HasInteracted)
                    interactableObject.Preview();

                if (Input.GetKeyDown(KeyBindings.Interact))
                    if (!interactableObject.HasInteracted)
                        interactableObject.Interact();
            }
            else
                HideInteractablePrompt();
        }
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
        interactionPromptText.text = "[" + KeyBindings.Interact + "] " + interactablePrompt;
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
