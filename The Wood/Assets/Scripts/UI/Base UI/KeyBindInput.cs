using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// This class allows the player is give a key input as a KeyCode.
/// There should be a KeyBindings class that can provide a default
/// KeyCode to this component via a manager.
/// </summary>
[RequireComponent(typeof(Button))]
public class KeyBindInput : MonoBehaviour
{
    [HideInInspector] public Button button;
    
    [SerializeField] Text valueText;
    private KeyCode value;
    public KeyCode Value
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
    public static bool RecordingKeyCode = false;
    
    private Array keyCodes;
    
    public UnityEvent onValueChanged;
    
    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { ButtonPressed(); });
        onValueChanged.AddListener(delegate { UpdateValueText(); });

        // Get all possible KeyCodes
        keyCodes = Enum.GetValues(typeof(KeyCode));
    }

    private void Update()
    {
        if (RecordingKeyCode)
        {
            // Allow the player to cancel the key bind
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RecordingKeyCode = false;
                return;
            }

            foreach (KeyCode kcode in keyCodes)
            {
                if (Input.GetKey(kcode))
                {
                    Value = kcode;

                    RecordingKeyCode = false;
                }
            }
        }
    }

    public void ButtonPressed()
    {
        RecordingKeyCode = true;
    }

    private void UpdateValueText()
    {
        if (valueText != null)
            valueText.text = KeyBindings.GetFormat(Value);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
