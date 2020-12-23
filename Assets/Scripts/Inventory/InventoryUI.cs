using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public RectTransform scrollViewContent;
    InventoryUIItem itemContainer { get; set; }
    Item currentSelectedItem { get; set; }
    

    private void Awake()
    {
        itemContainer = Resources.Load<InventoryUIItem>("UI/Item_Container");

        InventoryManager.OnItemListUpdated += InventoryManager_OnItemListUpdated;
        UIEventHandler.OnItemRemovedFromInventory += RemoveItemUI;
    }

    private void OnDestroy()
    {
        InventoryManager.OnItemListUpdated -= InventoryManager_OnItemListUpdated;
        UIEventHandler.OnItemRemovedFromInventory -= RemoveItemUI;
    }

    private void InventoryManager_OnItemListUpdated(List<Item> updatedList)
    {
        // Clear old UI Item elements
        ClearItemUIList();

        // Construct new list of UI Item elements
        foreach (Item item in updatedList)
            AddItemUI(item);
    }

    private void AddItemUI(Item item)
    {
        InventoryUIItem emptyItem = Instantiate(itemContainer, scrollViewContent);
        emptyItem.SetItem(item);
    }

    private void RemoveItemUI(Item item)
    {
        foreach (Transform t in scrollViewContent)
        {
            if (t.GetComponent<InventoryUIItem>() == null)
                continue;

            if (t.GetComponent<InventoryUIItem>().Item.ItemSlug == item.ItemSlug)
            {
                Destroy(t.gameObject);
                break;
            }
        }
    }

    private void ClearItemUIList()
    {
        foreach (Transform t in scrollViewContent)
            Destroy(t.gameObject);
    }
}
