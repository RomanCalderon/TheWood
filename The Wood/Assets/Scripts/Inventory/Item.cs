using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum ItemType
{
    CONSUMABLE, // A consumable type
    WEAPON,     // A weapon type
    GOOD,       // An Item with value
    TOOL,       // An Item that helps accomplish tasks
    QUESTING,   // A Quest Item
    RESOURCE    // An Item used for building
}

[Serializable]
public class Item
{
    [Header("Data")]
    public string Name;         // Name of the Item
    public string ID;             // Item ID
    public string ItemSlug;     // Item slug
    public string Description;  // A description for the Item
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ItemType ItemType;   // The type of Item
    public string ActionName;   // Ex: Drink, Use, Equip
    public List<BaseStat> Stats { get; set; }
    public bool ItemModifier;

    [Header("UI")]
    public Sprite Icon;


    #region Constructors
    
    public Item(List<BaseStat> stats, string itemSlug)
    {
        Stats = stats;
        ItemSlug = itemSlug;
        ID = Guid.NewGuid().ToString();
    }

    [JsonConstructor]
    public Item(List<BaseStat> stats, string itemSlug, string description, ItemType itemType, string actionName, string itemName, bool itemModifier)
    {
        Stats = stats;
        ID = Guid.NewGuid().ToString();
        ItemSlug = itemSlug;
        Description = description;
        ItemType = itemType;
        ActionName = actionName;
        Name = itemName;
        ItemModifier = itemModifier;
    }

    #endregion

    /// <summary>
    /// Compares other by its ID (Guid).
    /// Returns true if others' ID is the exact same Guid.
    /// </summary>
    /// <param name="other">Other Item being compared to.</param>
    /// <returns>True if ID is the exact same Guid.</returns>
    public bool Equals(Item other)
    {
        return ID == other.ID;
    }

}
