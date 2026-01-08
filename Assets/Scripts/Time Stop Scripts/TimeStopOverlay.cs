using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class TimeStopOverlay : MonoBehaviour
{
    // Time stop volume 
    [SerializeField] private Volume timeStopVolume;

    // Volume fade fields
    private float fadeDuration = 1.5f;
    private Coroutine currentFade = null;

    void Awake()
    {
        // Initializes fields
        timeStopVolume.weight = 0f;
        timeStopVolume.enabled = true;
    }

    // Starts fading the time stop volume to grayscale
    public void StartTimeStopVFX()
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        currentFade = StartCoroutine(FadeWeightTo(1f));
    }

    // Starts fading the time stop volume away from grayscale
    public void StopTimeStopVFX()
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        currentFade = StartCoroutine(FadeWeightTo(0f));
    }

    // Fades the time stop volume weight to a target weight
    private IEnumerator FadeWeightTo(float target)
    {
        float start = timeStopVolume.weight;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            timeStopVolume.weight = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        timeStopVolume.weight = target;
    }
}
