﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChrisTutorials.Persistent;
using UnityEditor;

[System.Serializable]
public class BlueprintSaveData
{
    [System.NonSerialized] public Blueprint blueprint;
    public string prefabPath;
    public int instanceID;

    public float[] position;
    public float[] rotation;

    public int currentProgress;
    public int requirement;
    public string buildingPrefabName;
    public string completeBuildingSoundName;

    public BlueprintSaveData(Blueprint blueprint, int id)
    {
        this.blueprint = blueprint;
        prefabPath = blueprint.prefabPath;
        instanceID = blueprint.instanceID = id;

        position = new float[3]
        {
            blueprint.transform.position.x,
            blueprint.transform.position.y,
            blueprint.transform.position.z
        };
        rotation = new float[4]
        {
            blueprint.transform.rotation.x,
            blueprint.transform.rotation.y,
            blueprint.transform.rotation.z,
            blueprint.transform.rotation.w
        };

        currentProgress = blueprint.CurrentProgress;
        requirement = blueprint.Requirement;
        buildingPrefabName = blueprint.buildingPrefab.name;
        completeBuildingSoundName = blueprint.completeBuildingSound.name;
    }
}

public class Blueprint : Interactable
{
    public string prefabPath;
    public int instanceID = -1;
    public bool modified;

    public int CurrentProgress;
    public int Requirement;
    public GameObject buildingPrefab;
    public AudioClip completeBuildingSound;
    
    /// <summary>
    /// Displays an info prompt for this Blueprint.
    /// The player can progress this Blueprint to completion or cancel/remove this Blueprint.
    /// </summary>
    public override void Preview()
    {
        if (!BuildingController.InBuildMode)
            return;

        InteractionController.PreviewInteraction(GetInteractionPreview());

        // Cancel the Blueprint
        if (Input.GetKeyUp(KeyBindings.ActionTwo))
            Destroy(gameObject);
    }

    public override void Interact()
    {
        BuildingController.RequestResources(new Resource(ItemDatabase.instance.GetItem("wood"), Requirement - CurrentProgress), CheckResourceRequest);

        HasInteracted = false;
    }

    private void CheckResourceRequest(Resource resource, int quantity)
    {
        //print(gameObject.name + " received " + quantity + " " + resource.item.Name);
        CurrentProgress += quantity;
        modified = true;

        if (CurrentProgress >= Requirement)
            CompleteBuilding();
    }

    private void CompleteBuilding()
    {
        Building b = Instantiate(buildingPrefab, transform.position, transform.rotation).GetComponent<Building>();
        b.modified = true;
        AudioManager.Instance.Play(completeBuildingSound, transform.position, 0.2f).maxDistance = 10f;
        Destroy(gameObject);
    }

    private string GetInteractionPreview()
    {
        // First add the Build blueprint option prompt
        string result = "[" + KeyBindings.GetFormat(KeyBindings.ActionOne) + "] " + GetInteractablePrompt();

        // If this blueprint is incomplete, add the progress prompt
        if (CurrentProgress < Requirement)
            result += "\n\tBUILD PROGRESS: " + CurrentProgress + "/" + Requirement;

        // Finally add the Cancel blueprint option prompt
        result += "\n[" + KeyBindings.GetFormat(KeyBindings.ActionTwo) + "] Cancel Blueprint";

        return result;
    }

    private void OnEnable()
    {
        BuildingManager.OnSave += BuildingManager_OnSave;
        BuildingManager.OnLoad += BuildingManager_OnLoad;
    }

    private void BuildingManager_OnSave()
    {
        // Add a reference of this blueprint to the BuildingManager
        // so it can be saved/loaded
        BuildingManager.AddBlueprint(this);
    }

    private void BuildingManager_OnLoad()
    {
        // If the BluidingManager already has this blueprint stored,
        // removed this duplicate
        BuildingManager.ContainsBlueprint(this, RemoveDuplicate);
    }

    private void OnDisable()
    {
        BuildingManager.OnSave -= BuildingManager_OnSave;
        BuildingManager.OnLoad -= BuildingManager_OnLoad;

        // Removes a reference of the blueprint from the BuildingManager
        // when destroyed so it wont be reloaded
        BuildingManager.RemoveBlueprint(this);
    }

    private void RemoveDuplicate()
    {
        Destroy(gameObject);
    }
}
