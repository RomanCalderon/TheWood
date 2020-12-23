using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionGoal : Goal
{
    public string ItemID;

    public CollectionGoal(Quest quest) : base(quest) { }

    public override void Init(Quest quest, bool completed, int currentAmount)
    {
        Quest = quest;

        Completed = completed;
        CurrentAmount = currentAmount;

        UIEventHandler.OnItemAddedToInventory += ItemPickedUp;
    }

    void ItemPickedUp(Item item)
    {
        if (Quest.IsTracked && !Quest.Completed && item.ItemSlug == ItemID)
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
