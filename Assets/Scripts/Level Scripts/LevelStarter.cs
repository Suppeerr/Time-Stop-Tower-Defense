using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelStarter : MonoBehaviour
{
    // Level starter instance
    public static LevelStarter Instance;

    // Tutorial image
    [SerializeField] private Image tutorialImage;

    // Static level start boolean
    public static bool HasLevelStarted { get; private set; }

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

        HasLevelStarted = false;
    }

    void Update()
    {
        // Starts level if enter key pressed
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartLevel();
        }
    }

    // Starts the level
    public void StartLevel()
    {
        HasLevelStarted = true;
        gameObject.SetActive(false);
        tutorialImage.enabled = false;
        UISoundManager.Instance.PlayClickSound(false);
    }
}
