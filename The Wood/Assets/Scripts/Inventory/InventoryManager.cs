using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryManagerSaveData
{
    public List<string> playerItemsSlugs;

    public InventoryManagerSaveData(InventoryManager inventoryManager)
    {
        playerItemsSlugs = new List<string>();

        foreach (Item item in inventoryManager.GetAllItems())
            playerItemsSlugs.Add(item.ItemSlug);
    }
}

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerWeaponController))]
[RequireComponent(typeof(ConsumableController))]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; set; }

    public delegate void ItemListHandler(List<Item> updatedList);
    public static event ItemListHandler OnItemListUpdated;

    PlayerWeaponController playerWeaponController;
    ConsumableController consumableController;
    public InventoryUIDetails inventoryDetailsPanel;

    [SerializeField] List<Item> playerItems = new List<Item>();

    public Item BuildTool { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        UIEventHandler.OnItemAddedToInventory += UIEventHandler_OnItemAddedToInventory;
        UIEventHandler.OnItemRemovedFromInventory += UIEventHandler_OnItemRemovedFromInventory;

        playerWeaponController = GetComponent<PlayerWeaponController>();
        consumableController = GetComponent<ConsumableController>();
    }

    private void OnDestroy()
    {
        instance = null;
        SaveLoadController.OnSaveGame -= SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame -= SaveLoadController_OnLoadGame;

        UIEventHandler.OnItemAddedToInventory -= UIEventHandler_OnItemAddedToInventory;
        UIEventHandler.OnItemRemovedFromInventory -= UIEventHandler_OnItemRemovedFromInventory;
    }

    private void Start()
    {
        // The build tool is an "internal" Item, meaning it cannot be dropped or
        // equipped like other Items in the Inventory
        BuildTool = ItemDatabase.instance.GetItem("build_tool");

        // FOR TESTING
        //GiveDefaultItems();
    }

    #region Give Item

    private void GiveDefaultItems()
    {
        // Basic Items given at the beginning
        GiveItem("torch");
        GiveItem("potion_log");
        GiveItem("pitchfork");
        GiveItem("wood", 7);
        GiveItem("rope", 5);
        GiveItem("iron_scrap", 3);
    }

    public void GiveItem(string itemSlug)
    {
        Item item = ItemDatabase.instance.GetItem(itemSlug);
        
        GiveItem(item);
    }

    public void GiveItem(string itemSlug, int amount)
    {
        Item item = ItemDatabase.instance.GetItem(itemSlug);
        
        GiveItem(item, amount);
    }

    public void GiveItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
            GiveItem(item);
    }

    public void GiveItem(Item item)
    {
        if (item == null)
            return;

        item.CreateID();
        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    #endregion

    #region Get Item(s)

    public Item GetItem(string itemSlug)
    {
        foreach (Item item in playerItems)
            if (item.ItemSlug == itemSlug)
                return item;

        Debug.Log("Item with slug [" + itemSlug + "] isn't in inventory");
        return null;
    }

    public List<Item> GetAllItems()
    {
        return playerItems;
    }

    public List<Item> GetItemsOfType(ItemType itemType)
    {
        List<Item> result = new List<Item>();

        foreach (Item item in playerItems)
            if (item.ItemType == itemType)
                result.Add(item);

        return result;
    }

    #endregion

    #region Equip/Unequip/Consume Item

    public void EquipItem(Item itemToEquip)
    {
        playerWeaponController.EquipWeapon(itemToEquip);
    }

    public void EquipBuildTool()
    {
        playerWeaponController.EquipWeapon(BuildTool);
    }

    public void UnequipBuildTool()
    {
        playerWeaponController.UnequipWeapon();
    }

    public void ConsumeItem(Item itemToConsume)
    {
        consumableController.ConsumeItem(itemToConsume);
    }

    #endregion

    #region Remove Item

    public void RemoveItem(Item item)
    {
        // Item is null
        if (item == null)
            return;

        // Item doesn't exist in player inventory
        if (playerItems.Find(i => i.ItemSlug == item.ItemSlug) == null)
            return;

        playerItems.Remove(item);
        UIEventHandler.ItemRemovedFromInventory(item);
    }

    public void RemoveItem(string itemSlug)
    {
        if (itemSlug == string.Empty || !playerItems.Contains(GetItem(itemSlug)))
            return;

        RemoveItem(GetItem(itemSlug));
    }

    #endregion

    public void SetItemDetails(Item item, Button selectedButton)
    {
        inventoryDetailsPanel.SetItem(item, selectedButton);
    }

    // Events
    public void UpdateItemList()
    {
        OnItemListUpdated?.Invoke(playerItems);
    }

    // Event Listeners
    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SaveInventoryManager(this, "/inventory.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        InventoryManagerSaveData data = SaveSystem.LoadData<InventoryManagerSaveData>("/inventory.dat");

        // New game - Give default Items
        if (data == null)
        {
            GiveDefaultItems();
            return;
        }

        // From load - Give saved Items
        foreach (string itemSlug in data.playerItemsSlugs)
            GiveItem(itemSlug);
    }
    
    /// <summary>
    /// Sorts the new Item into the Inventory list of Items
    /// then invokes the OnItemListUpdated event.
    /// </summary>
    /// <param name="item">The new Item.</param>
    private void UIEventHandler_OnItemAddedToInventory(Item item)
    {
        if (playerItems.Count > 0)
            playerItems.Sort(SortByType);

        // Invoke event (OnItemListUpdated)
        UpdateItemList();
    }

    private void UIEventHandler_OnItemRemovedFromInventory(Item item)
    {
        if (playerItems.Count > 0)
            playerItems.Sort(SortByType);

        // Invoke event (OnItemListUpdated)
        UpdateItemList();
    }

    // Item compare method
    int SortByType(Item a, Item b)
    {
        return a.ItemType.CompareTo(b.ItemType);
    }
}
