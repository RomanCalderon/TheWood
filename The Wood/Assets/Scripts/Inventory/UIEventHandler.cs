using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    public delegate void UIDisplayEventHandler(bool state);
    public static event UIDisplayEventHandler OnUIDisplayed;

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

    public static void UIDisplayed(bool state)
    {
        OnUIDisplayed?.Invoke(state);
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
}
