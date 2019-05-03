using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    private GraphicsSettings graphicsSettings = new GraphicsSettings();

    private VideoSettings videoSettings = new VideoSettings();

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
    [SerializeField] ToggleButton softVegetationToggleButton;
    [SerializeField] ToggleButton softParticlesToggleButton;

    [Header("Video UI")]
    [SerializeField] Dropdown monitorDropdown;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown displayModeDropdown;
    [SerializeField] Dropdown vsyncDropdown;

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
        #region Graphics
        overallQualityDropdown.onValueChanged.AddListener(delegate { UpdateOverallQuality(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { UpdateTextureQuality(); });
        antiAliasingDropdown.onValueChanged.AddListener(delegate { UpdateAntiAliasing(); });
        shadowQualityDropdown.onValueChanged.AddListener(delegate { UpdateShadowQuality(); });
        shadowCascadesDropdown.onValueChanged.AddListener(delegate { UpdateShadowCascades(); });
        shadowDistanceSlider.onValueChanged.AddListener(delegate { UpdateShadowDistance(); });
        softVegetationToggleButton.onValueChanged.AddListener(delegate { UpdateSoftVegetation(); });
        softParticlesToggleButton.onValueChanged.AddListener(delegate { UpdateSoftParticles(); });

        // Load OverallQuality value
        overallQualityDropdown.value = PlayerPrefs.GetInt("OverallQuality", 1);
        EnableCustomControls(overallQualityDropdown.value == 0);
        #endregion

        #region Video
        monitorDropdown.onValueChanged.AddListener(delegate { UpdateMonitor(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { UpdateResolution(); });
        displayModeDropdown.onValueChanged.AddListener(delegate { UpdateDisplayMode(); });
        vsyncDropdown.onValueChanged.AddListener(delegate { UpdateVSync(); });

        GetMonitors();
        #endregion
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
    
    void EnableCustomControls(bool state)
    {
        if (state)
            SetCustomGraphics();
        else
        {
            UpdateTextureQuality();
            UpdateAntiAliasing();
            UpdateShadowQuality();
            UpdateShadowCascades();
            UpdateShadowDistance();
            UpdateSoftVegetation();
            UpdateSoftParticles();
        }

        textureQualityDropdown.interactable = state;
        antiAliasingDropdown.interactable = state;
        shadowQualityDropdown.interactable = state;
        shadowCascadesDropdown.interactable = state;
        shadowDistanceSlider.interactable = state;
        softVegetationToggleButton.button.interactable = state;
        softParticlesToggleButton.button.interactable = state;
    }

    void SetCustomGraphics()
    {
        textureQualityDropdown.value = PlayerPrefs.GetInt("TextureQuality", 0);
        antiAliasingDropdown.value = PlayerPrefs.GetInt("AntiAliasing", 0);
        shadowQualityDropdown.value = PlayerPrefs.GetInt("ShadowQuality", 0);
        shadowCascadesDropdown.value = PlayerPrefs.GetInt("ShadowCascades", 0);
        shadowDistanceSlider.value = PlayerPrefs.GetInt("ShadowDistance", 500);
        softVegetationToggleButton.Value = (PlayerPrefs.GetInt("SoftVegetation", 1) == 1);
        softParticlesToggleButton.Value = (PlayerPrefs.GetInt("SoftParticles", 1) == 1);
    }

    void UpdateOverallQuality()
    {
        int overallQualityValue = (6 - overallQualityDropdown.value);

        EnableCustomControls(overallQualityValue == 6);

        graphicsSettings.SetOverallQuality((GraphicsSettings.OverallQualities)overallQualityValue);
        
        PlayerPrefs.SetInt("OverallQuality", overallQualityDropdown.value);
    }

    void UpdateTextureQuality()
    {
        graphicsSettings.SetTextureQuality(textureQualityDropdown.value);

        PlayerPrefs.SetInt("TextureQuality", textureQualityDropdown.value);
    }

    void UpdateAntiAliasing()
    {
        graphicsSettings.SetAntiAliasing(3 - antiAliasingDropdown.value);

        PlayerPrefs.SetInt("AntiAliasing", antiAliasingDropdown.value);
    }

    void UpdateShadowQuality()
    {
        graphicsSettings.SetShadowQuality((ShadowResolution)(3 - shadowQualityDropdown.value));

        PlayerPrefs.SetInt("ShadowQuality", shadowQualityDropdown.value);
    }

    void UpdateShadowCascades()
    {
        graphicsSettings.SetShadowCascades(2 - shadowCascadesDropdown.value);

        PlayerPrefs.SetInt("ShadowCascades", shadowCascadesDropdown.value);
    }

    void UpdateShadowDistance()
    {
        graphicsSettings.SetShadowDistance((int)shadowDistanceSlider.value);

        PlayerPrefs.SetInt("ShadowDistance", (int)shadowDistanceSlider.value);
    }

    void UpdateSoftVegetation()
    {
        graphicsSettings.SetSoftVegetation(softVegetationToggleButton.Value);

        PlayerPrefs.SetInt("SoftVegetation", softVegetationToggleButton.Value ? 1 : 0);
    }

    void UpdateSoftParticles()
    {
        graphicsSettings.SetSoftParticles(softParticlesToggleButton.Value);

        PlayerPrefs.SetInt("SoftParticles", softParticlesToggleButton.Value ? 1 : 0);
    }

    #endregion

    #region Video UI

    void GetMonitors()
    {
        Display[] displays = videoSettings.GetDisplays();

        foreach (Display display in displays)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(display.ToString());
            monitorDropdown.options.Add(option);
        }

    }

    void UpdateMonitor()
    {
        videoSettings.SetDisplay(monitorDropdown.value);
    }

    void UpdateResolution()
    {

    }

    void UpdateDisplayMode()
    {

    }

    void UpdateVSync()
    {

    }

    #endregion
}
