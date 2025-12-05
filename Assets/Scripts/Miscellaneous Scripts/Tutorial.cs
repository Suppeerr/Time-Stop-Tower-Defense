using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialImages;
    [SerializeField] private TMP_Text levelStartIndicator;
    [SerializeField] private TMP_Text moneyIndicator;
    [SerializeField] private TMP_Text storedTimeIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialImages = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraSwitch.IsTutorialActive)
        {
            levelStartIndicator.enabled = false;
            moneyIndicator.enabled = false;
            storedTimeIndicator.enabled = false;
        }
    }
}
