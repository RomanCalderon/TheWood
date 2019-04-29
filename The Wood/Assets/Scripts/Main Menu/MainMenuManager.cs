using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Awake()
    {
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
        throw new System.NotImplementedException();
    }

    // CONTINUE GAME
    private void SignInteractions_OnContinueGamePressed()
    {
        throw new System.NotImplementedException();
    }

    // OPTIONS
    private void SignInteractions_OnOptionsPressed()
    {
        throw new System.NotImplementedException();
    }

    // QUIT GAME
    private void SignInteractions_OnQuitPressed()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
