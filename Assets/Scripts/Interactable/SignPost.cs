using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignPost : ActionItem
{
    [SerializeField]
    string signName;
    [SerializeField]
    string[] dialogue;

    public override void Interact()
    {
        base.Interact();

        DialogueSystem.instance.AddNewDialogue(signName, dialogue);
    }
}
