using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    // UI Sound Manager Instance
    public static UISoundManager Instance;

    // Left and right click sounds
    [SerializeField] private AudioSource leftClickSound;
    [SerializeField] private AudioSource rightClickSound;

    private void Awake()
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

    // Plays either the left or right click sound
    public void PlayClickSound(bool isLeft)
    {
        if (isLeft)
        {
            leftClickSound.Play();
        }
        else
        {
            rightClickSound.Play();
        }
    }
}
