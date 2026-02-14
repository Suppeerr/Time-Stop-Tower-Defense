using UnityEngine;
using TMPro;

public class StoredTimeManager : MonoBehaviour
{
    // Stored time manager instance
    public static StoredTimeManager Instance;

    // Stored seconds
    private int seconds;

    // Stored seconds cap
    private int maxSeconds = 25;

    // Stored seconds UI
    [SerializeField] private TMP_Text storedTimeText;

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

        // Initializes seconds amount
        seconds = 0;
        UpdateUI();
    }

    void Update()
    {
        // Changes indicator color during time stop
        if (TimeStop.Instance.IsFrozen)
        {
            storedTimeText.color = Color.yellow;
        }
        else
        {
            storedTimeText.color = Color.white;
        }

        // Moves the seconds indicator for specific cameras
        if (CameraSwitcher.Instance.ActiveCam == 2)
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(138f, 132f);
        }
        else
        {
            storedTimeText.rectTransform.anchoredPosition = new Vector3(770f, 132f);
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
        if (isNeg && seconds + amount >= 0)
        {
            seconds -= amount;
        }
        else if (seconds < maxSeconds || amount < 0)
        {
            seconds += amount;
        }
        
        UpdateUI();
    }

    // Returns seconds stored
    public int GetSeconds()
    {
        return seconds;
    }
}
