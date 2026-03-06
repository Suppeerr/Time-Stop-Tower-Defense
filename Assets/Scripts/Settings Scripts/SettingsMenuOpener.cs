using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SettingsMenuOpener : MonoBehaviour
{
    // Open sett
    public static SettingsMenuOpener Instance;

    // Settings menu object
    [SerializeField] private GameObject settingsMenu;

    // Settings button animator
    [SerializeField] private Animator buttonAnimator;

    // Settings menu animator
    [SerializeField] private Animator menuAnimator;

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
        // Opens the settings when the escape key is pressed
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UpdateMenu();
        }
    }
    
    // Switches the menu opened boolean when menu opened/closed 
    public void UpdateMenu()
    {
        MenuOpened = !MenuOpened;
        
        UpdatePause();
        StartCoroutine(UpdateMenuVisuals());
    }

    // Updates the appearance of the settings menu when opened or closed
    private IEnumerator UpdateMenuVisuals()
    {
        // Opens or closes the menu 
        if (MenuOpened && !settingsMenu.activeSelf)
        {
            buttonAnimator.SetTrigger("Open");
            settingsMenu.SetActive(true);
            UISoundManager.Instance.PlayClickSound(false);
            menuAnimator.SetTrigger("Fade In");
            SettingsManager.Instance.OpenSettings();
        }
        else if (!MenuOpened && settingsMenu.activeSelf)
        {
            buttonAnimator.SetTrigger("Close");
            menuAnimator.SetTrigger("Fade Out");
            UISoundManager.Instance.PlayClickSound(true);
            yield return new WaitForSeconds(1f / 6f);
            settingsMenu.SetActive(false);
            SettingsManager.Instance.CloseSettings();
        }
    }

    // Updates the time scale in response to whether the settings are open
    public void UpdatePause()
    {
        if (!MenuOpened && Time.timeScale == 0f && !TimeStop.Instance.IsFrozen)
        {
            Time.timeScale = 1f;
        } 
        else 
        {
            Time.timeScale = 0f;
        }
    }
}
