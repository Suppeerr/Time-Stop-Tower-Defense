using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    // Settings manager instance
    public static SettingsManager Instance;

    // Master volume fields
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider masterSlider;

    // Sound effect volume field
    [SerializeField] private Slider soundSlider;

    // Music volume fields
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource musicSource;

    // Damage indicator fields
    [SerializeField] private Slider dmgIndicatorSlider;
    [SerializeField] private EnemyDamageIndicator dmgIndicatorScript;

    // Day/night cycle field
    [SerializeField] private Toggle dayNightFreezeToggle;

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
        // Sets volume to default values
        AdjustVolume();
    }

    // Adjusts music volume and pitch when settings are opened
    public void OpenSettings()
    {
        musicSource.pitch = 0.9f;
        LimitVolume(-5f);
    }

    // Resets music volume and pitch when settings are closed
    public void CloseSettings()
    {
        musicSource.pitch = 1f;
        AdjustVolume();
    }

    // Adjusts volume to slider values
    public void AdjustVolume()
    {
        SetVolume();

        if (SettingsMenuOpener.Instance.MenuOpened)
        {
            LimitVolume(-5f);
        }
    }

    // Limits the volume to a specified max volume
    private void LimitVolume(float maxVolume)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Min(maxVolume, masterSlider.value));
        masterMixer.SetFloat("SfxVolume", Mathf.Min(maxVolume, soundSlider.value));
        masterMixer.SetFloat("MusicVolume", Mathf.Min(maxVolume, musicSlider.value));
    }

    // Sets volumes to their respective slider values when adjusted
    private void SetVolume()
    {
        masterMixer.SetFloat("SfxVolume", soundSlider.value);
        masterMixer.SetFloat("MusicVolume", musicSlider.value);
        masterMixer.SetFloat("MasterVolume", masterSlider.value);

        if (soundSlider.value <= -39.5f || masterSlider.value <= -39.5f)
        {
            masterMixer.SetFloat("SfxVolume", -80f);
        }

        if (BaseHealthManager.Instance.IsGameOver)
        {
            return;
        }

        if ((musicSlider.value <= -39.5f || masterSlider.value <= -39.5f))
        {
            musicSource.mute = true;
        }
        else
        {
            musicSource.mute = false;
        }
    }

    // Manually mutes the music volume
    public void MuteMusicVolume()
    {
        musicSource.mute = true;
    }

    // Tells the damage indicator script to update its indicator sizes to the slider value
    public void AdjustDamageIndicator()
    {
        dmgIndicatorScript.UpdateIndicatorSize(dmgIndicatorSlider.value);
    }

    // Freezes the day/night cycle at the current time
    public void FreezeDayNightCycle()
    {
        StartCoroutine(FreezeCycle());
        UISoundManager.Instance.PlayClickSound(!dayNightFreezeToggle.isOn);
    }

    private IEnumerator FreezeCycle()
    {
        while (TutorialManager.Instance.IsTutorialActive)
        {
            yield return null;
        }

        DayAndNightAdjuster.Instance.ToggleCycle(dayNightFreezeToggle.isOn);
    }

    // Resets the day/night cycle to the default time
    public void ResetDayNightCycle()
    {
        DayAndNightAdjuster.Instance.ResetCycle();
        UISoundManager.Instance.PlayClickSound(false);
    }

    // Restarts the level
    public void RestartLevel()
    {
        UISoundManager.Instance.PlayClickSound(false);
        BaseHealthManager.Instance.RestartLevel();
    }

    // Exits the level and returns to the level selector scene
    public void ExitLevel()
    {
        UISoundManager.Instance.PlayClickSound(false);
        SceneManager.LoadScene("UI and Main Menu", LoadSceneMode.Single);
    }
}
