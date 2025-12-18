using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;
    [SerializeField] private List<GameObject> tutorialImages;
    [SerializeField] private Image levelStartImage;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text moneyIndicator;
    [SerializeField] private Image coinImage;
    [SerializeField] private TMP_Text storedTimeIndicator;
    [SerializeField] private Image hourglassImage;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject exitButton;
    private int currentIndex = 0;
    public static bool IsTutorialActive { get; private set; }

    // Avoids duplicates of this object
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsTutorialActive = false;
        
        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelStarter.HasLevelStarted && Keyboard.current.mKey.wasPressedThisFrame && !IsTutorialActive)
        {
            StartTutorial();
        }

        if (currentIndex == 0 && leftArrow.GetComponent<Button>().interactable == true)
        {
            leftArrow.GetComponent<Button>().interactable = false;
        }
        else if (currentIndex != 0 && leftArrow.GetComponent<Button>().interactable == false)
        {
            leftArrow.GetComponent<Button>().interactable = true;
        }

        if (currentIndex == tutorialImages.Count - 1 && rightArrow.GetComponent<Button>().interactable == true)
        {
            rightArrow.GetComponent<Button>().interactable = false;
        }
        else if (currentIndex != tutorialImages.Count - 1 && rightArrow.GetComponent<Button>().interactable == false)
        {
            rightArrow.GetComponent<Button>().interactable = true;
        }
    }

    public void StartTutorial()
    {
        IsTutorialActive = true;

        CameraSwitch.Instance.ToggleTutorial(true);
    }

    public void EndTutorial()
    {
        IsTutorialActive = false;
        currentIndex = 0;

        CameraSwitch.Instance.ToggleTutorial(false);
    }

    public void UpdateScreenUI(bool enabled)
    {
        levelStartImage.enabled = enabled;
        tutorialImage.enabled = enabled;
        moneyIndicator.enabled = enabled;
        coinImage.enabled = enabled;
        storedTimeIndicator.enabled = enabled;
        hourglassImage.enabled = enabled;
    }

    public void UpdateTutorialUI(bool enabled)
    {
        leftArrow.SetActive(enabled);
        rightArrow.SetActive(enabled);
        exitButton.SetActive(enabled);
    }

    public void NextImage()
    {
        currentIndex++;
        UpdateImage();
    }

    public void PreviousImage()
    {
        currentIndex--;
        UpdateImage();
    }

    public void UpdateImage()
    {
        for (int i = 0; i < tutorialImages.Count; i++)
        {
            tutorialImages[i].SetActive(i == currentIndex);
        }
    }
}
