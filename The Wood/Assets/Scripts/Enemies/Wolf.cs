using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Hostile
{
    protected override void Start()
    {
        base.Start();

        characterStats = new CharacterStats(12, 10);
        ID = 0;
        ExperienceReward = 60;
        DropTable = new DropTable
        {
            loot = new List<LootDrop>
            {
                new LootDrop("torch", 2),
                new LootDrop("potion_log", 90)
            }
        };
    }
    
}
