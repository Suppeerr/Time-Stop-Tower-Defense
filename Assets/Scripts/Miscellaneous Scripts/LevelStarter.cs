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

        HasLevelStarted = false;
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HasLevelStarted = true;
            
            gameObject.SetActive(false);
        }
    }
}
