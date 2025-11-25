using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text enemiesDefeatedText;
    private int enemiesDefeatedCounter = 0;

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
