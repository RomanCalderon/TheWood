using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
    public string Name;
    public string[] Dialogue;

    public override void Interact()
    {
        base.Interact();

        DialogueSystem.instance.AddNewDialogue(Name, Dialogue);

        Debug.Log("Interacting with NPC.");
    }
}
