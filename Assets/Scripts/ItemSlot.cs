using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    // Data
    Item item;
    PickupItem itemObject;

    // UI
    Image itemDisplayImage;

    public void Initialize(Item item, PickupItem itemObject)
    {
        this.item = item;
        this.itemObject = itemObject;
        itemDisplayImage.sprite = item.Icon;
    }
}
