using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BaseHealthManager : MonoBehaviour
{
    // Base health manager instance
    public static BaseHealthManager Instance;

    // Base health thresholds and the current active image
    [SerializeField] private List<HpThreshold> thresholds = new List<HpThreshold>();
    [SerializeField] private Image currentHpImage;

    // Game over and winning indicators
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text levelRestartText;
    [SerializeField] private TMP_Text levelWinText;

    // Maximum and current base hp
    private int maxBaseHp = 500;
    private int currentBaseHp;

    // Game over static boolean
    public static bool IsGameOver { get; private set; }

    private void Awake()
    {
        // Initializes base hp image and avoids duplicates of this object
        currentHpImage.sprite = thresholds[0].image;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initializes game over, hp, and UI fields
        IsGameOver = false;
        currentBaseHp = maxBaseHp;

        currentHpImage.enabled = false;
        gameOverText.enabled = false;
        levelRestartText.enabled = false;
        levelWinText.enabled = false;
    }

    void Update()
    {
        // Displays base hp image on level start
        if (LevelStarter.HasLevelStarted && currentHpImage.enabled == false)
        {
            currentHpImage.enabled = true;
        }

        // Toggles game over when base hp reaches 0
        if (currentBaseHp <= 0 || IsGameOver)
        {
            ToggleGameOver();
        }

        // Restarts level when game is over and enter key is pressed
        if (IsGameOver && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            RestartLevel();
        }
    }

    // Updates base hp UI whenever base hp changes
    private void UpdateUI()
    {
        HpThreshold match = null;
        float hpPercent = (float) currentBaseHp / maxBaseHp;

        foreach (HpThreshold threshold in thresholds)
        {
            if (hpPercent >= threshold.percent && (match == null || match.percent < threshold.percent))
            {
                match = threshold;
            }
        }

        currentHpImage.sprite = match.image;
    }

    // Updates base hp by amount parameter
    public void UpdateBaseHP(int amount)
    {
        if (IsGameOver || (amount > 0 && currentBaseHp >= maxBaseHp))
        {
            return;
        }

        currentBaseHp += amount;
        UpdateUI();
    }

    // Returns the current base hp
    public int GetBaseHp()
    {
        return currentBaseHp;
    }

    // Toggles game win when win conditions are met
    public void ToggleWin()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        levelWinText.enabled = true;
        levelRestartText.enabled = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        Time.timeScale = 0f;
    }

    // Toggles game over when lose conditions are met
    public void ToggleGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        gameOverText.enabled = true;
        levelRestartText.enabled = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        Time.timeScale = 0f;
    }

    // Restarts the level
    public void RestartLevel()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }
}
