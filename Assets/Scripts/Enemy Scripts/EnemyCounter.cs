using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text enemiesDefeatedText;
    [SerializeField] private Image enemiesDefeatedImage;
    private int enemiesDefeatedCounter = 0;
    private bool textEnabled = false;
    private bool hasWon = false;

    void Start()
    {
        enemiesDefeatedText.enabled = false;
        enemiesDefeatedImage.enabled = false;
        UpdateUI();
    }

    void Update()
    {
        if (LevelStarter.HasLevelStarted && textEnabled == false)
        {
            textEnabled = true;
            enemiesDefeatedText.enabled = true;
            enemiesDefeatedImage.enabled = true;
        }

        if (enemiesDefeatedCounter == 90 && !hasWon)
        {
            hasWon = true;
            BaseHealthManager.Instance.ToggleWin();
        }

        if (CameraSwitch.ActiveCam == 2)
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(123, 56);
        }
        else
        {
            enemiesDefeatedText.rectTransform.anchoredPosition = new Vector2(758, 56);
        }
    }

    // Calls whenever an enemy is defeated
    private void UpdateUI()
    {
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = enemiesDefeatedCounter.ToString();
        }
    }

    public void IncrementCount()
    {
        enemiesDefeatedCounter++;
        UpdateUI();
    }
}
