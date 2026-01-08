using UnityEngine;

public class SettingsMenuOpener : MonoBehaviour
{
    // Open sett
    public static SettingsMenuOpener Instance;

    // Settings menu object
    [SerializeField] private GameObject settingsMenu;

    // Menu opened boolean
    public bool MenuOpened { get; private set; } = false;

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

    void Update()
    {
        // Opens or closes the menu 
        if (MenuOpened && !settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(true);
            SettingsManager.Instance.OpenSettings();
        }
        else if (!MenuOpened && settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            SettingsManager.Instance.CloseSettings();
        }
    }
    
    // Switches the menu opened boolean when menu opened/closed 
    public void UpdateMenu()
    {
        MenuOpened = !MenuOpened;
        UpdatePause();
    }

    // Updates the time scale in response to whether the settings are open
    public void UpdatePause()
    {
        if (!MenuOpened && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        } 
        else 
        {
            Time.timeScale = 0f;
        }
    }
}
