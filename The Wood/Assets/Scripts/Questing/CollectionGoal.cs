using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionGoal : Goal
{
    public string ItemID;
    
    public override void Init(Quest quest)
    {
        base.Init(quest);
        UIEventHandler.OnItemAddedToInventory += ItemPickedUp;
    }

    void ItemPickedUp(Item item)
    {
        if (Quest.IsTracked && item.ItemSlug == ItemID)
        {
            CurrentAmount++;
            //Debug.Log("CollectionGoal progress = " + CurrentAmount + "/" + RequiredAmount);
            Evaluate();
        }
    }
}
