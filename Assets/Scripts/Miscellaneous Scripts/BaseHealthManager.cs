using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class BaseHealthManager : MonoBehaviour
{
    public static BaseHealthManager Instance;
    [SerializeField] private TMP_Text baseHpText;
    [SerializeField] private TMP_Text levelRestartText;
    public static bool IsGameOver { get; private set; }
    private int startingBaseHp;
    private int currentBaseHp;
    private bool textEnabled = false;

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

        IsGameOver = false;
        startingBaseHp = 500;
        currentBaseHp = startingBaseHp;
        UpdateUI();
        baseHpText.enabled = false;
        levelRestartText.enabled = false;
    }

    void Update()
    {
        if (LevelStarter.HasLevelStarted && textEnabled == false)
        {
            textEnabled = true;
            baseHpText.enabled = true;
        }
        if (currentBaseHp <= 0)
        {
            ToggleGameOver();
        }
        if (IsGameOver && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
            Time.timeScale = 1f;
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
        levelRestartText.enabled = true;
        ProjectileManager.Instance.DestroyAllProjectiles();

        // Freezes time
        Time.timeScale = 0f;
    }
}
