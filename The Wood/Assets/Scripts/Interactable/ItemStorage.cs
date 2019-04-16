using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStorageSaveData
{
    public List<Item> items;

    public ItemStorageSaveData(ItemStorage itemStorage)
    {
        items = itemStorage.items;
    }
}

public class ItemStorage : Interactable
{
    public List<Item> items = new List<Item>();

    protected override void Awake()
    {
        base.Awake();

        // Save/Load
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;
    }

    public override void Interact()
    {
        base.Interact();
    }

    #region Event Listeners

    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SaveItemStorage(this, Application.persistentDataPath + "/itemstorage.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        ItemStorageSaveData data = SaveSystem.LoadData<ItemStorageSaveData>(Application.persistentDataPath + "/itemstorage.dat");

        if (data == null)
            return;

        items = data.items;
    }

    #endregion
}
