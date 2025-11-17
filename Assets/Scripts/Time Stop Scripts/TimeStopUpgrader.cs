using UnityEngine;
using TMPro;
using System.Collections;

public class TimeStopUpgrader : MonoBehaviour
{
    public GameObject storedTimeManager;
    private StoredTimeManager storedTimeManagerScript;
    [SerializeField] private TMP_Text upgradeOneIndicator;
    [SerializeField] private Color baseGlowColor;
    private Color baseColor = Color.yellow;
    [SerializeField] private Color invalidGlowColor;
    private Color invalidColor = Color.red;

    void Start()
    {
        storedTimeManagerScript = storedTimeManager.GetComponent<StoredTimeManager>();
        upgradeOneIndicator.text = "Upgrade 1: +1s timestop duration; Cost: 10 seconds";
        upgradeOneIndicator.color = baseColor;
        Material mat = upgradeOneIndicator.fontMaterial;
        mat.SetColor("_GlowColor", baseGlowColor);
    }

    private void OnMouseDown()
    {
        if (ProjectileManager.IsFrozen || BaseHealthManager.IsGameOver)
        {
            return;
        }

        // Check if stored time manager has enough seconds
        int seconds = storedTimeManagerScript.GetSeconds();
        if (seconds >= 10)
        {
            upgradeOneIndicator.text = "Upgrade brought!";
        }
        else
        {
            StartCoroutine(FlashRed(0.4f));
        }
    }
    
    private IEnumerator FlashRed(float duration)
    {
        // Get the font material instance
        Material mat = upgradeOneIndicator.fontMaterial;

        // Set color and glow to red 
        upgradeOneIndicator.color = invalidColor;
        mat.SetColor("_GlowColor", invalidGlowColor);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        upgradeOneIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
    }
}   
