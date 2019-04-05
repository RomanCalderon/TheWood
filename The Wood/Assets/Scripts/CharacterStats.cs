using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    public List<BaseStat> stats = new List<BaseStat>();

    public CharacterStats(int attack, int defense)
    {
        stats = new List<BaseStat>()
        {
            new BaseStat(BaseStat.BaseStatType.ATTACK, attack, "Attack"),
            new BaseStat(BaseStat.BaseStatType.DEFENSE, defense, "Defense")
        };
    }

    public BaseStat GetStat(BaseStat.BaseStatType stat)
    {
        return stats.Find(x => x.StatType == stat);
    }

    public void AddStatBonus(List<BaseStat> statBonuses)
    {
        foreach (BaseStat statBonus in statBonuses)
            GetStat(statBonus.StatType).AddStatBonus(new StatBonus(statBonus.BaseValue));
    }

    public void RemoveStatBonus(List<BaseStat> statBonuses)
    {
        foreach (BaseStat statBonus in statBonuses)
            GetStat(statBonus.StatType).RemoveStatBonus(new StatBonus(statBonus.BaseValue));
    }
}
