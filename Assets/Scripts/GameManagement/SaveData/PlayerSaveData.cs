using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int level;
    public int experience;
    public int health;
    public float[] position;

    public PlayerSaveData(Player player)
    {
        // Get player level
        level = player.PlayerLevel.Level;
        // Get player experience
        experience = player.PlayerLevel.CurrentExperience;
        // Get player health
        health = player.GetHealth();
        // Get player position
        position = new float[3]
        {
            player.transform.position.x,
            player.transform.position.y,
            player.transform.position.z,
        };
    }
}
