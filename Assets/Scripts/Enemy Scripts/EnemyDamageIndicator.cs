using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyDamageIndicator : MonoBehaviour
{
    [SerializeField] private GameObject damageNumber;
    [SerializeField] private Canvas enemyCanvas;
    private TMP_Text dmgText; 
    private float fadeDelay = 2f;
    private float fadeDuration = 0.5f;
    private float floatSpeed = 2f;
    private Coroutine hideCoroutine;

    // Spawns the damage indicator visual, makes it float up, and then fades it out 
    public void ShowDamage(float dmg)
    {
        GameObject dmgObj = Instantiate(damageNumber, enemyCanvas.transform);
        dmgObj.transform.localPosition = new Vector3(0, 2, 0);
        dmgText = dmgObj.GetComponent<TextMeshPro>();
        dmgText.text = dmg.ToString();
        SetAlpha(1f);

        // Restart damage indicator fade timer
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(FadeAfterDelay());
    }

    // Hides the damage indicator after a few seconds
    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsed = 0f;
        Color dmgColor = dmgText.color;
        float startAlpha = dmgColor.a;

        while (elapsed < fadeDuration)
        {
            if (!ProjectileManager.IsFrozen)
            {
                elapsed += Time.deltaTime;
                float percentElapsed = elapsed / fadeDuration;
                float alpha = Mathf.Lerp(startAlpha, 0f, percentElapsed);
                SetAlpha(alpha);
            }
            
            yield return null;
        }
        SetAlpha(0f);
    }

    // Sets damage indicator visibility
    private void SetAlpha(float alpha)
    {
        Color dmgColor = dmgText.color;
        dmgColor.a = alpha;
        dmgText.color = dmgColor;
    } 
}
