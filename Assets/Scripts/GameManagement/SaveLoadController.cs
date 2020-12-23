using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController : MonoBehaviour
{
    public static SaveLoadController Instance;

    public delegate void SaveLoadHandler();
    public static event SaveLoadHandler OnSaveGame;
    public static event SaveLoadHandler OnLoadGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == "Game")
            LoadGame();
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    LoadGame();
    //}

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
