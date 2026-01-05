using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    // Stored money
    private int money;

    // Money stored UI
    [SerializeField] private TMP_Text moneyText;

    void Awake()
    {
        // Initializes money amount
        money = 10;
        UpdateUI();
    }

    void Update()
    {
        // Moves the money indicator for specific cameras
        if (CameraSwitch.ActiveCam == 2)
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(124, 187);
        }
        else
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(761, 187);
        }
    }

    // Changes indicated money count to the stored money amount
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }
    }

    // Increments money by amount
    public void UpdateMoney(int amount = 1, bool isNeg = false)
    {
        if (isNeg)
        {
            money -= amount;
        }
        else
        {
            money += amount;
        }
        
        UpdateUI();
    }

    // Returns money stored
    public int GetMoney()
    {
        return money;
    }
}
