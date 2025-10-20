using UnityEngine;
using TMPro;

public class MoneyManagement : MonoBehaviour
{
    public int money;
    [SerializeField] private TMP_Text moneyText;

    void Awake()
    {
        moneyText.text = "Money?: ";
        money = 0;
        UpdateUI();
    }

    // Calls whenever money changes
    private void UpdateUI()
    {
        Debug.Log("Inside UpdateUI");
        if (moneyText != null)
        {
            Debug.Log("Updating Money Count");
            moneyText.text = "Money: " + money;
        }
    }

    // Public method for collecting coins
    public void CollectCoin(int amount = 1)
    {
        money += amount;
        UpdateUI();
    }
}
