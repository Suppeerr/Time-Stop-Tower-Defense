using UnityEngine;
using System.Collections;

public class LanternUpdate : MonoBehaviour
{
    private Light lanternLight;
    private Coroutine flickerRoutine = null;

    private LanternState state = LanternState.Off;

    private float startDelay;
    private float fadeDuration = 1.5f;

    // Lantern flickering
    private float minLightIntensity = 10f;
    private float maxLightIntensity = 30f;
    private float flickerSpeed = 2f;

    private bool lastIsNight;

    public enum LanternState
    {
        Off,
        TurningOn,
        On,
        TurningOff
    }

    void Awake()
    {
        lanternLight = GetComponent<Light>();
        lanternLight.intensity = 0f;
        lanternLight.enabled = true;
        startDelay = Random.Range(0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        bool isNight = !DayAndNightAdjustment.IsDay;

        if (isNight != lastIsNight)
        {
            if (isNight)
            {
                SwitchState(LanternState.TurningOn);
            }
            else
            {
                SwitchState(LanternState.TurningOff);
            }
        }

        lastIsNight = isNight;
    }

    private void SwitchState(LanternState newState)
    {
        state = newState;

        switch (state)
        {
            case LanternState.TurningOn:
                StartCoroutine(SwitchLight(true));
                break;
            case LanternState.TurningOff:
                StartCoroutine(SwitchLight(false));
                break;
        }
    }
    
    private IEnumerator SwitchLight(bool shouldBeOn)
    {
        if (!shouldBeOn && flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        yield return new WaitForSeconds(startDelay);

        if (shouldBeOn)
        {
            yield return StartCoroutine(FadeLight(0f, minLightIntensity, fadeDuration));

            state = LanternState.On;
            flickerRoutine = StartCoroutine(Flicker());
        }
        else 
        {
            float lightIntensity = lanternLight.intensity;
            
            yield return StartCoroutine(FadeLight(lightIntensity, 0f, fadeDuration));
            state = LanternState.Off;
        }
    }

    private IEnumerator Flicker()
    {
        while (state == LanternState.On)
        {
            yield return StartCoroutine(FadeLight(minLightIntensity, maxLightIntensity, flickerSpeed));
            yield return StartCoroutine(FadeLight(maxLightIntensity, minLightIntensity, flickerSpeed));
        }
    }

    private IEnumerator FadeLight(float start, float end, float duration)
    {
        float elapsedDelay = 0f;

        while (elapsedDelay < duration)
        {
            elapsedDelay += Time.deltaTime;
            lanternLight.intensity = Mathf.Lerp(start, end, elapsedDelay / duration);
            yield return null;
        }

        lanternLight.intensity = end;
    }
}
