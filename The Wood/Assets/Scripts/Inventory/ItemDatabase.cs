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

    public Item GetItem(string itemSlug)
    {
        foreach (Item item in items)
            if (item.ItemSlug == itemSlug)
                return item;

        Debug.LogError("Couldn't find item: " + itemSlug);
        return null;
    }

    public void DropLoot(string itemSlug, Vector3 pos)
    {
        Item item = GetItem(itemSlug);

        if (item != null)
        {
            PickupItem instance = Instantiate(pickupItem, transform.position, Quaternion.identity);
            instance.Item = item;
            instance.SetInteractionPrompt(Interactable.InteractionTypes.TAKE, item.Name);
        }
    }

    public void DropLoot(Item item, Vector3 pos)
    {
        if (item != null)
        {
            PickupItem instance = Instantiate(pickupItem, pos, Quaternion.identity);
            instance.Item = item;
            instance.SetInteractionPrompt(Interactable.InteractionTypes.TAKE, item.Name);
        }
    }
}
