using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarItem : MonoBehaviour
{
    [Header("Icon UI")]
    [SerializeField] Image iconImage;

    [Header("Background UI")]
    [SerializeField] Image backgroundImage;
    Color backgroundImageTargetColor;
    Color normalColor;
    [SerializeField] Color activeColor;

    // Start is called before the first frame update
    void Start()
    {
        BuildingController.OnEnabledBuildMode += BuildingController_OnEnabledBuildMode;
        BuildingController.OnDisabledBuildMode += BuildingController_OnDisabledBuildMode;

        normalColor = backgroundImage.color;
        backgroundImageTargetColor = normalColor;
    }

    private void Update()
    {
        backgroundImage.color = Color.Lerp(backgroundImage.color, backgroundImageTargetColor, Time.deltaTime * 10f);
    }

    private void BuildingController_OnEnabledBuildMode()
    {
        backgroundImageTargetColor = activeColor;
    }

    private void BuildingController_OnDisabledBuildMode()
    {
        backgroundImageTargetColor = normalColor;
    }
}
