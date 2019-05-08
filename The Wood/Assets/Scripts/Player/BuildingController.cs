using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource
{
    public string itemSlug;
    public int quantity;

    public Resource(string itemSlug, int quantity)
    {
        this.itemSlug = itemSlug;
        this.quantity = quantity;
    }

    public bool Equals(Resource other)
    {
        return other.itemSlug == itemSlug;
    }
}

public class BuildingController : MonoBehaviour
{
    public enum Modes
    {
        NONE,
        BLUEPRINT,
        BUILD
    }

    public delegate void BuildModeStateHandler();
    public static event BuildModeStateHandler OnEnabledBuildMode;
    public static event BuildModeStateHandler OnDisabledBuildMode;

    public delegate void BuildModeHandler(Blueprint blueprint);
    public static event BuildModeHandler OnSelectedBlueprint;
    public delegate void ResourceRequestHandler(List<RequiredResource> requiredResources, Action<string, int> onValidate);
    public static event ResourceRequestHandler OnRequestResources;

    public delegate void ResourcesHandler(List<Resource> resources);
    public static event ResourcesHandler OnAddResources;

    [SerializeField] Transform playerCamera;
    [SerializeField] LayerMask validLayerMask;
    int terrainLayer;

    [SerializeField] List<Resource> resources;

    [SerializeField] List<Blueprint> blueprints = new List<Blueprint>();
    
    public static Modes CurrentMode { get; private set; }
    public static bool InBuildMode { get; private set; }
    public static bool InBlueprintMode { get; private set; }

    private Blueprint blueprintToPlace;
    private Blueprint blueprintInstance;
    private string originalLayerName;

    [Header("UI")]
    [SerializeField] RectTransform blueprintUIHolder;
    [SerializeField] GameObject blueprintUIButtonPrefab;


    private void Awake()
    {
        // Resources
        InventoryManager.OnItemListUpdated += InventoryManager_OnItemListUpdated;

        OnRequestResources += ValidateResourceRequest;
        OnAddResources += BuildingController_OnAddResources;

        // Blueprints
        OnSelectedBlueprint += BuildingController_OnSelectedBlueprint;
    }

    private void OnDestroy()
    {
        // Resources
        InventoryManager.OnItemListUpdated -= InventoryManager_OnItemListUpdated;

        OnRequestResources -= ValidateResourceRequest;
        OnAddResources -= BuildingController_OnAddResources;

        // Blueprints
        OnSelectedBlueprint -= BuildingController_OnSelectedBlueprint;
    }

    private void Start()
    {
        // Set to the Terrain's layer
        terrainLayer = LayerMask.NameToLayer("Terrain");

        // Setup the UI
        foreach (Blueprint blueprint in blueprints)
        {
            BlueprintUIButton blueprintUI = Instantiate(blueprintUIButtonPrefab, blueprintUIHolder).GetComponent<BlueprintUIButton>();
            blueprintUI.Initialize(blueprint, this);
        }
    }

    private void Update()
    {
        BlueprintMode();
        BuildMode();

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }
    }

    private void ToggleBuildMode()
    {
        SetMode((InBuildMode) ? Modes.NONE : Modes.BUILD);
    }

    // Sets the new mode (None/Blueprint/Build)
    public static void SetMode(Modes newMode)
    {
        //print("Switch from [" + CurrentMode + "] to [" + newMode + "]");

        // Unequip the Build Tool if player is switching from Build Mode to Blueprint Mode
        if (CurrentMode == Modes.BUILD && newMode == Modes.BLUEPRINT)
        {
            InventoryManager.instance.UnequipBuildTool();
        }

        // If build mode is switched off, invoke the OnDisabledBuildMode event
        if (CurrentMode == Modes.BUILD && newMode != Modes.BUILD)
            OnDisabledBuildMode?.Invoke();

        switch (newMode)
        {
            case Modes.NONE:
                InBlueprintMode = InBuildMode = false;
                InventoryManager.instance.UnequipBuildTool();
                break;
            case Modes.BLUEPRINT:
                InBuildMode = false;
                InBlueprintMode = true;
                break;
            case Modes.BUILD:
                InBlueprintMode = false;
                InBuildMode = true;
                InventoryManager.instance.EquipBuildTool();
                OnEnabledBuildMode?.Invoke();
                break;
            default:
                Debug.LogError("No case for new mode: " + newMode);
                break;
        }
        CurrentMode = newMode;
        //print("New mode: " + CurrentMode.ToString());
    }

    #region Blueprint Mode

    private void BlueprintMode()
    {
        if (InBlueprintMode)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            Quaternion blueprintRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);

            if (Physics.Raycast(ray, out RaycastHit hit, 6f, validLayerMask))
            {
                // Create blueprintInstance if it's null
                if (blueprintInstance == null)
                {
                    if (blueprintToPlace != null)
                        PreviewBlueprint(blueprintToPlace);
                    else
                        return;
                }

                // If the raycast hit is not on the Terrain, return
                if (hit.transform.gameObject.layer != terrainLayer)
                {
                    blueprintToPlace.gameObject.SetActive(false);
                    return;
                }

                // Enable blueprintInstance if it's disabled
                if (!blueprintInstance.gameObject.activeSelf)
                    blueprintInstance.gameObject.SetActive(true);

                // Display a preview of where this blueprint will be placed
                if (blueprintInstance != null && blueprintInstance.gameObject.activeSelf)
                {
                    blueprintInstance.transform.position = hit.point;
                    blueprintInstance.transform.rotation = blueprintRotation;

                    // Place blueprintInstance
                    if (Input.GetKeyDown(KeyBindings.ActionOne))
                    {
                        PlaceBlueprint(hit.point, blueprintRotation);
                    }
                }
            }
            else
            {
                // Hide the blueprint if it's in an invalid location
                if (blueprintToPlace != null)
                    blueprintToPlace.gameObject.SetActive(false);
            }
            
            // Cancel Blueprint Mode with ActionTwo
            if (Input.GetKeyDown(KeyBindings.ActionTwo))
            {
                CancelBlueprintMode();
            }
        }
        else
        {
            if (blueprintInstance != null)
            {
                Destroy(blueprintInstance.gameObject);
                blueprintToPlace = blueprintInstance = null;
            }
        }
    }

    public void PreviewBlueprint(Blueprint blueprintReference)
    {
        if (InBlueprintMode)
        {
            originalLayerName = LayerMask.LayerToName(blueprintReference.gameObject.layer);

            blueprintInstance = Instantiate(blueprintReference.gameObject, GameObject.FindGameObjectWithTag("BlueprintHolder").transform).GetComponent<Blueprint>();
            blueprintInstance.gameObject.name = blueprintReference.gameObject.name;
            blueprintInstance.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            blueprintToPlace = blueprintInstance;
        }
        else
        {
            // Stop blueprint preview mode
            if (blueprintInstance != null)
            {
                Destroy(blueprintInstance.gameObject);
                blueprintToPlace = blueprintInstance = null;
            }
        }
    }

    public void CancelBlueprintMode()
    {
        SetMode(Modes.NONE);

        // Stop blueprint preview mode
        if (blueprintInstance != null)
        {
            Destroy(blueprintInstance.gameObject);
            blueprintToPlace = blueprintInstance = null;
        }
    }

    private void PlaceBlueprint(Vector3 pos, Quaternion rot)
    {
        blueprintInstance.gameObject.SetActive(true);
        blueprintInstance.modified = true;
        blueprintInstance.gameObject.layer = LayerMask.NameToLayer(originalLayerName);
        blueprintInstance = null;
    }

    #endregion

    #region Build Mode

    private void BuildMode()
    {
        if (InBuildMode)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 6f, validLayerMask))
            {
                if (Input.GetKeyDown(KeyBindings.ActionOne))
                {

                }
            }
        }
    }

    //private void CancelBuildMode()
    //{
    //    SetMode(Modes.NONE);
    //}

    #endregion

    // Events
    public static void SelectedBlueprint(Blueprint blueprint)
    {
        OnSelectedBlueprint?.Invoke(blueprint);
    }

    public static void AddResources(List<Resource> resources)
    {
        OnAddResources?.Invoke(resources);
    }

    // Event Listeners
    private void BuildingController_OnSelectedBlueprint(Blueprint blueprint)
    {
        // When the player selects a Blueprint UI, enable Blueprint Mode
        SetMode(Modes.BLUEPRINT);

        blueprintToPlace = blueprint;
    }

    private void BuildingController_OnAddResources(List<Resource> resources)
    {
        if (resources == null)
            return;

        // Add more resources
        foreach (Resource r in resources)
        {
            InventoryManager.instance.GiveItem(r.itemSlug, r.quantity);
        }
    }
    
    private void InventoryManager_OnItemListUpdated(List<Item> updatedList)
    {
        resources.Clear();

        // Update resources List with all Resources from updated list (playerItems)
        foreach (Item item in updatedList)
            AddResource(item);
    }


    #region Resource Management

    void FindResourcesFromInventory()
    {
        List<Item> resourceItems = InventoryManager.instance.GetItemsOfType(ItemType.RESOURCE);
        List<Resource> resourceGroups = new List<Resource>();

        // For each item in the inventory
        foreach (Item item in resourceItems)
        {
            bool groupFound = false;

            // Check if there's a resource group with this item
            foreach (Resource r in resourceGroups)
            {
                // If a group of similar item type exists, add 1 to it's quantity
                // and move on to the next item in the inventory
                if (r.itemSlug == item.ItemSlug)
                {
                    r.quantity++;
                    groupFound = true;
                    break;
                }
            }

            // If a group of similar type does not exist, create a new group
            // and add this item to it
            if (!groupFound)
                resourceGroups.Add(new Resource(item.ItemSlug, 1));
        }

        resources = resourceGroups;
    }

    private void AddResource(Item item)
    {
        if (item.ItemType != ItemType.RESOURCE)
            return;

        bool groupFound = false;

        // Check if there's a resource group with this item
        foreach (Resource r in resources)
        {
            // If a group of similar item type exists, add 1 to it's quantity
            if (r.itemSlug == item.ItemSlug)
            {
                r.quantity++;
                groupFound = true;
                return;
            }
        }

        // If a group of similar type does not exist, create a new group
        // and add this item to it
        if (!groupFound)
            resources.Add(new Resource(item.ItemSlug, 1));
    }

    private void RemoveResource(Item item)
    {
        if (item.ItemType != ItemType.RESOURCE)
            return;

        // Check if there's a resource group with this item
        foreach (Resource r in resources)
        {
            // If a group of similar item type exists, subtract 1 from it's quantity
            if (r.itemSlug == item.ItemSlug)
            {
                r.quantity--;

                // Remove this resource if quantity is 0
                if (r.quantity <= 0)
                    resources.Remove(r);

                return;
            }
        }
    }

    public static void RequestResources(List<RequiredResource> resource, Action<string, int> onValidate)
    {
        // Calls ValidateResourcesRequest()
        OnRequestResources?.Invoke(resource, onValidate);
    }

    public void ValidateResourceRequest(List<RequiredResource> requestedResources, Action<string, int> onValidate)
    {
        if (requestedResources != null && requestedResources.Count > 0)
        {
            // For each RequiredResource
            foreach (RequiredResource rr in requestedResources)
            {
                // Search the player's Resource collection for that Resource
                foreach (Resource r in resources)
                {
                    if (r.itemSlug == rr.ItemSlug)
                    {
                        int amountNeeded = rr.RequiredAmount - rr.CurrentAmount;
                        int providedAmount = Mathf.Min(amountNeeded, r.quantity);

                        onValidate(r.itemSlug, providedAmount);

                        // Remove the proper amount to Resources provided
                        StartCoroutine(RemoveItemEnum(r.itemSlug, providedAmount));

                        break;
                    }
                }
            }
        }
        else
            Debug.LogError("Requested resource is null");
    }

    private IEnumerator RemoveItemEnum(string itemSlug, int iterations = 1)
    {
        for (int i = 0; i < iterations; i++)
        {
            InventoryManager.instance.RemoveItem(itemSlug);
            yield return null;
        }
    }

    #endregion
}
