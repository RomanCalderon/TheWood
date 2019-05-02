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
    
    public void SetOverallQuality(OverallQualities newOverallQuality)
    {
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

    /// <summary>
    /// Sets QualitySettings.shadowResolution to newShadowQuality. (ShadowResolution)
    /// </summary>
    /// <param name="newShadowQuality">The new Shadow Resolution.</param>
    public void SetShadowQuality(ShadowResolution newShadowQuality)
    {
        QualitySettings.shadowResolution = newShadowQuality;
    }

    /// <summary>
    /// Sets QualitySettings.shadowCascades to newShadowCascades.
    /// 4 = Four Cascades
    /// 2 = Two Cascades
    /// 1 = No Cascades
    /// </summary>
    /// <param name="newShadowCascades">The new shadow cascades. (0-2)</param>
    public void SetShadowCascades(int newShadowCascades)
    {
        switch (newShadowCascades)
        {
            case 2:
                QualitySettings.shadowCascades = 4;
                break;
            case 1:
                QualitySettings.shadowCascades = 2;
                break;
            case 0:
                QualitySettings.shadowCascades = 1;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets QualitySettings.shadowDistance to newShadowDistance.
    /// </summary>
    /// <param name="newShadowDistance">New shadow distance.</param>
    public void SetShadowDistance(int newShadowDistance)
    {
        QualitySettings.shadowDistance = newShadowDistance;
    }

    /// <summary>
    /// Sets QualitySettings.softVegetation to newSoftVegetation.
    /// </summary>
    /// <param name="newSoftVegetation">New soft vegetation value.</param>
    public void SetSoftVegetation(bool newSoftVegetation)
    {
        QualitySettings.softVegetation = newSoftVegetation;
    }

    /// <summary>
    /// Sets QualitySettings.softParticles to newSoftParticles.
    /// </summary>
    /// <param name="newSoftParticles">New softParticles value.</param>
    public void SetSoftParticles(bool newSoftParticles)
    {
        QualitySettings.softParticles = newSoftParticles;
    }
}
