using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettings
{
    public enum OverallQualities
    {
        VERY_LOW,
        LOW,
        MEDIUM,
        HIGH,
        VERY_HIGH,
        ULTRA,
        CUSTOM
    }

    public OverallQualities OverallQuality;
    
    public int TextureQuality;
    
    public int AntiAliasing;
    
    public void SetOverallQuality(OverallQualities newOverallQuality)
    {
        try
        {
            OverallQuality = newOverallQuality;
        }
        catch
        {
            Debug.Log((int)newOverallQuality);
        }
        QualitySettings.SetQualityLevel((int)newOverallQuality, true);
    }

    /// <summary>
    /// 0 = Full,
    /// 1 = Half,
    /// 2 = Quarter,
    /// 3 = Eighth
    /// </summary>
    public void SetTextureQuality(int newTextureQuality)
    {
        newTextureQuality = Mathf.Clamp(newTextureQuality, 0, 3);

        QualitySettings.masterTextureLimit = newTextureQuality;
    }

    /// <summary>
    /// 0 = Disabled,
    /// 1 = 2x,
    /// 2 = 4x,
    /// 3 = 8x
    /// </summary>
    public void SetAntiAliasing(int newAntiAliasing)
    {
        switch (newAntiAliasing)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                break;
            default:
                QualitySettings.antiAliasing = 0;
                Debug.Log("Unexpected value: " + newAntiAliasing);
                break;
        }
    }
}
