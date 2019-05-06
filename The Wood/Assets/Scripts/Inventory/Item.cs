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
    [SerializeField] private string ID;             // Item ID
    public string ItemSlug;     // Item slug
    public string Description;  // A description for the Item
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ItemType ItemType;   // The type of Item
    public string ActionName;   // Ex: Drink, Use, Equip
    public List<BaseStat> Stats { get; set; }
    public bool ItemModifier;

    private bool idCreated = false;

    [Header("UI")]
    public Sprite Icon;


    #region Constructors

    /// <summary>
    /// This constructor should ONLY be called from the ItemDatabase class.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="itemSlug"></param>
    /// <param name="description"></param>
    /// <param name="itemType"></param>
    /// <param name="actionName"></param>
    /// <param name="itemName"></param>
    /// <param name="itemModifier"></param>
    [JsonConstructor]
    public Item(List<BaseStat> stats, string itemSlug, string description, ItemType itemType, string actionName, string itemName, bool itemModifier)
    {
        Name = itemName;
        CreateID();
        ItemSlug = itemSlug;
        Description = description;
        ItemType = itemType;
        ActionName = actionName;
        Stats = stats;
        ItemModifier = itemModifier;
    }

    /// <summary>
    /// Copy constructor with unique ID.
    /// </summary>
    public Item(Item item)
    {
        Name = item.Name;
        idCreated = false;
        CreateID();
        ItemSlug = item.ItemSlug;
        Description = item.Description;
        ItemType = item.ItemType;
        ActionName = item.ActionName;
        Stats = item.Stats;
        ItemModifier = item.ItemModifier;
    }

    #endregion

    /// <summary>
    /// Creates a new, unique Item Guid.
    /// </summary>
    public void CreateID()
    {
        if (idCreated)
            return;

        ID = Guid.NewGuid().ToString();
        //Debug.Log("[" + Name + "] Create new guid [" + ID + "]");

        idCreated = true;
    }

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
