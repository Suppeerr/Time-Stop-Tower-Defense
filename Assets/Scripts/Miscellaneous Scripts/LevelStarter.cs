using UnityEngine;
using UnityEngine.InputSystem;

public class LevelStarter : MonoBehaviour
{
    public static LevelStarter Instance;
    public static bool HasLevelStarted { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Freezes time until level starts
        Time.timeScale = 0f;
        HasLevelStarted = false;
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            HasLevelStarted = true;
            gameObject.SetActive(false);
        }
    }
}
