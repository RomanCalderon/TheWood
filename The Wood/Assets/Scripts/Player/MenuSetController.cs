using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuPanels
{
    INVENTORY,
    QUESTS,
    BUILDER,
    NONE
}

public class MenuSetController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] CanvasGroup mainPanel;
    [SerializeField] CanvasGroup inventoryPanel;
    [SerializeField] CanvasGroup questPanel;
    [SerializeField] CanvasGroup builderPanel;

    [Header("Menu Buttons")]
    [SerializeField] Button inventoryButton;
    [SerializeField] Button questsButton;
    [SerializeField] Button builderButton;

    bool canInteract = true;
    bool menuIsActive { get; set; }
    MenuPanels currentMenuPanel = MenuPanels.INVENTORY;


    private void Awake()
    {
        QuestController.OnQuestMenuStateChanged += QuestController_OnQuestMenuStateChanged;
        BuildingController.OnSelectedBlueprint += BuildingController_OnSelectedBlueprint;

        UIEventHandler.OnItemStorage += UIEventHandler_OnItemStorage;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMenuSetState(MenuPanels.NONE);

        inventoryButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.INVENTORY); } );
        questsButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.QUESTS); });
        builderButton.onClick.AddListener(delegate { OpenMenuPanel(MenuPanels.BUILDER); });
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle Menu Set UI if player is not in bed
        if (Input.GetKeyDown(KeyBindings.ToggleMenuSet) && !PlayerSleepController.IsInBed && canInteract)
            ChangeMenuSetState(menuIsActive ? MenuPanels.NONE : currentMenuPanel);
    }


    private void ChangeMenuSetState(MenuPanels menuPanel)
    {
        OpenMenuPanel(menuPanel);

        UIEventHandler.UIDisplayed(menuIsActive);
    }
    
    private void OpenMenuPanel(MenuPanels openPanel)
    {
        menuIsActive = false;
        SetCanvasGroupActive(inventoryPanel, false);
        SetCanvasGroupActive(questPanel, false);
        SetCanvasGroupActive(builderPanel, false);

        inventoryButton.interactable = false;
        questsButton.interactable = false;
        builderButton.interactable = false;

        // Activate the main panel if openPanel is anything but NONE
        menuIsActive = openPanel != MenuPanels.NONE;
        SetCanvasGroupActive(mainPanel, menuIsActive);

        switch (openPanel)
        {
            case MenuPanels.INVENTORY:
                SetCanvasGroupActive(inventoryPanel, true);
                questsButton.interactable = true;
                builderButton.interactable = true;
                break;
            case MenuPanels.QUESTS:
                SetCanvasGroupActive(questPanel, true);
                inventoryButton.interactable = true;
                builderButton.interactable = true;
                break;
            case MenuPanels.BUILDER:
                SetCanvasGroupActive(builderPanel, true);
                inventoryButton.interactable = true;
                questsButton.interactable = true;
                break;
            case MenuPanels.NONE:
                SetCanvasGroupActive(inventoryPanel, false);
                SetCanvasGroupActive(questPanel, false);
                SetCanvasGroupActive(builderPanel, false);

                inventoryButton.interactable = false;
                questsButton.interactable = false;
                builderButton.interactable = false;
                break;
            default:
                break;
        }

        if (openPanel != MenuPanels.NONE)
            currentMenuPanel = openPanel;
    }

    private void SetCanvasGroupActive(CanvasGroup cg, bool state)
    {
        if (state) // Active
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        else // Inactive
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }


    // Event listener
    private void QuestController_OnQuestMenuStateChanged(bool state)
    {
        // Called when a Quest has been proposed, open the menu set AND quest panel
        ChangeMenuSetState(state ? MenuPanels.QUESTS : MenuPanels.NONE);
    }

    private void BuildingController_OnSelectedBlueprint(Blueprint blueprint)
    {
        ChangeMenuSetState(MenuPanels.NONE);
    }

    private void UIEventHandler_OnItemStorage(bool state)
    {
        canInteract = !state;
    }
}
