using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuPanels
{
    INVENTORY,
    QUESTS,
    BUILDER
}

public class MenuSetController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] RectTransform menuSet;
    [SerializeField] RectTransform inventoryPanel;
    [SerializeField] RectTransform questPanel;
    [SerializeField] RectTransform builderPanel;

    [Header("Menu Buttons")]
    [SerializeField] Button inventoryButton;
    [SerializeField] Button questsButton;
    [SerializeField] Button builderButton;

    bool menuIsActive { get; set; }
    MenuPanels currentMenuPanel = MenuPanels.INVENTORY;


    private void Awake()
    {
        QuestController.OnQuestMenuStateChanged += QuestController_OnQuestMenuStateChanged;
        BuildingController.OnSelectedBlueprint += BuildingController_OnSelectedBlueprint;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMenuSetState(menuIsActive = false);

        inventoryButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.INVENTORY); } );
        questsButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.QUESTS); });
        builderButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.BUILDER); });
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle Menu Set UI if player is awake
        if (Input.GetKeyDown(KeyBindings.ToggleMenuSet) && !PlayerSleepController.IsSleeping)
            ChangeMenuSetState(!menuIsActive, currentMenuPanel);
    }


    // Event listener
    private void QuestController_OnQuestMenuStateChanged(bool state)
    {
        // Called when a Quest has been proposed, open the menu set AND quest panel
        ChangeMenuSetState(state, MenuPanels.QUESTS);
    }

    private void BuildingController_OnSelectedBlueprint(Blueprint blueprint)
    {
        ChangeMenuSetState(false);
    }


    private void ChangeMenuSetState(bool newState)
    {
        menuIsActive = newState;
        menuSet.gameObject.SetActive(menuIsActive);

        UIEventHandler.UIDisplayed(menuIsActive);
    }

    private void ChangeMenuSetState(bool newState, MenuPanels menuPanel)
    {
        ChangeMenuSetState(newState);

        OpenMenuPanel(menuPanel);
    }
    
    private void OpenMenuPanel(MenuPanels openPanel)
    {
        inventoryPanel.gameObject.SetActive(false);
        questPanel.gameObject.SetActive(false);
        builderPanel.gameObject.SetActive(false);

        inventoryButton.interactable = false;
        questsButton.interactable = false;
        builderButton.interactable = false;

        switch (openPanel)
        {
            case MenuPanels.INVENTORY:
                inventoryPanel.gameObject.SetActive(true);
                questsButton.interactable = true;
                builderButton.interactable = true;
                break;
            case MenuPanels.QUESTS:
                questPanel.gameObject.SetActive(true);
                inventoryButton.interactable = true;
                builderButton.interactable = true;
                break;
            case MenuPanels.BUILDER:
                builderPanel.gameObject.SetActive(true);
                inventoryButton.interactable = true;
                questsButton.interactable = true;
                break;
            default:
                break;
        }

        currentMenuPanel = openPanel;
    }
}
