using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class QuestToken
{
    public string QuestName;
    public string QuestID;
    public string Description;
    public List<KillGoal> KillGoals;
    public List<CollectionGoal> CollectionGoals;
    public int ExperienceReward;
    public int MoneyReward;
    public string ItemRewardSlug;
    public bool IsTracked;
    public bool Completed;

    public QuestToken(Quest quest)
    {
        QuestName = quest.QuestName;
        QuestID = quest.QuestID;
        Description = quest.Description;
        KillGoals = quest.KillGoals;
        CollectionGoals = quest.CollectionGoals;
        ExperienceReward = quest.ExperienceReward;
        MoneyReward = quest.MoneyReward;
        ItemRewardSlug = quest.ItemRewardSlug;
        IsTracked = quest.IsTracked;
        Completed = quest.Completed;
    }
}

[Serializable]
public class Quest
{
    public string QuestName;
    public string QuestID;
    public string Description;
    public List<KillGoal> KillGoals = new List<KillGoal>();
    public List<CollectionGoal> CollectionGoals = new List<CollectionGoal>();
    public int ExperienceReward;
    public int MoneyReward;
    public string ItemRewardSlug;
    [HideInInspector] public bool IsTracked = false;
    [HideInInspector] public bool Completed = false;

    private bool idCreated = false;

    #region Constructors

    /// <summary>
    /// This constructor should only be called from the QuestDatabase.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="killGoals"></param>
    /// <param name="collectionGoals"></param>
    /// <param name="experienceReward"></param>
    /// <param name="moneyReward"></param>
    /// <param name="itemRewardSlug"></param>
    [JsonConstructor]
    public Quest(string name, string description, List<KillGoal> killGoals, List<CollectionGoal> collectionGoals, int experienceReward, int moneyReward, string itemRewardSlug)
    {
        QuestName = name;
        CreateID();
        Description = description;
        KillGoals = killGoals;
        CollectionGoals = collectionGoals;
        ExperienceReward = experienceReward;
        MoneyReward = moneyReward;
        ItemRewardSlug = itemRewardSlug;

        IsTracked = false;
        Completed = false;

        KillGoals?.ForEach(g => g.Init(this, false, 0));
        CollectionGoals?.ForEach(g => g.Init(this, false, 0));
    }

    /// <summary>
    /// This constructor is used by the save/load system, taking in a Quest Token.
    /// </summary>
    /// <param name="token"></param>
    public Quest(QuestToken token)
    {
        QuestName = token.QuestName;
        QuestID = token.QuestID;
        idCreated = true;
        Description = token.Description;
        KillGoals = token.KillGoals;
        CollectionGoals = token.CollectionGoals;
        ExperienceReward = token.ExperienceReward;
        MoneyReward = token.MoneyReward;
        ItemRewardSlug = token.ItemRewardSlug;

        IsTracked = token.IsTracked;
        Completed = token.Completed;

        InitializeQuest();

        KillGoals?.ForEach(g => g.Init(this, g.Completed, g.CurrentAmount));
        CollectionGoals?.ForEach(g => g.Init(this, g.Completed, g.CurrentAmount));
    }

    #endregion

    /// <summary>
    /// Hooks up all the listeners for Quest events
    /// </summary>
    public void InitializeQuest()
    {
        QuestController.OnQuestTracked += QuestTracked;
        QuestController.OnQuestUntracked += QuestUntracked;
        QuestController.OnQuestAbandoned += QuestAbandoned;
    }

    private void OnDisable()
    {
        QuestController.OnQuestTracked -= QuestTracked;
        QuestController.OnQuestUntracked -= QuestUntracked;
        QuestController.OnQuestAbandoned -= QuestAbandoned;
    }

    private void OnDestroy()
    {
        QuestController.OnQuestTracked -= QuestTracked;
        QuestController.OnQuestUntracked -= QuestUntracked;
        QuestController.OnQuestAbandoned -= QuestAbandoned;
    }

    public void CheckGoals()
    {
        Debug.Log("KillGoals Completed = " + KillGoals?.All(g => g.Completed));
        Debug.Log("CollectionGoals Completed = " + CollectionGoals?.All(g => g.Completed));

        Completed = ((KillGoals!=null)?KillGoals.All(g => g.Completed):true) && ((CollectionGoals!=null)?CollectionGoals.All(g => g.Completed):true);
    }

    public void GiveReward()
    {
        if (!string.IsNullOrEmpty(ItemRewardSlug))
            InventoryManager.instance.GiveItem(ItemRewardSlug);
    }

    public string GetQuestRewardString()
    {
        string rewardString = string.Empty;

        rewardString = ((ExperienceReward > 0) ? ExperienceReward + " Experience\n" : "") +
            ((MoneyReward > 0) ? MoneyReward + " Silver\n" : "") +
            ((!string.IsNullOrEmpty(ItemRewardSlug)) ? ItemDatabase.instance.GetItem(ItemRewardSlug).Name : "");

        return rewardString;
    }

    public string GetQuestProgressString()
    {
        string progressString = string.Empty;

        if (KillGoals != null)
            foreach (Goal goal in KillGoals)
                progressString += ("(" + goal.CurrentAmount + "/" + goal.RequiredAmount + ") " + goal.Description + "\n");

        if (CollectionGoals != null)
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

    /// <summary>
    /// Creates a new, unique Quest Guid.
    /// </summary>
    public void CreateID()
    {
        if (idCreated)
            return;

        QuestID = Guid.NewGuid().ToString();

        idCreated = true;
    }

    /// <summary>
    /// Compares other by its ID (Guid).
    /// Returns true if others' ID is the exact same Guid.
    /// </summary>
    /// <param name="other">Other Quest being compared to.</param>
    /// <returns>True if ID is the exact same Guid.</returns>
    public bool Equals(Quest other)
    {
        return QuestID == other.QuestID;
    }
}
