using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIItem : MonoBehaviour
{
    public Item Item;

    [Header("UI")]
    [SerializeField]
    Text itemNameText;
    [SerializeField]
    Image itemImage;

    public void SetItem(Item item)
    {
        Item = item;
        SetupItemValue();
    }

    void SetupItemValue()
    {
        itemNameText.text = Item.Name;
        itemImage.sprite = Resources.Load<Sprite>("UI/Icons/Items/" + Item.ItemSlug);
    }

    public void OnSelectItemButton()
    {
        InventoryManager.instance.SetItemDetails(Item, GetComponent<Button>());
    }
}
