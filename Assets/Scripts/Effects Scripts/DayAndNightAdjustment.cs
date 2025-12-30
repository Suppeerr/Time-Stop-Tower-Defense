using UnityEngine;
using System.Collections;

public class DayAndNightAdjustment : MonoBehaviour
{
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
    private float cycleDuration = 300f;

    // Loop/synced time variables
    private float elapsed = 0f;
    private float time;

    // Skybox fields
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Gradient dayTintGradient;

    // Determines whether the cycle is on or off
    private bool isCycleOn = true;

    // Static day checking boolean
    public static bool IsDay { get; private set; }

    void Start()
    {
        // Initialize fields
        IsDay = true;
        moonDirLight.enabled = false;
    }

    void Update()
    {
        // Freezes the cycle under certain conditions
        if (ProjectileManager.IsFrozen)
        {
            return;
        }

        UpdateCycle();
    }

    private void UpdateCycle()
    {
        // Updates time of day field 
        if (isCycleOn)
        {
            elapsed += Time.deltaTime;
        }
        
        time = Mathf.Repeat((elapsed / cycleDuration) + 0.1f, 1f);

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
        if (time < 0.55f && IsDay == false)
        {
            IsDay = true;
            RenderSettings.skybox = daySkybox;
        }
        else if (time > 0.55f && IsDay == true)
        {
            IsDay = false;
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
