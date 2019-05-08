using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    public delegate void UIDisplayEventHandler(bool state);
    public static event UIDisplayEventHandler OnUIDisplayed;
    private static int uiDisplayCount = 0;
    public static bool IsUIDisplayed
    {
        get { return uiDisplayCount > 0; }
        private set { }
    }

    public delegate void ItemEventHandler(Item item);
    public static event ItemEventHandler OnItemAddedToInventory;
    public static event ItemEventHandler OnItemRemovedFromInventory;
    public static event ItemEventHandler OnItemEquipped;
    public static event ItemEventHandler OnItemUnequipped;

    public delegate void PlayerHealthEventHandler(int currentHealth, int maxHealth);
    public static event PlayerHealthEventHandler OnPlayerHealthChanged;

    public delegate void StatsEventHandler();
    public static event StatsEventHandler OnStatsChanged;

    public delegate void PlayerLevelEventHandler();
    public static event PlayerLevelEventHandler OnPlayerLevelChange;

    public delegate void ItemStorageHandler(bool state);
    public static event ItemStorageHandler OnItemStorage;

    public static void UIDisplayed(bool state)
    {
        if (state)
            uiDisplayCount++;
        else if (uiDisplayCount > 0)
            uiDisplayCount--;

        //print("UIs displayed: " + uiDisplayCount);
        OnUIDisplayed?.Invoke(IsUIDisplayed);
    }

    public static void ItemAddedToInventory(Item item)
    {
        OnItemAddedToInventory?.Invoke(item);
    }

    public static void ItemRemovedFromInventory(Item item)
    {
        OnItemRemovedFromInventory?.Invoke(item);
    }

    public static void ItemEquipped(Item item)
    {
        OnItemEquipped?.Invoke(item);
    }

    public static void ItemUnequipped(Item item)
    {
        OnItemUnequipped?.Invoke(item);
    }

    public static void HealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static void StatsChanged()
    {
        OnStatsChanged?.Invoke();
    }

    public static void PlayerLevelChanged()
    {
        OnPlayerLevelChange?.Invoke();
    }

    public static void ItemStorageActive(bool state)
    {
        OnItemStorage?.Invoke(state);
    }
}
