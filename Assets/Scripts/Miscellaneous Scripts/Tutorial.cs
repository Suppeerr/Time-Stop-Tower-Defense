using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialImages;
    [SerializeField] private TMP_Text levelStartIndicator;
    [SerializeField] private TMP_Text moneyIndicator;
    [SerializeField] private TMP_Text storedTimeIndicator;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject exitButton;
    private int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraSwitch.IsTutorialActive)
        {
            StartTutorial();   
        }
    }

    private void StartTutorial()
    {
        if (levelStartIndicator.enabled == true)
        {
            levelStartIndicator.enabled = false;
            moneyIndicator.enabled = false;
            storedTimeIndicator.enabled = false;

            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
            exitButton.SetActive(true);
        }
    }

    public void EndTutorial()
    {
        levelStartIndicator.enabled = true;
        moneyIndicator.enabled = true;
        storedTimeIndicator.enabled = true;

        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        exitButton.SetActive(false);

        CameraSwitch.Instance.ToggleTutorial(false);
    }

    public void NextImage()
    {
        Debug.Log("Right Arrow Clicked!");
        if (currentIndex == tutorialImages.Count - 1)
        {
            return;
        }

        currentIndex++;
        UpdateImage();
    }

    public void PreviousImage()
    {
        Debug.Log("Left Arrow Clicked!");
        if (currentIndex == 0)
        {
            return;
        }

        currentIndex--;
        UpdateImage();
    }

    private void UpdateImage()
    {
        for (int i = 0; i < tutorialImages.Count; i++)
        {
            tutorialImages[i].SetActive(i == currentIndex);
        }
    }
}
