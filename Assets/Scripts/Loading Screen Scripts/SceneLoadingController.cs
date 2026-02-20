using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoadingController : MonoBehaviour
{
    [SerializeField] private Slider loadingBarSlider;
    private float displayedProgress = 0f;

    void Start()
    {
        if (loadingBarSlider != null)
            loadingBarSlider.value = 0f;

        displayedProgress = 0f;

        // Make sure we have a valid target scene
        if (string.IsNullOrEmpty(SceneLoader.targetScene))
        {
            Debug.LogError("SceneLoadingController: targetScene not set!");
            return;
        }

        StartCoroutine(LoadSceneAsync(SceneLoader.targetScene));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Unity async progress goes from 0 to 0.9
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smooth the displayed progress
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 2f);

            if (loadingBarSlider != null)
                loadingBarSlider.value = displayedProgress;

            // Once Unity says loading is done (progress >= 0.9) and bar is full, activate scene
            if (operation.progress >= 0.9f && displayedProgress >= 1f)
            {
                SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
                yield return new WaitForSeconds(0.2f); // optional small pause
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}