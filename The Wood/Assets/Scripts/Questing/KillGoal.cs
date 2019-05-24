using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KillGoal : Goal
{
    public int EnemyID;

    public KillGoal(Quest quest) : base(quest) { }

    public override void Init(Quest quest, bool completed, int currentAmount)
    {
        Quest = quest;

        Completed = completed;
        CurrentAmount = currentAmount;

        CombatEvents.OnEnemyDeath += EnemyDied;
    }

    void EnemyDied(Hostile enemy)
    {
        if (Quest.IsTracked && enemy.ID == EnemyID)
        {
            CurrentAmount++;
            Debug.Log("KillGoal progress = " + CurrentAmount + "/" + RequiredAmount);
            Evaluate();
        }
    }

    ~KillGoal()
    {
        CombatEvents.OnEnemyDeath -= EnemyDied;
    }
}
