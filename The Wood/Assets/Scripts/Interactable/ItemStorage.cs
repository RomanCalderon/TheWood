using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Space]
    [SerializeField] CanvasGroup chestCanvasGroup;

    public List<Item> items = new List<Item>();

    protected override void Awake()
    {
        base.Awake();

        // Save/Load
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        animator = GetComponent<Animator>();

        SetCanvasGroupActive(false);
    }
    
    public override void Interact()
    {
        if (HasInteracted)
            return;

        // Play the OpenChest animation, then display the chest UI
        animator.SetTrigger("OpenChest");
        StartCoroutine(OpenChestDelay(0.6f));

        base.Interact();
    }

    /// <summary>
    /// Displays the chest UI
    /// </summary>
    private void OpenChest()
    {
        print("Opened Chest");
        SetCanvasGroupActive(true);
        UIEventHandler.UIDisplayed(true);
    }

    /// <summary>
    /// Hides the chest display. Closes the chest.
    /// </summary>
    private void CloseChest()
    {
        animator.SetTrigger("CloseChest");
        SetCanvasGroupActive(false);
        UIEventHandler.UIDisplayed(false);
        HasInteracted = false;
    }

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
