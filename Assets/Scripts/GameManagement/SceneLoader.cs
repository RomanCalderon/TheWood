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

    [Space]
    [SerializeField] Slider loadingBar;

    AsyncOperation async;

    Coroutine beginLoadCoroutine;
    Coroutine audioListenerVolumeCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(int sceneIndex)
    {
        if (SceneManager.GetSceneByBuildIndex(sceneIndex) == null)
            return;
        
        LoadScene(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    }

    public void LoadScene(string sceneName)
    {
        if (IsLoadingScene || SceneManager.GetSceneByName(sceneName) == null)
            return;

        print("Loading scene: [" + sceneName + "]");
        if (beginLoadCoroutine != null)
            StopCoroutine(beginLoadCoroutine);
        beginLoadCoroutine = StartCoroutine(BeginLoad(sceneName));
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, (IsLoadingScene?1:0), Time.deltaTime * 4f);
    }

    private IEnumerator BeginLoad(string sceneName)
    {
        // Set IsLoadingScene to true and enable the loading canvas
        canvasGroup.blocksRaycasts = IsLoadingScene = true;

        // Decrease AudioListener.volume
        audioListenerVolumeCoroutine = StartCoroutine(AudioListenerVolume(false));

        // Reset loading bar
        loadingBar.value = 0f;

        yield return new WaitForSeconds(2f);

        // Start scene load
        async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while ((!async.isDone) && (async.progress < 0.9f))
        {
            loadingBar.value = Mathf.Lerp(loadingBar.value, async.progress, Time.deltaTime);
            yield return null;
        }
        async.allowSceneActivation = true;
        loadingBar.value = 1f;
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        EndLoad(scene);
    }

    private void EndLoad(Scene scene)
    {
        Debug.Log("Finished loading scene [" + scene.name + "]");

        canvasGroup.blocksRaycasts = IsLoadingScene = false;

        // Increase AudioListener.volume
        audioListenerVolumeCoroutine = StartCoroutine(AudioListenerVolume(true));
    }

    private IEnumerator AudioListenerVolume(bool state)
    {
        float timeout = 3f;

        if (state)
        {
            while (AudioListener.volume < 0.95f)
            {
                AudioListener.volume += Time.deltaTime;
                yield return null;

                timeout -= Time.deltaTime;
                if (timeout <= 0)
                    break;
            }
            AudioListener.volume = 1f;
        }
        else
        {
            while (AudioListener.volume > 0.01f)
            {
                AudioListener.volume -= Time.deltaTime;
                yield return null;

                timeout -= Time.deltaTime;
                if (timeout <= 0)
                    break;
            }
            AudioListener.volume = 0f;
        }
    }
}
