using UnityEngine;
using System.Collections;

public class DayAndNightAdjustment : MonoBehaviour
{
    public Material daySkybox;
    public Material nightSkybox;
    [SerializeField] private Light sunDirLight;
    [SerializeField] private Light moonDirLight;
    [SerializeField] private float moonRotation;
    
    [SerializeField] private AnimationCurve temperatureCurve;
    private float minTemp = 2600f;
    private float maxTemp = 13000f;

    private float cycleDuration = 400f;

    private float elapsed = 0f;
    private float time;

    public static bool IsDay { get; private set; }

    void Start()
    {
        IsDay = true;
        moonDirLight.enabled = false;
    }

    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        
        elapsed += Time.deltaTime;

        time = Mathf.Repeat((elapsed / cycleDuration) + 0.1f, 1f);

        UpdateTemperature();
        UpdateRotation();
        UpdateSkyBox();
        RotateSkyBox();
    }

    private void RotateSkyBox()
    {
        RenderSettings.skybox.SetFloat("_Rotation", time * 150f);
    }

    private void UpdateSkyBox()
    {
        if (time > 0.02f && time < 0.5f && IsDay == false)
        {
            IsDay = true;
            RenderSettings.skybox = daySkybox;
        }
        else if (time < 0.02f || time > 0.5f && IsDay == true)
        {
            IsDay = false;
            RenderSettings.skybox = nightSkybox;
        }
    }

    private void UpdateTemperature()
    {
        float t = temperatureCurve.Evaluate(time);
        sunDirLight.colorTemperature = Mathf.Lerp(minTemp, maxTemp, t);
    }

    private void UpdateRotation()
    {
        float rotationAngle = Mathf.Lerp(0f, 360f, time);
        if (time > 0.6f)
        {
            sunDirLight.enabled = false;
            moonDirLight.enabled = true;
        }
        else if (moonDirLight.enabled == true)
        {
            sunDirLight.enabled = true;
            moonDirLight.enabled = false;
        }

        sunDirLight.transform.rotation = Quaternion.Euler(time * 360f, -15f, 0f);
        moonDirLight.transform.rotation = Quaternion.Euler(time * moonRotation, -10f, 0f);
    }
}
