using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTool : MonoBehaviour, ITool, IWeapon
{
    private Animator animator;

    public List<BaseStat> Stats { get; set; }
    public CharacterStats CharacterStats { get; set; }
    public int CurrentDamage { get; set; }
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //BuildingController.InBuildMode = true;
        //print("BuildingController.InBuildMode = " + BuildingController.InBuildMode);
    }

    private void OnDisable()
    {
        //BuildingController.InBuildMode = false;
        //print("BuildingController.InBuildMode = " + BuildingController.InBuildMode);
    }

    public void PerformAttack(int damage)
    {
        CurrentDamage = damage;
        animator.SetTrigger("BuildTool_Action");
    }

    public void PerformBlock(bool isActive)
    {
        if (BuildingController.InBuildMode)
            return;

        animator.SetBool("BuildTool_Block", isActive);
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.tag == "Enemy")
            hit.GetComponent<Killable>().TakeDamage(CurrentDamage);
    }

    /// <summary>
    /// Called when the BuildTool_Action animation reaches the event trigger,
    /// at the point of impact.
    /// </summary>
    public void PerformActionEvent()
    {
        Interactable interactableObject = InteractionController.instance.GetBlueprint();
        Building buildingObject = InteractionController.instance.GetBuilding();
        
        // If the player is InBlueprintMode and hit a Blueprint
        if (BuildingController.InBuildMode && interactableObject != null)
            interactableObject.Interact();  // Interacts with the Blueprint

        // If the player is InBuildMode and hit a Building
        if (BuildingController.InBuildMode && buildingObject != null)
            buildingObject.TakeDamage(CurrentDamage);   // Deals damage to the Building
    }
}
