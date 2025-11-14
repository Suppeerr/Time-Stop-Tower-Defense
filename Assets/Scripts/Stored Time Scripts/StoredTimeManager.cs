using UnityEngine;
using TMPro;

public class StoredTimeManager : MonoBehaviour
{
    private int seconds;
    [SerializeField] private TMP_Text storedTimeText;

    void Awake()
    {
        seconds = 0;
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
    }

    // Calls whenever seconds changes
    private void UpdateUI()
    {
        if (storedTimeText != null)
        {
            storedTimeText.text = "Seconds: " + seconds + "s";
        }
    }

    // Public method for updating seconds
    public void UpdateSeconds(int amount = 1)
    {
        seconds += amount;
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
