using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyDamageIndicator : MonoBehaviour
{
    // Enemy position
    private Transform enemy;

    // Damage indicator fields
    private float fadeDelay = 1f;
    private float fadeDuration = 0.5f;
    private float floatDistance = 3f;
    private float floatSpeed = 2f;

    // Spawns the damage indicator visual
    public void ShowDamage(float dmg, Transform e)
    {
        enemy = e;

        TMP_Text dmgText = GetComponentInChildren<TMP_Text>();

        if (dmg >= 1000)
        {
            dmgText.fontSize = 7f;
        }

        dmgText.text = dmg.ToString();
        SetAlpha(1f, dmgText);

        StartCoroutine(FadeAndFloat(dmgText));
    }

    // Floats and fades the indicator gradually
    private IEnumerator FadeAndFloat(TMP_Text dmgText)
    {
        // Floats indicator up
        float elapsedDelay = 0f;

        float xOffset = Random.Range(-1f, 1f);
        float heightOffset = Random.Range(-1f, 1f);

        Vector3 startPos = enemy.position;
        startPos += Vector3.up * 2.5f + new Vector3(xOffset, 0f, 0f);
        Vector3 endPos = startPos + new Vector3(0f, floatDistance + heightOffset, 0f);

        while (elapsedDelay < fadeDelay)
        {
            if (!ProjectileManager.IsFrozen)
            {
                elapsedDelay += Time.deltaTime;

                float t = (elapsedDelay / fadeDelay) * floatSpeed;

                transform.position = Vector3.Lerp(startPos, endPos, t);
            }
        
            yield return null;
        }

        // Fades damage indicator
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
                SetAlpha(alpha, dmgText);
            }

            yield return null;
        }

        SetAlpha(0f, dmgText);
        Destroy(this.gameObject);
    }

    // Sets damage indicator transparency
    private void SetAlpha(float alpha, TMP_Text dmgText)
    {
        Color dmgColor = dmgText.color;
        dmgColor.a = alpha;
        dmgText.color = dmgColor;
    } 
}
