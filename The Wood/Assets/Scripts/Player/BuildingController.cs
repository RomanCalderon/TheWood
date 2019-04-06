using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource
{
    public Item item;
    public int quantity;

    public Resource(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public bool Equals(Resource other)
    {
        return other.item.ItemSlug == item.ItemSlug;
    }
}

public class BuildingController : MonoBehaviour
{
    public delegate void ResourceRequestHandler(Resource resource, Action<Resource, int> onValidate);
    public static event ResourceRequestHandler OnRequestResources;

    [SerializeField] Transform playerCamera;
    [SerializeField] LayerMask validBlueprintLayerMask;

    [SerializeField] List<Resource> resources;

    [SerializeField] List<Blueprint> blueprints = new List<Blueprint>();

    public static bool blueprintPreviewMode;
    private Blueprint blueprintToPlace;
    private Blueprint blueprintInstance;


    private void Awake()
    {
        UIEventHandler.OnItemAddedToInventory += AddResource;
        UIEventHandler.OnItemRemovedFromInventory += RemoveResource;
        OnRequestResources += ValidateResourceRequest;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PreviewBlueprint(blueprintPreviewMode = !blueprintPreviewMode, blueprints[0]);
        }

        if (blueprintPreviewMode)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            Quaternion blueprintRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);

            if (Physics.Raycast(ray, out RaycastHit hit, 6f, validBlueprintLayerMask))
            {
                // Recreate blueprintInstance if blueprintToPlace is null
                if (blueprintInstance == null)
                    PreviewBlueprint(true, blueprintToPlace);

                if (!blueprintInstance.gameObject.activeSelf)
                    blueprintInstance.gameObject.SetActive(true);

                // Display a preview of where this blueprint will be placed
                if (blueprintInstance != null)
                {
                    blueprintInstance.transform.position = hit.point;
                    blueprintInstance.transform.rotation = blueprintRotation;

                    // Place blueprintToPlace
                    if (Input.GetKeyDown(/*FOR TESTING*/KeyCode.P))
                    {
                        PlaceBlueprint(hit.point, blueprintRotation);
                    }
                }
            }
            else
                blueprintToPlace.gameObject.SetActive(false);
        }
    }

    private void PreviewBlueprint(bool previewMode, Blueprint blueprintReference)
    {
        if (previewMode)
        {
            blueprintToPlace = blueprintInstance = Instantiate(blueprintReference.gameObject).GetComponent<Blueprint>();
            blueprintToPlace.gameObject.name = blueprintInstance.gameObject.name = blueprintReference.gameObject.name;
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

    private void PlaceBlueprint(Vector3 pos, Quaternion rot)
    {
        blueprintInstance.generated = true;
        blueprintInstance = null;
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
                if (r.item.ItemSlug == item.ItemSlug)
                {
                    r.quantity++;
                    groupFound = true;
                    break;
                }
            }

            // If a group of similar type does not exist, create a new group
            // and add this item to it
            if (!groupFound)
                resourceGroups.Add(new Resource(item, 1));
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
            if (r.item.ItemSlug == item.ItemSlug)
            {
                r.quantity++;
                groupFound = true;
                return;
            }
        }

        // If a group of similar type does not exist, create a new group
        // and add this item to it
        if (!groupFound)
            resources.Add(new Resource(item, 1));
    }

    private void RemoveResource(Item item)
    {
        if (item.ItemType != ItemType.RESOURCE)
            return;

        // Check if there's a resource group with this item
        foreach (Resource r in resources)
        {
            // If a group of similar item type exists, subtract 1 from it's quantity
            if (r.item.ItemSlug == item.ItemSlug)
            {
                r.quantity--;

                // Remove this resource if quantity is 0
                if (r.quantity <= 0)
                    resources.Remove(r);

                return;
            }
        }
    }

    public static void RequestResources(Resource resource, Action<Resource, int> onValidate)
    {
        OnRequestResources?.Invoke(resource, onValidate);
    }

    public void ValidateResourceRequest(Resource requestedResource, Action<Resource, int> onValidate)
    {
        if (requestedResource != null)
        {
            foreach (Resource r in resources)
            {
                if (r.Equals(requestedResource))
                {
                    int providedAmount = Mathf.Min(requestedResource.quantity, r.quantity);
                    onValidate(r, providedAmount);
                    StartCoroutine(RemoveItemEnum(r.item, providedAmount));

                    return;
                }
            }
            Debug.Log("Player doesn't have requested resource " + requestedResource.item.Name);
        }
        else
            Debug.LogError("Requested resource is null");
    }

    private IEnumerator RemoveItemEnum(Item item, int iterations = 1)
    {
        for (int i = 0; i < iterations; i++)
        {
            InventoryManager.instance.RemoveItem(item);
            yield return null;
        }
    }

    #endregion
}
