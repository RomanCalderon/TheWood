using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool MenuOpen;

    [Header("New Game")]
    [SerializeField] GameObject newGameUI;
    [SerializeField] InputField newGameInputField;
    [SerializeField] Text newGameErrorText;

    [Header("Continue Game")]
    [SerializeField] GameObject continueGameUI;
    [SerializeField] GameObject savedGameButtonPrefab;
    [SerializeField] Transform savedGameButtonHolder;

    private void Awake()
    {
        // Verify the SavedGames directory exists
        // If not, create one
        GameDataManager.VerifySavedGamesDirectory();

        #region Button Event Subscriptions
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
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        MenuOpen = false;
    }

    #region New Game

    public void CreateNewGameButton()
    {
        GameDataManager.CreateNewGameDirectory(newGameInputField.text, VerifyNewGame);
    }

    void VerifyNewGame(int status, string newGamePath)
    {
        // Reset error text
        newGameErrorText.enabled = false;

        switch (status)
        {
            // Success: New Game Created
            case 0:
                StartGame(newGamePath);
                break;
            // Error: Invalid Game Name
            case 1:
                DisplayErrorText("Invalid game name. Please make sure there are only alphanumeric characters. (A-Z, 0-9)");
                break;
            // Error: Game Already Exists
            case 2:
                DisplayErrorText("Game with name '" + newGameInputField.text + "' already exists. Please choose a unique name.");
                break;
            default:
                break;
        }
    }

    public void CloseNewGameButton()
    {
        newGameUI.SetActive(MenuOpen = false);
        newGameInputField.text = string.Empty;
    }

    void DisplayErrorText(string errorString)
    {
        newGameErrorText.enabled = true;
        newGameErrorText.text = errorString;
    }

    #endregion

    #region Continue Game

    void LoadSavedGames(string[] games)
    {
        foreach (string s in games)
        {
            SavedGameButton button = Instantiate(savedGameButtonPrefab, savedGameButtonHolder).GetComponent<SavedGameButton>();
            button.Initialize(s, StartGame);
        }
    }

    public void CloseContinueGameUI()
    {
        // Hide continue game UI
        continueGameUI.SetActive(MenuOpen = false);

        // Remove all saved game buttons
        foreach (Transform t in savedGameButtonHolder)
            Destroy(t.gameObject);
    }

    #endregion

    // START GAME
    private void StartGame(string gamePath)
    {
        // Sets the current game path to gamePath
        GameDataManager.SetCurrentGamePath(gamePath);

        // FIXME: Let a scene loader handle this
        SceneManager.LoadScene("Game");
    }

    #region Button Event Listeners

    // NEW GAME
    private void SignInteractions_OnNewGamePressed()
    {
        newGameUI.SetActive(MenuOpen = true);
    }

    // CONTINUE GAME
    private void SignInteractions_OnContinueGamePressed()
    {
        continueGameUI.SetActive(MenuOpen = true);

        GameDataManager.RetreiveGameDirectories(LoadSavedGames);
    }

    // OPTIONS
    private void SignInteractions_OnOptionsPressed()
    {
        MenuOpen = true;

        GameSettings.instance.OpenMenu(true);
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
