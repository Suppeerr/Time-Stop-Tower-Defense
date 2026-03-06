using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class LevelStarter : MonoBehaviour
{
    // Level starter instance
    public static LevelStarter Instance;

    // Tutorial start button
    [SerializeField] private GameObject tutorialStartButton;

    // Level start and tutorial slide animator
    [SerializeField] private Animator startSlideAnimator;
    [SerializeField] private Animator tutorialSlideAnimator;

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
        startSlideAnimator.SetTrigger("Slide");
        tutorialSlideAnimator.SetTrigger("Slide");
        StartCoroutine(HideUI());

        HasLevelStarted = true;
        UISoundManager.Instance.PlayClickSound(false);
    }

    // Hides the level start UI after the level starts
    public IEnumerator HideUI()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
        tutorialStartButton.SetActive(false);
    }
}
