using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoadingController : MonoBehaviour
{
    [SerializeField] private Slider loadingBarSlider;

    void Start()
    {
        if (loadingBarSlider != null)
        {
            loadingBarSlider.value = 0f;
        }
            
        // Make sure we have a valid target scene
        if (string.IsNullOrEmpty(SceneLoader.targetScene))
        {
            Debug.LogError("SceneLoadingController: targetScene not set!");
            return;
        }

        Scene scene = SceneManager.GetSceneByName(SceneLoader.targetScene);

        Debug.Log("scene: " + scene);
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("Coroutine started");
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneLoader.targetScene);
        operation.allowSceneActivation = false;
        float minimumLoadTime = 1.0f;
        float timer = 0f;
        float displayedProgress = 0f;

        while (operation.progress < 0.9f || timer < minimumLoadTime)
        {
            timer += Time.unscaledDeltaTime;
            Debug.Log("Looping...");

            // Unity async progress goes from 0 to 0.9
            float unityProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Time-based progress
            float timeProgress = Mathf.Clamp01(timer / minimumLoadTime);

            // Target progress 
            float targetProgress = Mathf.Min(Mathf.Max(unityProgress, timeProgress), 1f);

            // Smooth the displayed progress
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.unscaledDeltaTime * 0.8f);

            if (loadingBarSlider != null)
            {
                loadingBarSlider.value = displayedProgress;
            }
                
            yield return null;
        }
            // Once Unity says loading is done and bar is full, activate scene
            loadingBarSlider.value = 1f;
            yield return new WaitForSecondsRealtime(0.2f); 
            operation.allowSceneActivation = true;
    }
}