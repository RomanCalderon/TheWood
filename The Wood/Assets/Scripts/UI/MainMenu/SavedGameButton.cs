using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGameButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Text gameNameText;

    string gamePath;
    
    public void Initialize(string gamePath, Action<string> loadGameCallback)
    {
        this.gamePath = gamePath;

        gameNameText.text = GameDataManager.GetGameNameFromPath(gamePath);

        button.onClick.AddListener(delegate { loadGameCallback(gamePath); });
    }
}
