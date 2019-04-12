using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public RectTransform scrollViewContent;
    InventoryUIItem itemContainer { get; set; }
    Item currentSelectedItem { get; set; }
    

    private void Start()
    {
        itemContainer = Resources.Load<InventoryUIItem>("UI/Item_Container");
        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;
    }

    private void ItemAdded(Item item)
    {
        InventoryUIItem emptyItem = Instantiate(itemContainer, scrollViewContent);
        emptyItem.SetItem(item);
    }

    private void ItemRemoved(Item item)
    {
        foreach (Transform t in scrollViewContent)
        {
            if (t.GetComponent<InventoryUIItem>().Item.ItemSlug == item.ItemSlug)
            {
                Destroy(t.gameObject);
                break;
            }
        }
    }
}
