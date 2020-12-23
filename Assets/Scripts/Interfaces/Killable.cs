using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Killable : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 100;
    protected int Health;
    protected bool isDead = false;

    protected virtual void Start()
    {
        Health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
        Health = Mathf.Max(0, Health);

        if (Health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    protected abstract void Die();

    public int GetHealth()
    {
        return Health;
    }
}
