using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Awake()
    {
        // Verify the SavedGames directory
        GameDataManager.VerifySavedGamesDirectory();

        // Unsubscribe from button events
        SignInteractions.OnNewGamePressed -= SignInteractions_OnNewGamePressed;
        SignInteractions.OnContinueGamePressed -= SignInteractions_OnContinueGamePressed;
        SignInteractions.OnOptionsPressed -= SignInteractions_OnOptionsPressed;
        SignInteractions.OnQuitPressed -= SignInteractions_OnQuitPressed;

        // Subscribe to button events
        SignInteractions.OnNewGamePressed += SignInteractions_OnNewGamePressed;
        SignInteractions.OnContinueGamePressed += SignInteractions_OnContinueGamePressed;
        SignInteractions.OnOptionsPressed += SignInteractions_OnOptionsPressed;
        SignInteractions.OnQuitPressed += SignInteractions_OnQuitPressed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region Button Event Listeners

    // NEW GAME
    private void SignInteractions_OnNewGamePressed()
    {
        //GameDataManager.CreateNewGameDirectory("");
    }

    // CONTINUE GAME
    private void SignInteractions_OnContinueGamePressed()
    {
        //GameDataManager.RetreiveSavedGames(PrintSavedGames);

        GameDataManager.SetCurrentGame("TestGame1");

        SceneManager.LoadScene("Game");
    }

    // OPTIONS
    private void SignInteractions_OnOptionsPressed()
    {

    }

    // QUIT GAME
    private void SignInteractions_OnQuitPressed()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

#endregion
}
