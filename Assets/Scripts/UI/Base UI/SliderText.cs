using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderText : MonoBehaviour
{
    Slider slider;

    [SerializeField] Text valueText;
    
    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { UpdateText(); });
        valueText.text = slider.value.ToString();
    }
    
    void UpdateText()
    {
        if (valueText != null)
            valueText.text = GetComponent<Slider>().value.ToString();
    }

    //private void OnValidate()
    //{
    //    UpdateText();
    //}
}
