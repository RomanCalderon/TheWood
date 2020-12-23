using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private Text healthText, levelText;
    [SerializeField] private Image healthFill, levelFill;
    [SerializeField] private Player player;
    [SerializeField] private Button unequipWeaponButton;

    // Stats
    private List<Text> playerStatTexts = new List<Text>();
    [SerializeField] private Text playerStatPrefab;
    [SerializeField] private Transform playerStatPanel;

    // Equipped Weapon
    [SerializeField] private Sprite defaultWeaponSprite;
    private PlayerWeaponController playerWeaponController;
    [SerializeField] private Text weaponStatPrefab;
    [SerializeField] private Transform weaponStatPanel;
    [SerializeField] private Text weaponNameText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private List<Text> weaponStatTexts = new List<Text>();

    private void Awake()
    {
        UIEventHandler.OnPlayerHealthChanged += UpdateHealth;
        UIEventHandler.OnStatsChanged += UpdateStats;
        UIEventHandler.OnItemEquipped += UpdateEquippedWeapon;
        UIEventHandler.OnItemUnequipped += UnequipWeapon;
        UIEventHandler.OnPlayerLevelChange += UpdateLevel;
    }

    private void OnDestroy()
    {
        UIEventHandler.OnPlayerHealthChanged -= UpdateHealth;
        UIEventHandler.OnStatsChanged -= UpdateStats;
        UIEventHandler.OnItemEquipped -= UpdateEquippedWeapon;
        UIEventHandler.OnItemUnequipped -= UnequipWeapon;
        UIEventHandler.OnPlayerLevelChange -= UpdateLevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        
        InitializeStats();
    }

    void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth.ToString();
        healthFill.fillAmount = (float)currentHealth / maxHealth;
    }

    void UpdateLevel()
    {
        levelText.text = player.PlayerLevel.Level.ToString();
        levelFill.fillAmount = (float)player.PlayerLevel.CurrentExperience / player.PlayerLevel.RequiredExperience;
    }

    void InitializeStats()
    {
        for (int i = 0; i < player.CharacterStats.stats.Count; i++)
        {
            playerStatTexts.Add(Instantiate(playerStatPrefab, playerStatPanel));
        }

        UpdateStats();
    }

    void UpdateStats()
    {
        for (int i = 0; i < player.CharacterStats.stats.Count; i++)
        {
            playerStatTexts[i].text = player.CharacterStats.stats[i].GetCalculatedStatValue() + " " + player.CharacterStats.stats[i].StatName;
        }
    }

    void UpdateEquippedWeapon(Item item)
    {
        weaponIcon.sprite = Resources.Load<Sprite>("UI/Icons/Items/" + item.ItemSlug);
        weaponNameText.text = item.Name;

        for (int i = 0; i < item.Stats.Count; i++)
        {
            weaponStatTexts.Add(Instantiate(weaponStatPrefab, weaponStatPanel));
            weaponStatTexts[i].text = item.Stats[i].GetCalculatedStatValue() + " " + item.Stats[i].StatName;
        }

        unequipWeaponButton.interactable = true;
    }

    public void UnequipWeapon(Item item)
    {
        weaponNameText.text = "Nothing Equipped";
        weaponIcon.sprite = defaultWeaponSprite;
        
        foreach (Text statText in weaponStatTexts)
            Destroy(statText.gameObject);

        weaponStatTexts.Clear();

        unequipWeaponButton.interactable = false;
    }

    public void UnequipButton()
    {
        playerWeaponController.UnequipWeapon();

        UnequipWeapon(null);
    }
}
