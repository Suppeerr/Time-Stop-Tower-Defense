using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    private int money;
    [SerializeField] private TMP_Text moneyText;

    void Awake()
    {
        money = 100;
        UpdateUI();
    }

    // Calls whenever money changes
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: $" + money;
        }
    }

    // Public method for collecting coins
    public void CollectCoin(int amount = 1)
    {
        money += amount;
        UpdateUI();
    }

    // Decreases money count by indicated amount
    public void DecreaseMoney(int decreaseAmount)
    {
        money -= decreaseAmount;
        UpdateUI();
    }

    // Returns money stored in the manager
    public int GetMoney()
    {
        return money;
    }
}
