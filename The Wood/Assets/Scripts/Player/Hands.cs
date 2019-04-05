using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hands : MonoBehaviour, IHands
{
    private Animator animator;
    public List<BaseStat> Stats { get; set; }

    public Item heldItem { get; set; }
    public int CurrentDamage { get; set; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PerformAttack(int damage)
    {
        animator.SetTrigger("Punch");
    }

    public void PerformBlock(bool isActive)
    {
        animator.SetTrigger("Block");
    }
}
