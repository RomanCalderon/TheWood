using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemStorageSaveData
{
    public List<string> itemSlugs;

    public ItemStorageSaveData(ItemStorage itemStorage)
    {
        itemSlugs = new List<string>();

        foreach (Item item in itemStorage.storageItems)
            itemSlugs.Add(item.ItemSlug);
    }
}

[RequireComponent(typeof(Animator))]
public class ItemStorage : Interactable
{
    Animator animator;

    [Space]
    [SerializeField] CanvasGroup chestCanvasGroup;
    [SerializeField] Button closeButton;

    [Space]
    [SerializeField] RectTransform playerItemsHolder;
    [SerializeField] RectTransform storageItemsHolder;

    InventoryUIItem itemContainer { get; set; }

    [Space]
    [SerializeField] int storageCapacity = 50;

    [Space]
    [SerializeField] DropZone.DropZoneIDs playerDropZone = DropZone.DropZoneIDs.DROPZONE1;
    [SerializeField] DropZone.DropZoneIDs storageDropZone = DropZone.DropZoneIDs.DROPZONE2;
    [Space]
    public List<Item> playerItems = new List<Item>();
    public List<Item> storageItems = new List<Item>();

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        itemContainer = Resources.Load<InventoryUIItem>("UI/Item_Container");

        // Save/Load
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        // Called on this Awake instead of the standard OnLoadGame event
        // because this object isn't in the scene by the time the OnLoadGame event is called
        SaveLoadController_OnLoadGame();

        SetCanvasGroupActive(false);
    }

    public override void Interact()
    {
        if (HasInteracted)
            return;

        // Play the OpenChest animation, then display the chest UI
        animator.SetTrigger("OpenChest");
        StartCoroutine(OpenChestDelay(0.6f));
        UIEventHandler.UIDisplayed(true);
        UIEventHandler.ItemStorageActive(true);

        base.Interact();
    }

    /// <summary>
    /// Checks if the target DropZone should hold obj.
    /// </summary>
    /// <param name="obj">Object being dropped.</param>
    /// <param name="dropZoneID">ID of the DropZone.</param>
    /// <param name="validDrop">Callback if drop is valid.</param>
    public void ElementRequest(GameObject obj, DropZone.DropZoneIDs dropZoneID, Action validDrop)
    {
        InventoryUIItem uiItem = obj.GetComponent<InventoryUIItem>();
        if (uiItem == null)
            return;

        if (dropZoneID == storageDropZone)
        {
            if (storageItems.Count + 1 <= storageCapacity)
                validDrop();
        }
        else if (dropZoneID == playerDropZone)
        {
            // FIXME: Add a player inventory capacity check
            validDrop();
        }
    }
    
    /// <summary>
    /// Called from UnityEvents.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="inStorage"></param>
    public void ReceiveElement(GameObject obj, DropZone.DropZoneIDs dropZoneID)
    {
        InventoryUIItem uiItem = obj.GetComponent<InventoryUIItem>();
        if (uiItem == null)
            return;
        
        // Destination is storage
        if (dropZoneID == storageDropZone)
        {
            // Check if the Item is already in storage
            if (ContainsItem(storageItemsHolder, uiItem.Item))
            {
                print("Item [" + uiItem.Item.Name + "] already in storage.");
                return;
            }

            // FIXME: It it's getting removed from the players' inventory
            // Item keeps getting added back to player inventory after moving it to storage

            storageItems.Add(uiItem.Item);
            playerItems.Remove(uiItem.Item);

            // Remove this Item from the players' inventroy
            InventoryManager.instance.RemoveItem(uiItem.Item);
        }
        // Destination is player inventory
        else if (dropZoneID == playerDropZone)
        {
            // Check if the Item is already in the players' inventory
            if (ContainsItem(playerItemsHolder, uiItem.Item))
            {
                print("Item [" + uiItem.Item.Name + "] already in player inventory.");
                return;
            }

            //playerItems.Add(uiItem.Item);
            storageItems.Remove(uiItem.Item);

            // Add this Item to the player's inventory
            InventoryManager.instance.GiveItem(uiItem.Item);
        }
    }

    #region Open/Close Chest

    /// <summary>
    /// Displays the chest UI
    /// </summary>
    private void OpenChest()
    {
        SetCanvasGroupActive(true);
        closeButton.onClick.AddListener(delegate { CloseChest(); });

        // Get an updated list of Items in the player's inventory
        InventoryManager.OnItemListUpdated += GetUpdatedItemList;
        InventoryManager.instance.UpdateItemList();
    }

    /// <summary>
    /// Hides the chest display. Closes the chest.
    /// </summary>
    private void CloseChest()
    {
        animator.SetTrigger("CloseChest");
        SetCanvasGroupActive(false);
        UIEventHandler.UIDisplayed(false);
        UIEventHandler.ItemStorageActive(false);

        InventoryManager.OnItemListUpdated -= GetUpdatedItemList;
        closeButton.onClick.RemoveAllListeners();

        // Clear playerItems UI and list
        foreach (Item item in playerItems)
            RemoveItemUI(item, playerItemsHolder);

        playerItems.Clear();

        HasInteracted = false;
    }

    #endregion
    
    #region Event Listeners

    /// <summary>
    /// Gets a list of all Items in the players' inventory.
    /// </summary>
    /// <param name="updatedList">List of Items in players' inventory.</param>
    private void GetUpdatedItemList(List<Item> updatedList)
    {
        playerItems.Clear();
        playerItems = new List<Item>(updatedList);

        UpdatePlayerItemsUI();

        UpdateStorageItemsUI();
    }

    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SaveItemStorage(this, "/itemstorage.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        ItemStorageSaveData data = SaveSystem.LoadData<ItemStorageSaveData>("/itemstorage.dat");

        if (data == null)
            return;

        // Clear the list before adding all saved storage Items
        storageItems.Clear();

        // From load - Give saved Items
        foreach (string itemSlug in data.itemSlugs)
            storageItems.Add(ItemDatabase.instance.GetItem(itemSlug));

        // Update UI to match storageItems list
        UpdateStorageItemsUI();
    }

    #endregion

    #region Helper Functions
    
    private bool ContainsItem(Transform parent, Item item)
    {
        foreach (Transform t in parent)
        {
            if (t.GetComponent<InventoryUIItem>() != null)
            {
                Item i = t.GetComponent<InventoryUIItem>().Item;

                if (item.Equals(i))
                    return true;
            }
        }

        return false;
    }

    private void UpdatePlayerItemsUI()
    {
        // Remove all UI elements in playerItemsHolder
        ClearPlayerItemsUI();
        
        // Construct new list of UI Item elements from playerItems list
        foreach (Item item in playerItems)
            AddItemUI(item, playerItemsHolder);
    }

    private void UpdateStorageItemsUI()
    {
        // Remove all UI elements in storageItemsHolder
        ClearStorageItemsUI();

        // Construct new list of UI Item elements from storageItems list
        foreach (Item item in storageItems)
            AddItemUI(item, storageItemsHolder);
    }

    private void ClearPlayerItemsUI()
    {
        RemoveAllUI(playerItemsHolder);
    }

    private void ClearStorageItemsUI()
    {
        RemoveAllUI(storageItemsHolder);
    }

    private void RemoveAllUI(RectTransform container)
    {
        foreach (Transform t in container)
            Destroy(t.gameObject);
    }

    private void AddItemUI(Item item, RectTransform parent)
    {
        InventoryUIItem emptyItem = Instantiate(itemContainer, parent);
        emptyItem.SetItem(item);
    }

    private void RemoveItemUI(Item item, RectTransform parent)
    {
        foreach (Transform t in parent)
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

    /// <summary>
    /// Hides or displays the Canvas via the CanvasGroup component.
    /// </summary>
    /// <param name="state"></param>
    private void SetCanvasGroupActive(bool state)
    {
        chestCanvasGroup.alpha = (state) ? 1f : 0f;
        chestCanvasGroup.interactable = state;
        chestCanvasGroup.blocksRaycasts = state;
    }

    private IEnumerator OpenChestDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        OpenChest();
    }

    #endregion
}
