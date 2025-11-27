using UnityEngine;

public class DayAndNightAdjustment : MonoBehaviour
{
    public Material daySkybox;
    public Material nightSkybox;
    private Light dirLight;
    private float elapsed = 0f;
    private float switchInterval = 20f;
    private float rotation = 0f;
    private bool isDay = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dirLight = gameObject.GetComponent<Light>();
        ChangeToDay();
    }

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        
        if (elapsed < switchInterval)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            elapsed = 0;
            isDay = !isDay;

            if (isDay)
            {
                ChangeToDay();
            }
            else
            {
                ChangeToNight();
            }
        }

        RenderSettings.skybox.SetFloat("_Rotation", rotation);
        if (rotation < 360f)
        {
            rotation += Time.deltaTime;
        }
        else
        {
            rotation = 0f;
        }
        
    }

    private void ChangeToDay()
    {
        RenderSettings.skybox = daySkybox;
        dirLight.colorTemperature = 6000f;
    }

    private void ChangeToNight()
    {
        RenderSettings.skybox = nightSkybox;
        dirLight.colorTemperature = 13000f;
    }
}
