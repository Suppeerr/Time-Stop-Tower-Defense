using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class SettingsManager : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] Slider masterSlider;

    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioSource musicSource;

    [SerializeField] Slider dmgIndicatorSlider;
    [SerializeField] EnemyDamageIndicator dmgIndicatorScript;

    [SerializeField] Toggle dayNightFreezeToggle;
    [SerializeField] DayAndNightAdjustment dayNightAdjustScript;

    void Start()
    {
        AdjustVolume();
    }

    public void OpenSettings()
    {
        musicSource.pitch = 0.9f;
        LimitVolume(-5f);
    }

    public void CloseSettings()
    {
        musicSource.pitch = 1f;
        AdjustVolume();
        AdjustVolume("Sound");
        AdjustVolume("Music");
    }

    public void AdjustMaster()
    {
        AdjustVolume();
    }

    public void AdjustSound()
    {
        AdjustVolume("Sound");
    }

    public void AdjustMusic()
    {
        AdjustVolume("Music");
    }

    private void AdjustVolume(string parameter = null)
    {
        if (OpenSettingsMenu.IsMenuOpen)
        {
            LimitVolume(-5f);

            if (soundSlider.value > -39.5f || musicSlider.value > -39.5f || masterSlider.value > -39.5f)
            {
                return;
            }
        }

        if (parameter == "Sound")
        {
            if (soundSlider.value <= -39.5f)
            {
                masterMixer.SetFloat("SfxVolume", -80f);
                return;
            }

            masterMixer.SetFloat("SfxVolume", soundSlider.value);
        }
        else if (parameter == "Music")
        {
            if (musicSlider.value <= -39.5f)
            {
                musicSource.mute = true;
            }
            else
            {
                musicSource.mute = false;
            }

            masterMixer.SetFloat("MusicVolume", musicSlider.value);
        }
        else
        {
            if (masterSlider.value <= -39.5f)
            {
                musicSource.mute = true;
            }
            else
            {
                musicSource.mute = false;
            }

            masterMixer.SetFloat("MasterVolume", masterSlider.value);
        }
    }

    private void LimitVolume(float maxVolume)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Min(maxVolume, masterSlider.value));
        masterMixer.SetFloat("SfxVolume", Mathf.Min(maxVolume, soundSlider.value));
        masterMixer.SetFloat("MusicVolume", Mathf.Min(maxVolume, musicSlider.value));
    }

    private void SetMinimumVolume()
    {
        if (masterSlider.value == -40f)
        {
            masterMixer.SetFloat("MasterVolume", -80f);
        }

        if (soundSlider.value == -40f)
        {
            masterMixer.SetFloat("SfxVolume", -80f);
        }

        if (musicSlider.value == -40f)
        {
            masterMixer.SetFloat("MusicVolume", -80f);
        }
    }

    public void AdjustDamageIndicator()
    {
        dmgIndicatorScript.UpdateIndicatorSize(dmgIndicatorSlider.value);
    }

    public void FreezeDayNightCycle()
    {
        dayNightAdjustScript.ToggleCycle(dayNightFreezeToggle.isOn);
    }

    public void ResetDayNightCycle()
    {
        dayNightAdjustScript.ResetCycle();
    }

    public void RestartLevel()
    {
        BaseHealthManager.Instance.RestartLevel();
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene("UI and Main Menu", LoadSceneMode.Single);
    }
}
