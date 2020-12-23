using System.Collections;
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

    public List<RequiredResource> resources;
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

        resources = blueprint.resources;
        buildingPrefabName = blueprint.buildingPrefab.name;
        completeBuildingSoundName = blueprint.completeBuildingSound.name;
    }
}

[System.Serializable]
public class RequiredResource
{
    public string ItemSlug;
    public int CurrentAmount;
    public int RequiredAmount;

    public bool IsSufficient()
    {
        return (CurrentAmount >= RequiredAmount);
    }
}

public class Blueprint : Interactable
{
    public Sprite Icon;
    public string prefabPath;
    public int instanceID = -1;
    public bool modified;

    public List<RequiredResource> resources = new List<RequiredResource>();

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
        {
            // Provide all Resources used for this Blueprint
            BuildingController.AddResources(GetResources(resources));

            // Destroy this gameObject
            Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        BuildingController.RequestResources(resources, CheckResourceRequest);

        HasInteracted = false;
    }

    private void CheckResourceRequest(string resourceSlug, int quantity)
    {
        RequiredResource rr = resources.Find(r => r.ItemSlug == resourceSlug);
        rr.CurrentAmount += quantity;
        modified = true;

        // Check if all the requirements are met
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        foreach (RequiredResource rr in resources)
            if (!rr.IsSufficient())
                return; // This resource requirement is not complete
        
        // Called if all resource requirements are complete
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
        foreach (RequiredResource rr in resources)
        {
            if (rr.IsSufficient())
                result += "\n\t\t<color=green>" + rr.ItemSlug.ToUpper() + "</color>";
            else
                result += "\n\t\t" + rr.ItemSlug.ToUpper() + " " + rr.CurrentAmount + "/" + rr.RequiredAmount;
        }

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

    private List<Resource> GetResources(List<RequiredResource> rrList)
    {
        List<Resource> resources = new List<Resource>();

        foreach (RequiredResource rr in rrList)
        {
            resources.Add(new Resource(rr.ItemSlug, rr.CurrentAmount));
        }

        return resources;
    }
}
