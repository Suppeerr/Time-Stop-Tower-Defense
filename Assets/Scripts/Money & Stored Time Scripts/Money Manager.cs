using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    private int money;
    [SerializeField] private TMP_Text moneyText;

    void Awake()
    {
        money = 10;
        UpdateUI();
    }

    void Update()
    {
        if (CameraSwitch.ActiveCam == 2)
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(124, 187);
        }
        else
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(761, 187);
        }
    }

    // Calls whenever money changes
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }
    }

    // Increments money by amount
    public void UpdateMoney(int amount = 1)
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
