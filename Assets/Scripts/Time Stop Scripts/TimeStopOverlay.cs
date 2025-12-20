using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class TimeStopOverlay : MonoBehaviour
{
    public Volume timeStopVolume;
    private float fadeDuration = 1.5f;
    private Coroutine currentFade = null;

    void Awake()
    {
        timeStopVolume.weight = 0f;
        timeStopVolume.enabled = true;
    }

    public void StartTimeStopVFX()
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        currentFade = StartCoroutine(FadeWeightTo(1f));
    }

    public void StopTimeStopVFX()
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        currentFade = StartCoroutine(FadeWeightTo(0f));
    }

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
