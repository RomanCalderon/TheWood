using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    public static SaveLoadController Instance;

    public delegate void SaveLoadHandler();
    public static event SaveLoadHandler OnSaveGame;
    public static event SaveLoadHandler OnLoadGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadGame();
    }

    // Events
    public static void SaveGame()
    {
        OnSaveGame?.Invoke();
    }

    private void LoadGame()
    {
        OnLoadGame?.Invoke();
    }
}
