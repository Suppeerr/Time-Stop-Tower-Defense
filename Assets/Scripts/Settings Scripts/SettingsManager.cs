using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] Slider masterSlider;

    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioSource musicSource;

    [SerializeField] Slider dmgIndicatorSlider;
    [SerializeField] EnemyDamageIndicator dmgIndicatorScript;

    [SerializeField] Toggle dayNightFreezeToggle;
    [SerializeField] DayAndNightAdjustment dayNightAdjustScript;

    public bool SettingsOpened { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        AdjustVolume();
    }

    public void OpenSettings()
    {
        SettingsOpened = true;
        musicSource.pitch = 0.9f;
        LimitVolume(-5f);
    }

    public void CloseSettings()
    {
        SettingsOpened = false;
        musicSource.pitch = 1f;
        AdjustVolume();
    }

    public void AdjustVolume()
    {
        SetVolume();

        if (OpenSettingsMenu.IsMenuOpen)
        {
            LimitVolume(-5f);
        }
    }

    private void LimitVolume(float maxVolume)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Min(maxVolume, masterSlider.value));
        masterMixer.SetFloat("SfxVolume", Mathf.Min(maxVolume, soundSlider.value));
        masterMixer.SetFloat("MusicVolume", Mathf.Min(maxVolume, musicSlider.value));
    }

    private void SetVolume()
    {
        masterMixer.SetFloat("SfxVolume", soundSlider.value);
        masterMixer.SetFloat("MusicVolume", musicSlider.value);
        masterMixer.SetFloat("MasterVolume", masterSlider.value);

        if (soundSlider.value <= -39.5f || masterSlider.value <= -39.5f)
        {
            masterMixer.SetFloat("SfxVolume", -80f);
        }

        if (musicSlider.value <= -39.5f || masterSlider.value <= -39.5f)
        {
            musicSource.mute = true;
        }
        else
        {
            musicSource.mute = false;
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
