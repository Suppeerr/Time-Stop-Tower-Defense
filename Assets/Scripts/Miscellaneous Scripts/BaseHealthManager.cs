using UnityEngine;
using TMPro;

public class BaseHealthManager : MonoBehaviour
{
    public static BaseHealthManager Instance;
    [SerializeField] private TMP_Text baseHpText;
    public static bool IsGameOver { get; private set; }
    private int startingBaseHp;
    private int currentBaseHp;

    // Avoids duplicates of this object
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        startingBaseHp = 500;
        currentBaseHp = startingBaseHp;
        UpdateUI();
    }

    void Update()
    {
        if (currentBaseHp <= 0)
        {
            ToggleGameOver();
        }
    }

    // Calls whenever base hp changes
    private void UpdateUI()
    {
        if (baseHpText != null)
        {
            baseHpText.text = currentBaseHp + " HP";
            if (currentBaseHp <= 0)
            {
                baseHpText.text = "Game Over!";
            }
        }
    }

    // Public method for updating base hp
    public void UpdateBaseHP(int amount)
    {
        currentBaseHp += amount;
        UpdateUI();
    }

    // Returns base hp stored in the manager
    public int GetBaseHp()
    {
        return currentBaseHp;
    }

    public void ToggleGameOver()
    {
        if (IsGameOver)
        {
            return;
        }
        IsGameOver = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        // Freezes time
        Time.timeScale = 0f;
    }
}
