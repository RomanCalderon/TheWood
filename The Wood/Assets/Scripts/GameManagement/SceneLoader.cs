using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    public static bool IsLoadingScene
    {
        get;
        private set;
    }
    
    [SerializeField] CanvasGroup canvasGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int sceneIndex)
    {
        if (SceneManager.GetSceneByBuildIndex(sceneIndex) == null)
            return;
        
        LoadScene(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    }

    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) == null)
            return;

        print("Loading scene: [" + sceneName + "]");
        StartCoroutine(BeginLoad(sceneName));
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, (IsLoadingScene?1:0), Time.deltaTime * 4f);
    }

    private IEnumerator BeginLoad(string sceneName)
    {
        // Set IsLoadingScene to true and enable the loading canvas
        canvasGroup.blocksRaycasts = IsLoadingScene = true;

        while(AudioListener.volume > 0.01f)
        {
            AudioListener.volume -= Time.deltaTime;
            yield return null;
        }
        AudioListener.volume = 0f;

        yield return new WaitForSeconds(2f);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        StartCoroutine(EndLoad());
    }

    private IEnumerator EndLoad()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        canvasGroup.blocksRaycasts = IsLoadingScene = false;

        while (AudioListener.volume < 0.95f)
        {
            AudioListener.volume += Time.deltaTime;
            yield return null;
        }
        AudioListener.volume = 1f;
    }
}
