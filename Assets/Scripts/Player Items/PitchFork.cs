using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchFork : MonoBehaviour, ITool, IWeapon
{
    private Animator animator;

    public List<BaseStat> Stats { get; set; }
    public int CurrentDamage { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PerformAttack(int damage)
    {
        CurrentDamage = damage;
        animator.SetTrigger("Base_Attack");
    }

    public void PerformBlock(bool isActive)
    {
        animator.SetBool("Base_Block", isActive);
    }

    void OnTriggerEnter(Collider hit)
    {
        // Ignore player hits
        if (hit.transform.CompareTag("Player"))
            return;

        if (hit.transform.CompareTag("Enemy"))
            hit.GetComponent<Killable>().TakeDamage(CurrentDamage);

        if (hit.transform.CompareTag("Building"))
            hit.GetComponent<Killable>().TakeDamage(CurrentDamage);
    }

    public void PerformAction()
    {
        animator.SetTrigger("PitchFork_Action");
    }

    public void PerformActionEvent()
    {
        print("Perform Tool Action.");
    }
}
