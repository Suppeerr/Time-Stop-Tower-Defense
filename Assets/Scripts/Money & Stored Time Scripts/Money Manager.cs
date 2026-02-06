using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    // Money manager instance
    public static MoneyManager Instance;
    
    // Stored money
    private int money;

    // Money stored UI
    [SerializeField] private TMP_Text moneyText;

    // Coin drop sound effect
    [SerializeField] private AudioSource coinDrop;

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

        // Initializes money amount
        money = 10;
        UpdateUI();
    }

    void Update()
    {
        // Moves the money indicator for specific cameras
        if (CameraSwitcher.Instance.ActiveCam == 2)
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(124f, 187f);
        }
        else
        {
            moneyText.rectTransform.anchoredPosition = new Vector2(761f, 187f);
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
            coinDrop.Play();
        }
        
        UpdateUI();
    }

    // Returns money stored
    public int GetMoney()
    {
        return money;
    }
}
