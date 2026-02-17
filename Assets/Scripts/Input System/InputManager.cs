using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Input manager instance
    public static InputManager Instance { get; private set; }

    // Generic input field
    public GameInput Input { get; private set; }

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

        Input = new GameInput();
        Input.Enable();
    }

    void OnDestroy()
    {
        Input.Disable();     
    }
}
