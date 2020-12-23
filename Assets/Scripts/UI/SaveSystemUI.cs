using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystemUI : MonoBehaviour
{
    [SerializeField] Image savingGameIcon;
    [SerializeField] Image loadingGameIcon;

    private void Awake()
    {
        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;
    }

    private void OnDestroy()
    {
        SaveLoadController.OnSaveGame -= SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame -= SaveLoadController_OnLoadGame;
    }

    private void Start()
    {
        savingGameIcon.enabled = false;
        loadingGameIcon.enabled = false;
    }
    

    private void SaveLoadController_OnSaveGame()
    {
        StartCoroutine(SavingGame());
    }

    private void SaveLoadController_OnLoadGame()
    {
        StartCoroutine(LoadingGame());
    }

    private IEnumerator SavingGame()
    {
        savingGameIcon.enabled = true;

        yield return new WaitForSeconds(1f);

        savingGameIcon.enabled = false;
    }

    private IEnumerator LoadingGame()
    {
        loadingGameIcon.enabled = true;

        yield return new WaitForSeconds(1f);

        loadingGameIcon.enabled = false;
    }
}
