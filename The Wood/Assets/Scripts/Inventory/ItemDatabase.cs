using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance { get; set; }

    private List<Item> items { get; set; }

    [SerializeField] PickupItem pickupItem;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        BuildDatabase();
    }

    private void BuildDatabase()
    {
        items = JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("JSON/Items").ToString());
    }

    /// <summary>
    /// Used for retreiving an Item from the Item database.
    /// </summary>
    /// <param name="itemSlug">Slug for the requested Item.</param>
    /// <returns>Item in the database.</returns>
    public Item GetItem(string itemSlug)
    {
        foreach (Item item in items)
            if (item.ItemSlug == itemSlug)
                return new Item(item);

        Debug.LogError("Couldn't find item with slug [" + itemSlug + "]");
        return null;
    }

    public void DropLoot(string itemSlug, Vector3 pos)
    {
        Item item = GetItem(itemSlug);

        if (item != null)
        {
            PickupItem instance = Instantiate(pickupItem, pos, Quaternion.identity);
            instance.Item = item;
            instance.SetInteractionPrompt(Interactable.InteractionTypes.TAKE, item.Name);
        }
    }

    public void DropLoot(Item item, Vector3 pos)
    {
        if (item != null)
        {
            PickupItem instance = Instantiate(pickupItem, pos, Quaternion.identity);
            instance.Item = new Item(item);
            instance.SetInteractionPrompt(Interactable.InteractionTypes.TAKE, item.Name);
        }
    }
}
