using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTool : MonoBehaviour, ITool, IWeapon
{
    private Animator animator;

    public event ToolActionHandler OnPerformAction;

    public List<BaseStat> Stats { get; set; }
    public CharacterStats CharacterStats { get; set; }
    public int CurrentDamage { get; set; }
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        print("Perform Tool Action.");
        OnPerformAction?.Invoke();
    }
}
