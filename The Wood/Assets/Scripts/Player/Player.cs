using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(BuildingController))]
public class Player : Killable
{
    public CharacterStats CharacterStats;
    [SerializeField]
    RigidbodyFirstPersonController playerMovementController;
    
    public PlayerLevel PlayerLevel { get; set; }

    private void Awake()
    {
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        UIEventHandler.OnUIDisplayed += UIEventHandler_OnUIDisplayed;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        PlayerLevel = GetComponent<PlayerLevel>();
        CharacterStats = new CharacterStats(4, 2);
        
        UIEventHandler.HealthChanged(Health, maxHealth);
        SetCursorActive(false);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        UIEventHandler.HealthChanged(Health, maxHealth);
    }

    protected override void Die()
    {
        Debug.Log("Player dead. Reset health");
        Health = maxHealth;
        isDead = false;
        UIEventHandler.HealthChanged(Health, maxHealth);
    }

    #region Event Listeners

    private void UIEventHandler_OnUIDisplayed(bool state)
    {
        SetCursorActive(state);
        SetControlState(!state);
    }

    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SavePlayer(this, Application.persistentDataPath + "/player.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        PlayerSaveData data = SaveSystem.LoadData<PlayerSaveData>(Application.persistentDataPath + "/player.dat");

        if (data == null)
            return;

        // Set level
        PlayerLevel.Level = data.level;
        // Set experience
        PlayerLevel.GrantExperience(data.experience);
        // Set health
        Health = data.health;
        UIEventHandler.HealthChanged(Health, maxHealth);
        // Set position
        Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.position = position;

        //Debug.Log("Loaded player data.");
    }

    #endregion

    /// <summary>
    /// Sets the player controllability state
    /// </summary>
    /// <param name="state">Can the player be controlled?</param>
    public void SetControlState(bool state)
    {
        playerMovementController.movementSettings.LockMovement = !state;

        //playerCameraController.lockCamera = !state;
        //playerMovementController.keepDirection = state;
        //playerMovementController.lockMovement = !state;
        //playerMovementController.input = Vector2.zero;
        //playerMovementController.Sprint(false);
        //playerMovementController.enabled = state;
    }

    /// <summary>
    /// Sets the cursor controllability state
    /// </summary>
    /// <param name="state">Is the cursor visible and unlocked?</param>
    public static void SetCursorActive(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = (state) ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
