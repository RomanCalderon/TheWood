using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string QuestName;
    public string Description;
    public List<KillGoal> KillGoals = new List<KillGoal>();
    public List<CollectionGoal> CollectionGoals = new List<CollectionGoal>();
    public int ExperienceReward;
    public int MoneyReward;
    public Item ItemReward;
    [HideInInspector] public bool IsTracked;
    [HideInInspector] public bool Completed;

    public void InitializeQuest()
    {
        QuestController.OnQuestTracked += QuestTracked;
        QuestController.OnQuestUntracked += QuestUntracked;
        QuestController.OnQuestAbandoned += QuestAbandoned;

        KillGoals.ForEach(g => g.Init(this));
        CollectionGoals.ForEach(g => g.Init(this));
        IsTracked = false;
        Completed = false;
    }

    private void OnDisable()
    {
        QuestController.OnQuestTracked -= QuestTracked;
        QuestController.OnQuestUntracked -= QuestUntracked;
        QuestController.OnQuestAbandoned -= QuestAbandoned;
    }

    public void CheckGoals()
    {
        Debug.Log("KillGoals Completed = " + KillGoals.All(g => g.Completed));
        Debug.Log("CollectionGoals Completed = " + CollectionGoals.All(g => g.Completed));

        Completed = KillGoals.All(g => g.Completed) && CollectionGoals.All(g => g.Completed);
    }

    public void GiveReward()
    {
        if (ItemReward != null)
            InventoryManager.instance.GiveItem(ItemReward);
    }

    public string GetQuestRewardString()
    {
        string rewardString = string.Empty;

        rewardString = ((ExperienceReward > 0) ? ExperienceReward + " Experience\n" : "") +
            ((MoneyReward > 0) ? MoneyReward + " Silver\n" : "") +
            ((ItemReward != null) ? ItemReward.Name : "");

        return rewardString;
    }

    public string GetQuestProgressString()
    {
        string progressString = string.Empty;

        foreach (Goal goal in KillGoals)
            progressString += ("(" + goal.CurrentAmount + "/" + goal.RequiredAmount + ") " + goal.Description + "\n");

        foreach (Goal goal in CollectionGoals)
            progressString += ("(" + goal.CurrentAmount + "/" + goal.RequiredAmount + ") " + goal.Description + "\n");

        return progressString;
    }
    

    private void QuestTracked(Quest quest)
    {
        if (quest == this)
            IsTracked = true;
    }

    private void QuestUntracked(Quest quest)
    {
        if (quest == this)
            IsTracked = false;
    }

    private void QuestAbandoned(Quest quest)
    {
        if (quest == this)
            IsTracked = false;
    }
}
