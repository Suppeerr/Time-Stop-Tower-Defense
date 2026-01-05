using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BaseHealthManager : MonoBehaviour
{
    public static BaseHealthManager Instance;
    [SerializeField] private List<HpThreshold> thresholds = new List<HpThreshold>();
    [SerializeField] private Image currentHpImage;

    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text levelRestartText;
    [SerializeField] private TMP_Text levelWinText;

    public static bool IsGameOver { get; private set; }
    private int startingBaseHp;
    private int currentBaseHp;
    private bool imageEnabled = false;

    // Avoids duplicates of this object
    private void Awake()
    {
        currentHpImage.sprite = thresholds[0].image;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        IsGameOver = false;
        startingBaseHp = 500;
        currentBaseHp = startingBaseHp;
        UpdateUI(false);

        currentHpImage.enabled = false;
        gameOverText.enabled = false;
        levelRestartText.enabled = false;
        levelWinText.enabled = false;
    }

    void Update()
    {
        if (LevelStarter.HasLevelStarted && imageEnabled == false)
        {
            imageEnabled = true;
            currentHpImage.enabled = true;
        }
        if (currentBaseHp <= 0)
        {
            ToggleGameOver();
        }
        if (IsGameOver && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            RestartLevel();
        }
    }

    // Calls whenever base hp changes
    private void UpdateUI(bool shouldChange)
    {
        if (currentBaseHp <= 0 || IsGameOver)
        {
            gameOverText.enabled = true;
            return;
        }

        if (shouldChange)
        {
            HpThreshold match = null;
            float hpPercent = (float) currentBaseHp / startingBaseHp;

            foreach (HpThreshold threshold in thresholds)
            {
                if (hpPercent >= threshold.percent && (match == null || match.percent < threshold.percent))
                {
                    match = threshold;
                }
            }

            currentHpImage.sprite = match.image;
        }
    }

    // Public method for updating base hp
    public void UpdateBaseHP(int amount)
    {
        if (IsGameOver || (amount > 0 && currentBaseHp >= startingBaseHp))
        {
            return;
        }

        currentBaseHp += amount;
        UpdateUI(true);
    }

    // Returns base hp stored in the manager
    public int GetBaseHp()
    {
        return currentBaseHp;
    }

    public void ToggleWin()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        levelWinText.enabled = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        // Freezes time
        Time.timeScale = 0f;
    }

    public void ToggleGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        levelRestartText.enabled = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        // Freezes time
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        Time.timeScale = 1f;
    }
}
