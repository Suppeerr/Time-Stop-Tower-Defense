using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    // UI fields
    [SerializeField] private TMP_Text enemiesDefeatedText;
    [SerializeField] private Image enemiesDefeatedImage;
    private bool textEnabled = false;

    // Enemy counter
    private int enemiesDefeatedCounter = 0;

    // Enemies defeated needed to win the level
    private int enemiesNeeded;
    
    // Level won check
    private bool hasWon = false;

    void Start()
    {
        // Adjusts enemies needed count based on difficulty
        if (GameInstance.levelDifficulty == GameInstance.difficultyType.Easy)
        {
            enemiesNeeded = 60;
        }
        else if (GameInstance.levelDifficulty == GameInstance.difficultyType.Normal)
        {
            enemiesNeeded = 90;
        }
        else
        {
            enemiesNeeded = 120;
        }

        // Sets UI to inactive
        enemiesDefeatedText.enabled = false;
        enemiesDefeatedImage.enabled = false;
        UpdateUI();
    }

    void Update()
    {
        // Sets UI active on level start
        if (LevelStarter.HasLevelStarted && textEnabled == false)
        {
            textEnabled = true;
            enemiesDefeatedText.enabled = true;
            enemiesDefeatedImage.enabled = true;
        }

        // Toggles level win when goal reached
        if (enemiesDefeatedCounter >= enemiesNeeded && !hasWon)
        {
            enemiesDefeatedCounter = enemiesNeeded;
            UpdateUI();

            hasWon = true;
            BaseHealthManager.Instance.ToggleWin();
        }

        // Changes position of UI for certain cameras
        if (CameraSwitcher.Instance.ActiveCam == 2)
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(178, 70);
        }
        else
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(780, 70);
        }
    }

    // Updates visible enemies defeated counter
    private void UpdateUI()
    {
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = enemiesDefeatedCounter.ToString() + "/" + enemiesNeeded;
        }
    }

    // Increments enemies defeated count
    public void IncrementCount()
    {
        enemiesDefeatedCounter++;
        UpdateUI();
    }
}
