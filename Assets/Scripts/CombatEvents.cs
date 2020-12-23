using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvents : MonoBehaviour
{
    public delegate void EnemyEventHandler(Hostile enemy);
    public static event EnemyEventHandler OnEnemyDeath;

    public static void EnemyDied(Hostile enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }
}
