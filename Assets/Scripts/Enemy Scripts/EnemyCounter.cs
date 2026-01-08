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
    
    // Level won check
    private bool hasWon = false;

    void Start()
    {
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
        if (enemiesDefeatedCounter == 90 && !hasWon)
        {
            hasWon = true;
            BaseHealthManager.Instance.ToggleWin();
        }

        // Changes position of UI for certain cameras
        if (CameraSwitcher.Instance.ActiveCam == 2)
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(123, 56);
        }
        else
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(758, 56);
        }
    }

    // Updates visible enemies defeated counter
    private void UpdateUI()
    {
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = enemiesDefeatedCounter.ToString();
        }
    }

    // Increments enemies defeated count
    public void IncrementCount()
    {
        enemiesDefeatedCounter++;
        UpdateUI();
    }
}
