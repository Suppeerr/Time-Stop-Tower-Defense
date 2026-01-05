using UnityEngine;
using TMPro;

public class StoredTimeManager : MonoBehaviour
{
    // Stored seconds
    private int seconds;

    // Stored seconds cap
    private int maxSeconds = 25;

    // Stored seconds UI
    [SerializeField] private TMP_Text storedTimeText;

    void Awake()
    {
        // Initializes seconds amount
        seconds = 0;
        UpdateUI();
    }

    void Update()
    {
        // Changes indicator color during time stop
        if (ProjectileManager.Instance.IsFrozen)
        {
            storedTimeText.color = Color.yellow;
        }
        else
        {
            storedTimeText.color = Color.white;
        }

        // Moves the seconds indicator for specific cameras
        if (CameraSwitch.ActiveCam == 2)
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(-270, 132, 0);
        }
        else
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(368, 132, 0);
        }
    }

    // Changes indicated seconds count to the stored seconds amount
    private void UpdateUI()
    {
        if (storedTimeText != null)
        {
            storedTimeText.text = seconds + "s";
        }
    }

    // Increments seconds by amount
    public void UpdateSeconds(int amount = 1, bool isNeg = false)
    {
        if (seconds < maxSeconds)
        {
            if (isNeg)
            {
                seconds -= amount;
            }
            else
            {
                seconds += amount;
            }
        }
        
        UpdateUI();
    }

    // Returns seconds stored
    public int GetSeconds()
    {
        return seconds;
    }
}
