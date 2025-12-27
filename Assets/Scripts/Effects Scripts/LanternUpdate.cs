using UnityEngine;
using System.Collections;

public class LanternUpdate : MonoBehaviour
{
    // Light and flicker routine fields
    private Light lanternLight;
    private Coroutine flickerRoutine = null;

    // Initialize state to off
    private LanternState state = LanternState.Off;

    // Lantern fading/initial fade delay
    private float startDelay;
    private float fadeDuration = 1.5f;

    // Lantern flickering
    private float minLightIntensity = 10f;
    private float maxLightIntensity = 25f;
    private float flickerSpeed = 2f;

    // Stored value to prevent unnecessary updates every frame
    private bool lastIsNight;

    // Lantern window fields
    [SerializeField] private GameObject lanternWindow;
    [SerializeField] private Material onLanternMat;
    [SerializeField] private Material offLanternMat;

    // Defines the lantern's states
    public enum LanternState
    {
        Off,
        TurningOn,
        On,
        TurningOff
    }

    void Awake()
    {
        // Initializes fields
        lanternLight = GetComponent<Light>();
        lanternLight.intensity = 0f;
        lanternLight.enabled = true;
        startDelay = Random.Range(0f, 0.5f);
    }

    void Update()
    {
        // Updates boolean to match time of day
        bool isNight = !DayAndNightAdjustment.IsDay;

        // Checks whether lantern should be on or off
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

    // Switches lantern state to turning on or turning off
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
    
    // Animates light turning on or off
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
            // Turning on
            lanternWindow.GetComponent<Renderer>().material = onLanternMat;
            yield return StartCoroutine(FadeLight(0f, minLightIntensity, fadeDuration));

            state = LanternState.On;
            flickerRoutine = StartCoroutine(Flicker());
        }
        else 
        {
            // Turning off
            float lightIntensity = lanternLight.intensity;
            
            yield return StartCoroutine(FadeLight(lightIntensity, 0f, fadeDuration));
            lanternWindow.GetComponent<Renderer>().material = offLanternMat;
            state = LanternState.Off;
        }
    }

    // Flickers the lantern's intensity when on
    private IEnumerator Flicker()
    {
        while (state == LanternState.On)
        {
            yield return StartCoroutine(FadeLight(minLightIntensity, maxLightIntensity, flickerSpeed));
            yield return StartCoroutine(FadeLight(maxLightIntensity, minLightIntensity, flickerSpeed));
        }
    }

    // Animates changes to lantern intensity
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
