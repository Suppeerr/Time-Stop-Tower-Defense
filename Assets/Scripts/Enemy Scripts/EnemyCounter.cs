using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    // Enemy counter instance
    public static EnemyCounter Instance;

    // UI fields
    [SerializeField] private TMP_Text enemiesDefeatedText;
    [SerializeField] private Image enemiesDefeatedImage;
    private bool textEnabled = false;

    // Enemies defeated needed to win the level
    private int enemiesNeeded;
    
    // Level won check
    private bool hasWon = false;

    // Enemy counter
    public int EnemiesDefeated { get; private set; } = 0;

    private void Awake()
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
    }

    void Start()
    {
        // Adjusts enemies needed count based on difficulty
        if (GameInstance.levelDifficulty == GameInstance.difficultyType.Easy)
        {
            enemiesNeeded = 50;
        }
        else if (GameInstance.levelDifficulty == GameInstance.difficultyType.Normal)
        {
            enemiesNeeded = 90;
        }
        else
        {
            enemiesNeeded = 160;
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
        if (EnemiesDefeated >= enemiesNeeded && !hasWon)
        {
            EnemiesDefeated = enemiesNeeded;
            UpdateUI();

            hasWon = true;
            BaseHealthManager.Instance.ToggleWin();
        }

        // Changes position of UI for certain cameras
        if (CameraSwitcher.Instance.ActiveCam == 2)
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(190f, 70f);
        }
        else
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(780f, 70f);
        }
    }

    // Updates visible enemies defeated counter
    private void UpdateUI()
    {
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = EnemiesDefeated.ToString() + "/" + enemiesNeeded;
        }
    }

    // Increments enemies defeated count
    public void IncrementCount()
    {
        EnemiesDefeated++;
        UpdateUI();
    }
}
