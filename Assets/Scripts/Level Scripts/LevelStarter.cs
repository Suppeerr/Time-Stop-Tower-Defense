using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelStarter : MonoBehaviour
{
    public static LevelStarter Instance;
    [SerializeField] private Image tutorialImage;
    public static bool HasLevelStarted { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        HasLevelStarted = false;
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartLevel();
        }
    }

    public void StartLevel()
    {
        HasLevelStarted = true;
        gameObject.SetActive(false);
        tutorialImage.enabled = false;
    }
}
