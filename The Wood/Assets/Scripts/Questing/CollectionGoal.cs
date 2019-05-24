using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionGoal : Goal
{
    public string ItemID;
    
    public CollectionGoal(Quest quest) : base(quest)
    {
        Debug.Log("CollectionGoal()");
        //UIEventHandler.OnItemAddedToInventory += ItemPickedUp;
    }

    public override void Init(Quest quest, bool completed, int currentAmount)
    {
        Debug.Log("Collection Goal [" + Description + "] initialized.");
        Quest = quest;

        Completed = completed;
        CurrentAmount = currentAmount;

        UIEventHandler.OnItemAddedToInventory += ItemPickedUp;
    }

    void ItemPickedUp(Item item)
    {
        if (Quest.IsTracked && item.ItemSlug == ItemID)
        {
            CurrentAmount++;
            Debug.Log("CollectionGoal progress = " + CurrentAmount + "/" + RequiredAmount);
            Evaluate();
        }
    }

    ~CollectionGoal()
    {
        UIEventHandler.OnItemAddedToInventory -= ItemPickedUp;
    }
}
