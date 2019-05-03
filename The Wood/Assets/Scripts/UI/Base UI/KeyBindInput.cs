using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class KeybindRecordEvent : UnityEvent<bool, string>
{

}

/// <summary>
/// This class allows the player is give a key input as a KeyCode.
/// There should be a KeyBindings class that can provide a default
/// KeyCode to this component via a manager.
/// </summary>
[RequireComponent(typeof(Button))]
public class KeyBindInput : MonoBehaviour
{
    [HideInInspector] public Button button;

    /// <summary>
    /// Name of the keybind.
    /// </summary>
    [SerializeField] string keybindName;
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

    private bool recording = false;
    public static bool RecordingKeyCode = false;
    public KeybindRecordEvent onRecording = new KeybindRecordEvent();

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
        if (recording && RecordingKeyCode)
        {
            // Allow the player to cancel the key bind
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(FinishRecording());
                UpdateValueText();
                return;
            }

            foreach (KeyCode kcode in keyCodes)
            {
                if (Input.GetKeyDown(kcode))
                {
                    Value = kcode;

                    StartCoroutine(FinishRecording());
                }
            }
        }
    }

    public void ButtonPressed()
    {
        recording = RecordingKeyCode = true;
        button.interactable = false;
        onRecording?.Invoke(true, "press a key to bind [" + keybindName + "]\n[escape] cancel keybind");
    }

    private void UpdateValueText()
    {
        if (valueText != null)
            valueText.text = KeyBindings.GetFormat(Value);
    }

    private IEnumerator FinishRecording()
    {
        recording = RecordingKeyCode = false;
        button.interactable = true;
        yield return new WaitForSeconds(0.05f);

        onRecording?.Invoke(false, null);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
