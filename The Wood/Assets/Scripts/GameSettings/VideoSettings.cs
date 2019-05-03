using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSettings
{
    // Display
    public int currentDisplay;
    public bool displayChanged;

    // Resolution
    public Resolution[] resolutions;

    public Display[] GetDisplays()
    {
        return Display.displays;
    }

    public void SetDisplay(int displayIndex)
    {
        if (Display.displays.Length > displayIndex)
        {
            displayChanged = displayIndex != currentDisplay;

            currentDisplay = displayIndex;
            GameSettings.instance.StartCoroutine(GameSettings.instance.TargetDisplay(displayIndex));
        }
        else
            Debug.LogError("displayIndex [" + displayIndex + "] out of range: " + Display.displays.Length);
    }

    public Resolution[] GetResolutions()
    {
        return Screen.resolutions;
    }

    public void SetResolution(int newResolutionIndex)
    {
        resolutions = Screen.resolutions;

        if (newResolutionIndex < resolutions.Length)
        {
            Screen.SetResolution(resolutions[newResolutionIndex].width, resolutions[newResolutionIndex].height, Screen.fullScreenMode);
        }
        else
            Debug.LogError("newResolutionIndex [" + newResolutionIndex + "] out of range: " + resolutions.Length);
    }

    public void SetDisplayMode(FullScreenMode screenMode)
    {
        Screen.fullScreenMode = screenMode;
    }

    public int GetVSync()
    {
        return QualitySettings.vSyncCount;
    }

    public void SetVSync(int newVSync)
    {
        if (newVSync < 0 || newVSync > 2)
        {
            Debug.LogError("newVSync [" + newVSync + "] is out of bounds. (0-2)");
            return;
        }

        QualitySettings.vSyncCount = newVSync;
    }
}
