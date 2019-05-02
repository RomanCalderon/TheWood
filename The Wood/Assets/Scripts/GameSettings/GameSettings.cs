using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public GraphicsSettings GraphicsSettings = new GraphicsSettings();

    [SerializeField] int mainMenuSceneIndex;
    [SerializeField] GameObject backgroundPanelUI;

    [Header("Menu UI")]
    bool menuOpen = false;
    [SerializeField] GameObject optionsMenu;

    [Header("Graphics UI")]
    [SerializeField] Dropdown overallQualityDropdown;
    [Space]
    [SerializeField] Dropdown textureQualityDropdown;
    [SerializeField] Dropdown antiAliasingDropdown;
    [SerializeField] Dropdown shadowQualityDropdown;
    [SerializeField] Dropdown shadowCascadesDropdown;
    [SerializeField] Slider shadowDistanceSlider;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);

        OpenMenu(false);
    }

    private void Start()
    {
        UpdateOverallQuality();

        #region Graphics
        overallQualityDropdown.onValueChanged.AddListener(delegate { UpdateOverallQuality(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { UpdateTextureQuality(); });
        antiAliasingDropdown.onValueChanged.AddListener(delegate { UpdateAntiAliasing(); });
        #endregion
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Don't open the menu by key input while in the main menu scene
            if (!menuOpen && SceneManager.GetActiveScene().buildIndex == mainMenuSceneIndex)
                return;

            // Toggle options menu
            OpenMenu(menuOpen = !menuOpen);
        }
    }

    #region Menu UI

    public void OpenMenu(bool state = true)
    {
        menuOpen = state;

        backgroundPanelUI.SetActive(state);

        optionsMenu.SetActive(state);

        MainMenuManager.MenuOpen = state;
    }

    #endregion

    #region Graphics UI
    
    void UpdateOverallQuality()
    {
        int overallQualityValue = (6 - overallQualityDropdown.value);

        EnableFullGraphicsControls(overallQualityValue == 6);

        GraphicsSettings.SetOverallQuality((GraphicsSettings.OverallQualities)overallQualityValue);
    }

    void EnableFullGraphicsControls(bool state)
    {
        textureQualityDropdown.interactable = state;
        antiAliasingDropdown.interactable = state;
        shadowQualityDropdown.interactable = state;
        shadowCascadesDropdown.interactable = state;
        shadowDistanceSlider.interactable = state;
    }

    void UpdateTextureQuality()
    {
        GraphicsSettings.SetTextureQuality(textureQualityDropdown.value);
    }

    void UpdateAntiAliasing()
    {
        GraphicsSettings.SetAntiAliasing(3 - antiAliasingDropdown.value);
    }

    void UpdateShadowQuality()
    {

    }

    void UpdateShadowCascades()
    {

    }

    void UpdateShadowDistance()
    {

    }

    #endregion
}
