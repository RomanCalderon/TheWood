using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Transform playerHand;
    public GameObject EquippedWeapon { get; set; }
    bool canUseWeapon = true;

    Item currentlyEquippedItem;
    IWeapon equippedWeapon;
    CharacterStats characterStats;

    private void Awake()
    {
        UIEventHandler.OnUIDisplayed += UIEventHandler_OnUIDisplayed;
    }

    private void Start()
    {
        characterStats = GetComponent<Player>().CharacterStats;
    }

    private void UIEventHandler_OnUIDisplayed(bool state)
    {
        canUseWeapon = !state;
    }

    public void EquipWeapon(Item itemToEquip)
    {
        if (EquippedWeapon != null)
            UnequipWeapon();
        
        EquippedWeapon = Instantiate(Resources.Load<GameObject>("Items/" + itemToEquip.ItemSlug), playerHand.position, playerHand.rotation);
        equippedWeapon = EquippedWeapon.GetComponent<IWeapon>();
        EquippedWeapon.transform.SetParent(playerHand);
        equippedWeapon.Stats = itemToEquip.Stats;
        currentlyEquippedItem = itemToEquip;
        characterStats.AddStatBonus(itemToEquip.Stats);
        UIEventHandler.ItemEquipped(itemToEquip);
        UIEventHandler.StatsChanged();

        Debug.Log(EquippedWeapon.name + " equipped.");
    }

    public void UnequipWeapon()
    {
        InventoryManager.instance.GiveItem(currentlyEquippedItem.ItemSlug);
        characterStats.RemoveStatBonus(EquippedWeapon.GetComponent<IWeapon>().Stats);
        Destroy(EquippedWeapon);
        EquippedWeapon = null;
        equippedWeapon = null;
        UIEventHandler.StatsChanged();
        UIEventHandler.ItemUnequipped(currentlyEquippedItem);
    }

    private void Update()
    {
        UseWeapon();
    }

    void UseWeapon()
    {
        if (!canUseWeapon)
            return;

        if (Input.GetKeyDown(KeyBindings.ActionOne))
            PerformWeaponAttack();

        PerformWeaponBlock(Input.GetKey(KeyBindings.ActionTwo));
    }

    public void PerformWeaponAttack()
    {
        if (equippedWeapon == null)
            return;

        equippedWeapon.PerformAttack(GetDamage());
    }

    public void PerformWeaponBlock(bool isActive)
    {
        if (equippedWeapon == null)
            return;

        equippedWeapon.PerformBlock(isActive);
    }

    private int GetDamage()
    {
        int totalDamage = (characterStats.GetStat(BaseStat.BaseStatType.ATTACK).GetCalculatedStatValue()) + Random.Range(-1, 3);

        totalDamage += CalculateCritialDamage(totalDamage);

        //Debug.Log("Damage output: " + totalDamage);
        return totalDamage;
    }

    private int CalculateCritialDamage(int damage)
    {
        if (Random.value <= 0.1f)
            return Mathf.RoundToInt(damage * Random.Range(0.25f, 0.5f));

        return 0;
    }
}
