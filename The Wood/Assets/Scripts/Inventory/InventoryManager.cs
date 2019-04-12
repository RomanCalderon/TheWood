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
    }

    public void GiveItem(string itemSlug)
    {
        Item item = ItemDatabase.instance.GetItem(itemSlug);
        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    public void GiveItem(string itemSlug, int amount)
    {
        Item item = ItemDatabase.instance.GetItem(itemSlug);

        for (int i = 0; i < amount; i++)
        {
            playerItems.Add(item);
            UIEventHandler.ItemAddedToInventory(item);
        }
    }

    public void GiveItem(Item item)
    {
        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    public void GiveItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            playerItems.Add(item);
            UIEventHandler.ItemAddedToInventory(item);
        }
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


    public void SetItemDetails(Item item, Button selectedButton)
    {
        //if (!inventoryDetailsPanel.isActiveAndEnabled)
        //    return;

        inventoryDetailsPanel.SetItem(item, selectedButton);
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

}
