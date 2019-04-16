using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ChrisTutorials.Persistent;

[System.Serializable]
public class BuildingSaveData
{
    [System.NonSerialized] public Building building;
    public string prefabPath;
    public int instanceID;

    public float[] position;
    public float[] rotation;
    
    public string destructionPrefabName;

    public BuildingSaveData(Building building, int id)
    {
        this.building = building;
        instanceID = building.instanceID = id;
        prefabPath = building.prefabPath;

        destructionPrefabName = building.destructionPrefab.name;

        position = new float[3]
        {
            building.transform.position.x,
            building.transform.position.y,
            building.transform.position.z
        };
        rotation = new float[4]
        {
            building.transform.rotation.x,
            building.transform.rotation.y,
            building.transform.rotation.z,
            building.transform.rotation.w
        };
    }
}

public class Building : Killable
{
    public string prefabPath;
    [HideInInspector] public int instanceID = -1;
    [HideInInspector] public bool modified;

    public List<Resource> resources;

    [Header("Destroy Building")]
    public AudioClip[] destroyBuildingSounds;
    public GameObject destructionPrefab;

    protected override void Die()
    {
        AudioManager.Instance.Play(destroyBuildingSounds[Random.Range(0, destroyBuildingSounds.Length)], transform.position, 0.15f).maxDistance = 25f;

        // FIXME: Do something about the "broken building" objects
        if (destructionPrefab != null)
            Instantiate(destructionPrefab, transform.position, transform.rotation);

        // Drop the Resources used for this Building
        foreach (Resource r in resources)
            for (int i = 0; i < r.quantity; i++)
                ItemDatabase.instance.DropLoot(r.itemSlug, transform.position + Vector3.up * 1);

        Destroy(gameObject);
        return;
    }

    private void Awake()
    {
        BuildingManager.OnSave += BuildingManager_OnSave;
        BuildingManager.OnLoad += BuildingManager_OnLoad;
    }

    private void BuildingManager_OnSave()
    {
        // Add a reference of this building to the BuildingManager
        // so it can be saved/loaded
        BuildingManager.AddBuilding(this);
    }

    private void BuildingManager_OnLoad()
    {
        // If the BuildingManager already has this building stored,
        // removed this duplicate from the scene
        BuildingManager.ContainsBuilding(this, RemoveDuplicate);
    }

    private void OnDisable()
    {
        BuildingManager.OnSave -= BuildingManager_OnSave;
        BuildingManager.OnLoad -= BuildingManager_OnLoad;
        
        // Removes a reference of the building from the BuildingManager
        // when destroyed so it wont be reloaded
        BuildingManager.RemoveBuilding(this);
    }

    private void RemoveDuplicate()
    {
        Destroy(gameObject);
    }
}
