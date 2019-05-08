using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KillGoal : Goal
{
    public int EnemyID;
    
    public override void Init(Quest quest)
    {
        base.Init(quest);
        CombatEvents.OnEnemyDeath += EnemyDied;
    }

    void EnemyDied(Hostile enemy)
    {
        if (Quest.IsTracked && enemy.ID == EnemyID)
        {
            CurrentAmount++;
            //Debug.Log("KillGoal progress = " + CurrentAmount + "/" + RequiredAmount);
            Evaluate();
        }
    }

    ~KillGoal()
    {
        CombatEvents.OnEnemyDeath -= EnemyDied;
    }
}
