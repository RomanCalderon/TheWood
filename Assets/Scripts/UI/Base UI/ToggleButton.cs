using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[ExecuteInEditMode] [RequireComponent(typeof(Button))]
public class ToggleButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    
    [SerializeField] Text valueText;
    private bool value;
    public bool Value
    {
        get { return value; }
        set
        {
            if (value != this.value)
            {
                this.value = value;
                onValueChanged?.Invoke();
            }
            this.value = value;

            UpdateValueText();
        }
    }
    
    public UnityEvent onValueChanged;
    
    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { ToggleButtonPressed(); });
    }

    public void ToggleButtonPressed()
    {
        Value = !Value;
    }

    private void UpdateValueText()
    {
        if (valueText != null)
            valueText.text = (Value) ? "ON" : "OFF";
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void OnValidate()
    {
        UpdateValueText();
    }
}
