using UnityEngine;
using TMPro;

public class StoredTimeManager : MonoBehaviour
{
    private int seconds;
    private int maxSeconds = 0;
    [SerializeField] private TMP_Text storedTimeText;

    void Awake()
    {
        seconds = 25;
        UpdateUI();
    }

    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            storedTimeText.color = Color.yellow;
        }
        else
        {
            storedTimeText.color = Color.white;
        }

        if (CameraSwitch.ActiveCam == 2)
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(-292, 140, 0);
        }
        else
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(296, 140, 0);
        }
    }

    // Calls whenever seconds changes
    private void UpdateUI()
    {
        if (storedTimeText != null)
        {
            storedTimeText.text = seconds + "s Stored";
        }
    }

    // Public method for updating seconds
    public void UpdateSeconds(int amount = 1)
    {
        if (seconds < maxSeconds)
        {
            seconds += amount;
        }
        
        UpdateUI();
    }

    // Decreases second count by indicated amount
    public void DecreaseSeconds(int decreaseAmount)
    {
        seconds -= decreaseAmount;
        UpdateUI();
    }

    // Returns seconds stored in the manager
    public int GetSeconds()
    {
        return seconds;
    }
}
