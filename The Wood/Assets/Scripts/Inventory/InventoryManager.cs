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
            Destroy(gameObject);
        else
            instance = this;

        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        UIEventHandler.OnItemAddedToInventory += UIEventHandler_OnItemAddedToInventory;

        playerWeaponController = GetComponent<PlayerWeaponController>();
        consumableController = GetComponent<ConsumableController>();

    }

    private void Start()
    {
        // FOR TESTING
        GiveDefaultItems();
    }

    #region Give Item

    private void GiveDefaultItems()
    {
        // The build tool is an "internal" Item, meaning it cannot be dropped or
        // equipped like other Items in the Inventory
        BuildTool = ItemDatabase.instance.GetItem("build_tool");

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

    public void GiveItem(Item item)
    {
        if (item == null)
            return;

        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    public void GiveItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
            GiveItem(item);
    }

    #endregion

    #region GetItem(s)

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

    public void RemoveItem(Item item)
    {
        if (item == null || !playerItems.Contains(item))
            return;

        UIEventHandler.ItemRemovedFromInventory(item);
        playerItems.Remove(item);
    }

    public void RemoveItem(string itemSlug)
    {
        if (itemSlug == string.Empty || !playerItems.Contains(GetItem(itemSlug)))
            return;

        UIEventHandler.ItemRemovedFromInventory(GetItem(itemSlug));
        playerItems.Remove(GetItem(itemSlug));
    }

    public void SetItemDetails(Item item, Button selectedButton)
    {
        inventoryDetailsPanel.SetItem(item, selectedButton);
    }

    // Events
    private void UpdatedItemList()
    {
        OnItemListUpdated?.Invoke(playerItems);
    }

    // Event Listeners
    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SaveInventoryManager(this, Application.persistentDataPath + "/inventory.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        InventoryManagerSaveData data = SaveSystem.LoadData<InventoryManagerSaveData>(Application.persistentDataPath + "/inventory.dat");

        // New game - Give default Items
        if (data == null)
        {
            GiveDefaultItems();
            return;
        }

        // From load - Give saved Items
        foreach (string itemSlug in data.playerItemsSlugs)
        {
            if (!playerItems.Contains(GetItem(itemSlug)))
                GiveItem(itemSlug);
        }

        //Debug.Log("Loaded InventoryManager");
    }
    
    /// <summary>
    /// Sorts the new Item into the Inventory list of Items
    /// </summary>
    /// <param name="item">The new Item.</param>
    private void UIEventHandler_OnItemAddedToInventory(Item item)
    {
        if (playerItems.Count > 0)
        {
            playerItems.Sort(SortByType);
        }

        // Invoke event (OnItemListUpdated)
        UpdatedItemList();
    }

    // Item compare method
    int SortByType(Item a, Item b)
    {
        return a.ItemType.CompareTo(b.ItemType);
    }
}
