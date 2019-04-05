using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Goal
{
    protected Quest Quest;
    public string Description;
    public bool Completed { get; set; }
    public int CurrentAmount { get; set; }
    public int RequiredAmount;

    public virtual void Init(Quest quest)
    {
        Quest = quest;
        Completed = false;
        CurrentAmount = 0;
    }

    public void Evaluate()
    {
        Completed = (CurrentAmount >= RequiredAmount);

        if (Completed)
            Quest.CheckGoals();

        QuestController.UpdateQuestProgress(Quest);
    }
}
