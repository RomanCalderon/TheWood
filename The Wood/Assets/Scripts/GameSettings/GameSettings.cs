﻿using System.Collections;
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
    [SerializeField] Text infoText;

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
        GetResolutions();

        SetVideoSettings();
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

    void SetVideoSettings()
    {
        //monitorDropdown.value = PlayerPrefs.GetInt("Monitor", 0);
        //resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 0);
        vsyncDropdown.value = PlayerPrefs.GetInt("VSync", 1);
    }

    void GetMonitors()
    {
        Display[] displays = videoSettings.GetDisplays();

        for (int i = 0; i < displays.Length; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData("monitor " + (i + 1));
            monitorDropdown.options.Add(option);
        }

        monitorDropdown.value = videoSettings.currentDisplay = PlayerPrefs.GetInt("UnitySelectMonitor");
    }

    public IEnumerator TargetDisplay(int targetDisplay)
    {
        if (videoSettings.displayChanged)
            infoText.text = "restart game to apply changes";

        // THE PROCESS BELOW REQUIRES A RESET FOR CHANGES TO APPLY

            // Get the current screen resolution.
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Set the target display and a low resolution.
        PlayerPrefs.SetInt("UnitySelectMonitor", targetDisplay);
        Screen.SetResolution(800, 600, FullScreenMode.MaximizedWindow);

        // Wait a frame.
        yield return null;

        // Restore resolution.
        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreenMode);
    }

    void UpdateMonitor()
    {
        videoSettings.SetDisplay(monitorDropdown.value);

        //PlayerPrefs.SetInt("Monitor", monitorDropdown.value);
    }

    void GetResolutions()
    {
        Resolution[] resolutions = videoSettings.GetResolutions();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].Equals(Screen.currentResolution))
                currentResolutionIndex = i;

            Dropdown.OptionData option = new Dropdown.OptionData(resolutions[i].width + "x" + resolutions[i].height);
            resolutionDropdown.options.Add(option);
        }

        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value = currentResolutionIndex);
    }

    void UpdateResolution()
    {
        videoSettings.SetResolution(resolutionDropdown.value);

        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
    }

    void UpdateDisplayMode()
    {
        videoSettings.SetDisplayMode((FullScreenMode)displayModeDropdown.value);

        PlayerPrefs.SetInt("DisplayMode", displayModeDropdown.value);
    }

    void UpdateVSync()
    {
        videoSettings.SetVSync(vsyncDropdown.value);

        PlayerPrefs.SetInt("VSync", vsyncDropdown.value);
    }

    #endregion
}