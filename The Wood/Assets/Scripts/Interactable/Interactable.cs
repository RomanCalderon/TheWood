using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public enum InteractionTypes
    {
        TALK,
        READ,
        BUILD,
        DECONSTRUCT,
        SEARCH,
        TAKE,
        SLEEP
    }

    [HideInInspector] public bool HasInteracted;
    [SerializeField] InteractionTypes interactionType;
    [SerializeField] protected string interactionName;
    public KeyCode interactionKeyBinding = KeyBindings.Interact;

    protected virtual void Awake()
    {
        DialogueSystem.OnDialogueFinished += DialogueSystem_OnDialogueFinished;
    }

    public virtual void Preview()
    {
        if (HasInteracted)
            return;

        InteractionController.PreviewInteraction("[" + KeyBindings.Interact + "] " + GetInteractablePrompt());
    }

    public virtual void Interact()
    {
        HasInteracted = true;
    }

    // Events
    private void DialogueSystem_OnDialogueFinished()
    {
        HasInteracted = false;
    }

    public void SetInteractionPrompt(InteractionTypes interactionType, string interactionName)
    {
        this.interactionType = interactionType;
        this.interactionName = interactionName;
    }


    protected string GetInteractablePrompt()
    {
        switch (interactionType)
        {
            case InteractionTypes.TALK:
                return "TALK WITH " + interactionName.ToUpper();
            case InteractionTypes.READ:
                return "READ " + interactionName.ToUpper();
            case InteractionTypes.BUILD:
                return "BUILD " + interactionName.ToUpper();
            case InteractionTypes.SEARCH:
                return "SEARCH " + interactionName.ToUpper();
            case InteractionTypes.TAKE:
                return "TAKE " + interactionName.ToUpper();
            case InteractionTypes.SLEEP:
                return "SLEEP";
            default:
                break;
        }

        Debug.LogError("Invalid interaction type: " + interactionType);
        return string.Empty;
    }

    public InteractionTypes GetInteractionType()
    {
        return interactionType;
    }
}
