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

[RequireComponent(typeof(Animator))]
public class ItemStorage : Interactable
{
    Animator animator;

    public List<Item> items = new List<Item>();

    protected override void Awake()
    {
        base.Awake();

        // Save/Load
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        animator = GetComponent<Animator>();
    }
    
    public override void Interact()
    {
        if (HasInteracted)
            return;

        print("Opened Chest");
        animator.SetTrigger("OpenChest");

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
