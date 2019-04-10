using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class BuildingManagerSaveData
{
    public List<BlueprintSaveData> blueprints;
    public List<BuildingSaveData> buildings;

    public BuildingManagerSaveData(BuildingManager buildingManager)
    {
        blueprints = buildingManager.blueprints;
        buildings = buildingManager.buildings;
    }
}

public class BuildingManager : MonoBehaviour
{
    // Saving/Loading
    public delegate void SaveLoadHandler();
    public static event SaveLoadHandler OnSave;
    public static event SaveLoadHandler OnLoad;

    // Blueprints
    public delegate void BlueprintHandler(Blueprint blueprint);
    public static event BlueprintHandler OnBlueprintAdded;
    public static event BlueprintHandler OnBlueprintRemoved;

    public delegate void ContainsBlueprintHandler(Blueprint blueprint, Action onContains);
    public static event ContainsBlueprintHandler OnContainsBlueprint;

    // Buildings
    public delegate void BuildingHandler(Building building);
    public static event BuildingHandler OnBuildingAdded;
    public static event BuildingHandler OnBuildingRemoved;

    public delegate void ContainsBuildingHandler(Building building, Action onContains);
    public static event ContainsBuildingHandler OnContainsBuilding;

    public List<BlueprintSaveData> blueprints = new List<BlueprintSaveData>();
    public List<BuildingSaveData> buildings = new List<BuildingSaveData>();


    private void Awake()
    {
        // Save/Load
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        // Blueprints
        OnBlueprintAdded += BuildingManager_OnBlueprintAdded;
        OnBlueprintRemoved += BuildingManager_OnBlueprintRemoved;
        OnContainsBlueprint += BuildingManager_OnContainsBlueprint;
        // Buildings
        OnBuildingAdded += BuildingManager_OnBuildingAdded;
        OnBuildingRemoved += BuildingManager_OnBuildingRemoved;
        OnContainsBuilding += BuildingManager_OnContainsBuilding;
    }

    #region Events

    // Blueprints
    public static void AddBlueprint(Blueprint blueprint)
    {
        OnBlueprintAdded?.Invoke(blueprint);
    }

    public static void RemoveBlueprint(Blueprint blueprint)
    {
        OnBlueprintRemoved?.Invoke(blueprint);
    }

    public static void ContainsBlueprint(Blueprint blueprint, Action onContains)
    {
        OnContainsBlueprint?.Invoke(blueprint, onContains);
    }

    // Buildings
    public static void AddBuilding(Building building)
    {
        OnBuildingAdded?.Invoke(building);
    }

    public static void RemoveBuilding(Building building)
    {
        OnBuildingRemoved?.Invoke(building);
    }

    public static void ContainsBuilding(Building building, Action onContains)
    {
        OnContainsBuilding?.Invoke(building, onContains);
    }

    #endregion


    #region Event Listeners

    // Save/Load
    private void SaveLoadController_OnSaveGame()
    {
        OnSave?.Invoke();

        SaveSystem.SaveBuildingManager(this, Application.persistentDataPath + "/buildingmanager.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        OnLoad?.Invoke();

        BuildingManagerSaveData data = SaveSystem.LoadData<BuildingManagerSaveData>(Application.persistentDataPath + "/buildingmanager.dat");

        if (data == null)
            return;

        // Set blueprints data and buildings data
        blueprints = data.blueprints;
        buildings = data.buildings;

        Transform blueprintHolder = GameObject.FindGameObjectWithTag("BlueprintHolder").transform;
        Transform buildingHolder = GameObject.FindGameObjectWithTag("BuildingHolder").transform;

        // Create blueprints from save
        foreach (BlueprintSaveData bsd in blueprints)
        {
            // Create blueprint GameObject to instantiate
            GameObject blueprintPrefab = Resources.Load(bsd.prefabPath) as GameObject;

            // Set its position and rotation
            Vector3 pos = new Vector3(bsd.position[0], bsd.position[1], bsd.position[2]);
            Quaternion rot = new Quaternion(bsd.rotation[0], bsd.rotation[1], bsd.rotation[2], bsd.rotation[3]);

            // Get its component references
            GameObject buildingPrefab = Resources.Load<GameObject>("Buildings/" + bsd.buildingPrefabName);
            AudioClip completeBuildingSound = Resources.Load<AudioClip>("Audio/Building/" + bsd.completeBuildingSoundName);

            // Instantiate the blueprint
            Blueprint blueprintInstance = Instantiate(blueprintPrefab, pos, rot, blueprintHolder).GetComponent<Blueprint>();
            blueprintInstance.instanceID = bsd.instanceID;
            blueprintInstance.modified = true;
            blueprintInstance.CurrentProgress = bsd.currentProgress;
            blueprintInstance.Requirement = bsd.requirement;
            blueprintInstance.buildingPrefab = buildingPrefab;
            blueprintInstance.completeBuildingSound = completeBuildingSound;
        }

        // Create buildings from save
        foreach (BuildingSaveData bsd in buildings)
        {
            // Create building GameObject to instantiate
            GameObject buildingPrefab = Resources.Load(bsd.prefabPath) as GameObject;

            // Set its position and rotation
            Vector3 pos = new Vector3(bsd.position[0], bsd.position[1], bsd.position[2]);
            Quaternion rot = new Quaternion(bsd.rotation[0], bsd.rotation[1], bsd.rotation[2], bsd.rotation[3]);
            
            // Get its component references
            AudioClip[] destroyBuildingSounds = Resources.LoadAll<AudioClip>("Audio/Building/DestroyBuildingSounds");
            GameObject destructionPrefab = Resources.Load<GameObject>("Buildings/" + bsd.destructionPrefabName);

            // Instantiate the building
            Building buildingInstance = Instantiate(buildingPrefab, pos, rot, buildingHolder).GetComponent<Building>();
            buildingInstance.instanceID = bsd.instanceID;
            buildingInstance.modified = true;
            buildingInstance.destroyBuildingSounds = destroyBuildingSounds;
            buildingInstance.destructionPrefab = destructionPrefab;
        }
    }

    // Blueprints
    private void BuildingManager_OnBlueprintAdded(Blueprint blueprint)
    {
        if (blueprint != null && !ContainsBlueprint(blueprint) && blueprint.modified)
            blueprints.Add(new BlueprintSaveData(blueprint, blueprints.Count));
        else if (ContainsBlueprint(blueprint))
            // Update the duplicate bsd with it's current values
            blueprints[IndexOfBlueprint(blueprint.instanceID)] = new BlueprintSaveData(blueprint, blueprint.instanceID);
        else if (blueprint == null)
            Debug.LogError("Blueprint is null");
        else
        {
            //Debug.Log("Already have a reference to [" + blueprint.name + "] ID: " + blueprint.instanceID);
        }
    }

    private void BuildingManager_OnBlueprintRemoved(Blueprint blueprint)
    {
        if (blueprint != null && ContainsBlueprint(blueprint))
            blueprints.Remove(GetBlueprintData(blueprint));
    }

    private void BuildingManager_OnContainsBlueprint(Blueprint blueprint, Action onContains)
    {
        if (!blueprint.modified && ContainsBlueprint(blueprint))
        {
            print("Destroy pre-existing blueprint and its savedata");
            blueprints.Remove(GetBlueprintData(blueprint));
            onContains();
        }
    }

    // Buildings
    private void BuildingManager_OnBuildingAdded(Building building)
    {
        if (building != null && !ContainsBuilding(building) && building.modified)
            buildings.Add(new BuildingSaveData(building, buildings.Count));
        else if (building == null)
            Debug.LogError("Building is null");
        else
        {
            //Debug.Log("Already have a reference to [" + building.name + "] ID: " + building.instanceID);
        }
    }

    private void BuildingManager_OnBuildingRemoved(Building building)
    {
        if (building != null && ContainsBuilding(building))
            buildings.Remove(GetBuildingData(building));
    }

    private void BuildingManager_OnContainsBuilding(Building building, Action onContains)
    {
        if (!building.modified && ContainsBuilding(building))
        {
            print("Destroy pre-existing building and its savedata");
            buildings.Remove(GetBuildingData(building));
            onContains();
        }
    }

    #endregion


    private bool ContainsBlueprint(Blueprint blueprint)
    {
        return (blueprints.Find(b => b.instanceID == blueprint.instanceID) != null);
    }

    private BlueprintSaveData GetBlueprintData(Blueprint blueprint)
    {
        return blueprints.Find(bsd => bsd.instanceID == blueprint.instanceID);
    }

    private bool ContainsBuilding(Building building)
    {
        return (buildings.Find(b => b.instanceID == building.instanceID) != null);
    }

    private BuildingSaveData GetBuildingData(Building building)
    {
        return buildings.Find(bsd => bsd.instanceID == building.instanceID);
    }

    private int IndexOfBlueprint(int instanceId)
    {
        return blueprints.IndexOf(blueprints.Find(bsd => bsd.instanceID == instanceId));
    }

    //public static void Replace<T>(ref IList<T> list, T oldItem, T newItem)
    //{
    //    int index = 0;

    //    if (list.Contains(oldItem))
    //    {
    //        index = list.IndexOf(oldItem);
    //        list.Remove(oldItem);
    //        list.Insert(index, newItem);
    //    }
    //}
}
