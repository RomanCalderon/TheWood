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
        BuildingController.InBuildMode = true;
        print("BuildingController.InBuildMode = " + BuildingController.InBuildMode);
    }

    private void OnDisable()
    {
        BuildingController.InBuildMode = false;
        print("BuildingController.InBuildMode = " + BuildingController.InBuildMode);
    }

    public void PerformAttack(int damage)
    {
        CurrentDamage = damage;
        animator.SetTrigger("BuildTool_Action");
    }

    public void PerformBlock(bool isActive)
    {
        animator.SetBool("BuildTool_Block", isActive);
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.tag == "Enemy")
            hit.GetComponent<Killable>().TakeDamage(CurrentDamage);
    }

    public void PerformAction()
    {
        animator.SetTrigger("BuildTool_Action");
    }

    public void PerformActionEvent()
    {
        Interactable interactableObject = InteractionController.instance.GetInteractable();

        if (interactableObject == null)
            return;

        if (interactableObject is Blueprint)
            interactableObject.Interact();
    }
}
