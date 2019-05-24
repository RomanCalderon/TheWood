using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Goal
{
    [System.NonSerialized] protected Quest Quest;
    public string Description;
    public bool Completed { get; set; }
    public int CurrentAmount { get; set; }
    public int RequiredAmount;

    public Goal(Quest quest)
    {
        Quest = quest;
    }

    public virtual void Init(Quest quest, bool completed, int currentAmount)
    {
        Quest = quest;

        Completed = completed;
        CurrentAmount = currentAmount;
    }

    public void Evaluate()
    {
        Completed = (CurrentAmount >= RequiredAmount);

        if (Completed)
            Quest.CheckGoals();

        Debug.Log("QuestController.UpdateQuestProgress(Quest)");
        QuestController.UpdateQuestProgress(Quest);
    }
}
