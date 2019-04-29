using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Awake()
    {
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

    }

    // CONTINUE GAME
    private void SignInteractions_OnContinueGamePressed()
    {

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
