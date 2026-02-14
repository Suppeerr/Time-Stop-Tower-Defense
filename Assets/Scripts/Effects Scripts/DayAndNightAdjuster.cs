using UnityEngine;
using System.Collections;

public class DayAndNightAdjuster : MonoBehaviour
{
    // Day and night adjuster instance
    public static DayAndNightAdjuster Instance;

    // Directional lights
    [SerializeField] private Light sunDirLight;
    [SerializeField] private Light moonDirLight;

    // Moon rotation adjustment
    [SerializeField] private float moonRotation;
    
    // Temperature fields
    [SerializeField] private AnimationCurve temperatureCurve;
    private float minTemp = 2600f;
    private float maxTemp = 13000f;

    // Day/night duration
    private float cycleDuration = 250f;

    // Loop/synced time variables
    private float elapsed = 0f;
    private float time;

    // Skybox fields
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Gradient dayTintGradient;

    // Determines whether the cycle is on or off
    private bool isCycleOn = true;

    // Firefly VFX
    [SerializeField] private GameObject fireflies;

    // Static day checking boolean
    public static bool IsDay { get; private set; }

    void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize fields
        IsDay = true;
        moonDirLight.enabled = false;
    }

    void Update()
    {
        UpdateCycle();
    }

    private void UpdateCycle()
    {
        // Updates time of day field 
        if (isCycleOn)
        {
            elapsed += Time.deltaTime;
        }
        
        time = Mathf.Repeat((elapsed / cycleDuration) + 0.1f, 0.96f);

        UpdateTemperature();
        UpdateRotation();
        UpdateSkyBox();
        RotateSkyBox();
    }

    // Rotates the active skybox with time
    private void RotateSkyBox()
    {
        if (IsDay)
        {
            daySkybox.SetFloat("_Rotation", time * 150f);
        }
        else
        {
            nightSkybox.SetFloat("_Rotation", time * 150f);
        }
    }

    // Switches and tints the skybox according to time
    private void UpdateSkyBox()
    {
        if (time < 0.52f && IsDay == false)
        {
            IsDay = true;
            fireflies.SetActive(false);
            RenderSettings.skybox = daySkybox;
        }
        else if (time > 0.52f && IsDay == true)
        {
            IsDay = false;
            fireflies.SetActive(true);
            RenderSettings.skybox = nightSkybox;
        }

        if (IsDay)
        {
            daySkybox.SetColor("_TintColor", dayTintGradient.Evaluate(time * 2f));
        }
    }

    // Updates the light's temperature according to the animation curve
    private void UpdateTemperature()
    {
        float t = temperatureCurve.Evaluate(time);
        sunDirLight.colorTemperature = Mathf.Lerp(minTemp, maxTemp, t);
    }

    // Rotates the active light according to time
    private void UpdateRotation()
    {
        if (!IsDay && moonDirLight.enabled == false)
        {
            sunDirLight.enabled = false;
            moonDirLight.enabled = true;
        }
        else if (IsDay && moonDirLight.enabled == true)
        {
            sunDirLight.enabled = true;
            moonDirLight.enabled = false;
        }

            sunDirLight.transform.rotation = Quaternion.Euler(time * 360f, -15f, 0f);
            moonDirLight.transform.rotation = Quaternion.Euler(time * moonRotation, -10f, 0f);
    }

    public void ToggleCycle(bool freezeToggle)
    {
        isCycleOn = !freezeToggle;
    }

    public void ResetCycle()
    {
        elapsed = 0f;
        UpdateCycle();
    }
}
