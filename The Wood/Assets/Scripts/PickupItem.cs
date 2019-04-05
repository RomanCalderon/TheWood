using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : Interactable
{
    public Item Item { get; set; }

    public override void Interact()
    {
        InventoryManager.instance.GiveItem(Item);
        Destroy(gameObject);
    }
}
