using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintUIButton : MonoBehaviour
{
    Blueprint blueprint;
    BuildingController buildingController;
    [SerializeField] Image blueprintIcon;
    [SerializeField] Button button;

    public void Initialize(Blueprint blueprint, BuildingController buildingController)
    {
        this.blueprint = blueprint;
        this.buildingController = buildingController;

        blueprintIcon.sprite = blueprint.Icon;

        button.onClick.AddListener(delegate { BuildingController.SelectedBlueprint(blueprint); } );
    }
}
