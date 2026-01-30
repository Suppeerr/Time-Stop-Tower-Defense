using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Tutorial manager instance
    public static TutorialManager Instance;

    // Tutorial page images and current page number
    [SerializeField] private List<GameObject> tutorialImages;
    private int currentIndex = 0;

    // Screen UI
    [SerializeField] private List<GameObject> screenUI;

    // Tutorial UI
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject exitButton;
    private Button leftArrowButton;
    private Button rightArrowButton;

    // Tutorial active boolean
    public bool IsTutorialActive { get; private set; }

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

    void Start()
    {
        // Initialize fields and image
        IsTutorialActive = false;
        leftArrowButton = leftArrow.GetComponent<Button>();
        rightArrowButton = rightArrow.GetComponent<Button>();
        
        UpdateImage();
    }

    void Update()
    {
        // Starts tutorial if m key pressed and level has not started
        if (!LevelStarter.HasLevelStarted && Keyboard.current.mKey.wasPressedThisFrame && !IsTutorialActive)
        {
            StartTutorial();
        }    

        // Disables the left button on the first page
        if (currentIndex == 0 && leftArrowButton.interactable == true)
        {
            leftArrowButton.interactable = false;
        }

        // Disables the right button on the last page
        if (currentIndex == tutorialImages.Count - 1 && rightArrowButton.interactable == true)
        {
            rightArrowButton.interactable = false;
        }
    }

    // Starts the tutorial
    public void StartTutorial()
    {
        IsTutorialActive = true;
        UISoundManager.Instance.PlayClickSound(false);

        DayAndNightAdjuster.Instance.ToggleCycle(true);

        CameraSwitcher.Instance.ToggleTutorial(true);
    }

    // Ends the tutorial
    public void EndTutorial()
    {
        IsTutorialActive = false;
        UISoundManager.Instance.PlayClickSound(false);
        currentIndex = 0;

        DayAndNightAdjuster.Instance.ToggleCycle(false);

        CameraSwitcher.Instance.ToggleTutorial(false);
    }

    // Enables or disables the screen UI
    public void UpdateScreenUI(bool enabled)
    {
        foreach (GameObject canvas in screenUI)
        {
            canvas.SetActive(enabled);
        }
    }

    // Enables or disables the tutorial UI
    public void UpdateTutorialUI(bool enabled)
    {
        leftArrow.SetActive(enabled);
        rightArrow.SetActive(enabled);
        exitButton.SetActive(enabled);
    }

    // Displays the previous tutorial page
        public void PreviousImage()
        {
            currentIndex--;
            UpdateImage();
            UISoundManager.Instance.PlayClickSound(true);

            if (rightArrowButton.interactable == false)
            {
            rightArrowButton.interactable = true; 
            }
        }  

    // Displays the next tutorial page
    public void NextImage()
    {
        currentIndex++;
        UpdateImage();
        UISoundManager.Instance.PlayClickSound(false);

        if (leftArrowButton.interactable == false)
        {
            leftArrowButton.interactable = true;
        }
    }

    // Shows the tutorial image corresponding to the current page
    public void UpdateImage()
    {
        for (int i = 0; i < tutorialImages.Count; i++)
        {
            tutorialImages[i].SetActive(i == currentIndex);
        }
    }
}
