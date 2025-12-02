using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text enemiesDefeatedText;
    private int enemiesDefeatedCounter = 0;
    private bool textEnabled = false;

    void Start()
    {
        enemiesDefeatedText.enabled = false;
    }

    void Update()
    {
        if (LevelStarter.HasLevelStarted && textEnabled == false)
        {
            enabled = true;
            enemiesDefeatedText.enabled = true;
        }
    }

    // Calls whenever an enemy is defeated
    private void UpdateUI()
    {
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = enemiesDefeatedCounter + " Enemies Defeated!";
        }
    }

    public void IncrementCount()
    {
        enemiesDefeatedCounter++;
        UpdateUI();
    }
}
