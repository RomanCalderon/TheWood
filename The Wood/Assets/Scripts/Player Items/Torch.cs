using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour, IWeapon
{
    private Animator animator;
    public List<BaseStat> Stats { get; set; }
    public CharacterStats CharacterStats { get; set; }
    public int CurrentDamage { get; set; }

    [SerializeField]
    Light torchLightSource;
    [SerializeField]
    Vector2 flickerRange = new Vector2(0.3f, 1.2f);
    [SerializeField]
    float strength = 4f;
    [SerializeField]
    float rateDampening = 0.1f;
    private float baseIntensity;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        baseIntensity = torchLightSource.intensity;
        StartCoroutine(DoFlicker());
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
        if (hit.transform.tag == "Enemy")
            hit.GetComponent<Killable>().TakeDamage(CurrentDamage);
    }

    private IEnumerator DoFlicker()
    {
        while (true)
        {
            torchLightSource.intensity = Mathf.Lerp(torchLightSource.intensity, Random.Range(flickerRange.x, flickerRange.y), strength * Time.deltaTime);
            yield return new WaitForSeconds(rateDampening);
        }
    }
}
