using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIDetails : MonoBehaviour
{
    Item item;
    Button selectedItemButton;

    [SerializeField] Transform itemDropTransform;
    [SerializeField] Button itemInteractButton, dropItemButton;
    [SerializeField] Text itemNameText, itemTypeText, itemDescriptionText, itemInteractButtonText;
    [SerializeField] Text statText;

    private void Start()
    {
        DisplayItemDetails(false);
    }

    public void SetItem(Item item, Button selectedButton)
    {
        DisplayItemDetails(true);
        statText.text = string.Empty;
        if (item.Stats != null)
            foreach (BaseStat stat in item.Stats)
                statText.text += stat.BaseValue + " " + stat.StatName + "\n";

        itemInteractButton.onClick.RemoveAllListeners();
        dropItemButton.onClick.RemoveAllListeners();
        
        this.item = item;
        selectedItemButton = selectedButton;
        itemNameText.text = item.Name;
        itemTypeText.text = item.ItemType.ToString();
        itemDescriptionText.text = item.Description;
        
        // Disable action button if ActionName is empty
        if (item.ActionName == "")
            itemInteractButton.gameObject.SetActive(false);
        // Otherwise setup action button
        else
        {
            itemInteractButton.gameObject.SetActive(true);
            itemInteractButtonText.text = item.ActionName;
            itemInteractButton.onClick.AddListener(OnItemInteract);
        }

        dropItemButton.onClick.AddListener(OnDropItem);
    }

    public void OnItemInteract()
    {
        switch (item.ItemType)
        {
            case ItemType.CONSUMABLE:
                InventoryManager.instance.ConsumeItem(item);
                break;
            case ItemType.WEAPON:
            case ItemType.GOOD:
            case ItemType.TOOL:
                InventoryManager.instance.EquipItem(item);
                break;
            default:
                break;
        }
        
        RemoveItem();
    }

    public void OnDropItem()
    {
        ItemDatabase.instance.DropLoot(item, itemDropTransform.position);
        InventoryManager.instance.RemoveItem(item);
        RemoveItem();
    }

    private void RemoveItem()
    {
        item = null;
        DisplayItemDetails(false);
    }

    private void DisplayItemDetails(bool state)
    {
        gameObject.SetActive(state);
    }
}
