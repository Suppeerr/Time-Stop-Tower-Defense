using UnityEngine;

public class OpenSettingsMenu : MonoBehaviour
{
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private GameObject settingsMenu;
    public static bool IsMenuOpen { get; private set; }

    void Start()
    {
        IsMenuOpen = false;
    }

    void Update()
    {
        if (IsMenuOpen && !settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(true);
            settingsManager.OpenSettings();
        }
        else if (!IsMenuOpen && settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            settingsManager.CloseSettings();
        }
    }
    
    public void UpdateMenu()
    {
        IsMenuOpen = !IsMenuOpen;
        UpdatePause();
    }

    public void UpdatePause()
    {
        if (!IsMenuOpen && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        } 
        else 
        {
            Time.timeScale = 0f;
        }
    }
}
