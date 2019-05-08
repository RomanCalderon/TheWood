using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    private Stack<GameObject> menusOpen = new Stack<GameObject>();
    private bool subMenuOpen
    {
        get
        {
            return menusOpen.Count > 0;
        }
    }

    [Header("UI")]
    [SerializeField] GameObject pauseMenuUI;
    [Space]
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject quitWarningUI;
    private bool quitToMenu = false;
    private bool quitToDesktop = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
            instance = this;
        DontDestroyOnLoad(this);

        PauseController.OnPauseEvent += PauseController_OnPauseEvent;
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !SceneLoader.IsLoadingScene)
        {
            if (PauseController.IsPaused)
            {
                if (!subMenuOpen)
                    PauseController.ResumeGame();
                else
                    CloseSubMenu();
            }
            else
                PauseController.PauseGame();
        }
    }

    #region UI Button Press Events

    public void ResumeButton()
    {
        menusOpen.Clear();

        PauseController.ResumeGame();
    }

    public void OptionsButton()
    {
        SubMenuOpened(optionsMenu);
    }

    public void QuitToMenuButton()
    {
        quitToMenu = true;
        SubMenuOpened(quitWarningUI);
    }

    public void QuitToDesktopButton()
    {
        quitToDesktop = true;
        SubMenuOpened(quitWarningUI);
    }

    public void CancelQuitButton()
    {
        quitToMenu = false;
        quitToDesktop = false;

        CloseSubMenu();
    }

    public void ConfirmQuitButton()
    {
        if (quitToMenu)
            SceneLoader.instance.LoadScene("MainMenu");
        else if (quitToDesktop)
            Application.Quit();

        // Closes the warning menu and pause menu
        CloseSubMenu();
        menusOpen.Clear();
        PauseController.ResumeGame();
    }

    #endregion
    
    public void SubMenuOpened(GameObject menu)
    {
        menusOpen.Push(menu);

        if (menu == optionsMenu)
            GameSettings.instance.OpenMenu(true);
        else
            menu.SetActive(true);
        
        UIEventHandler.UIDisplayed(true);
    }

    public void CloseSubMenu()
    {
        GameObject menu = menusOpen.Pop();

        if (menu == optionsMenu)
            GameSettings.instance.OpenMenu(false);
        else
            menu.SetActive(false);

        UIEventHandler.UIDisplayed(false);
    }

    private void PauseController_OnPauseEvent(bool paused)
    {
        UIEventHandler.UIDisplayed(paused);

        if (!subMenuOpen)
            pauseMenuUI.SetActive(paused);
    }
}
