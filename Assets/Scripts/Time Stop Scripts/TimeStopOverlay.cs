using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class TimeStopOverlay : MonoBehaviour
{
    public Volume timeStopVolume;
    private ColorAdjustments colorAdjustments;
    private float fadeDuration = 1f;

    private Coroutine currentFade = null;

    void Awake()
    {
        if (timeStopVolume.profile.TryGet<ColorAdjustments>(out var ca))
        {
            colorAdjustments = ca;
        }
    }

    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }
            currentFade = StartCoroutine(FadeSaturationTo(-100));
            timeStopVolume.enabled = true;
        }
        else
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }
            currentFade = StartCoroutine(FadeSaturationTo(0, disableAfter: true));
        }
    }

    private IEnumerator FadeSaturationTo(float target, bool disableAfter = false)
    {
        float start = colorAdjustments.saturation.value;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            colorAdjustments.saturation.value = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        colorAdjustments.saturation.value = target;

        if (disableAfter)
        {
            timeStopVolume.enabled = false;
        }
            
    }
}
