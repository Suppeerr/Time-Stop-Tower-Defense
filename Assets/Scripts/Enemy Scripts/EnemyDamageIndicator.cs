using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyDamageIndicator : MonoBehaviour
{
    [SerializeField] private GameObject damageIndicator;
    [SerializeField] private GameObject enemy;
    private float fadeDelay = 1f;
    private float fadeDuration = 0.5f;
    private float floatDistance = 3f;
    private float floatSpeed = 2f;

    // Spawns the damage indicator visual, makes it float up, and then fades it out 
    public void ShowDamage(float dmg)
    {
        GameObject dmgObj = Instantiate(damageIndicator, enemy.transform);
        dmgObj.transform.localScale = Vector3.one;
        TMP_Text dmgText = dmgObj.GetComponentInChildren<TMP_Text>();
        dmgText.text = dmg.ToString();
        SetAlpha(1f, dmgText);

        StartCoroutine(FadeAndFloat(dmgObj, dmgText));
    }

    // Hides the damage indicator after a few seconds
    private IEnumerator FadeAndFloat(GameObject dmgObj, TMP_Text dmgText)
    {
        float elapsedDelay = 0f;
        float xOffset = Random.Range(-2f, 2f);
        float heightOffset = Random.Range(-2f, 2f);
        Vector3 startPos = new Vector3(xOffset, 7, 0);;
        Vector3 endPos = startPos + new Vector3(0, floatDistance + heightOffset, 0);

        while (elapsedDelay < fadeDelay)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            // CheckEnemyDeath(dmgObj);
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / fadeDelay;
            dmgObj.transform.localPosition = Vector3.Lerp(startPos, endPos, t * floatSpeed);

            yield return null;
        }

        float elapsed = 0f;
        Color dmgColor = dmgText.color;
        float startAlpha = dmgColor.a;

        while (elapsed < fadeDuration)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            // CheckEnemyDeath(dmgObj);
            elapsed += Time.deltaTime;
            float percentElapsed = elapsed / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, 0f, percentElapsed);
            SetAlpha(alpha, dmgText);
            yield return null;
        }
        SetAlpha(0f, dmgText);
        Destroy(dmgObj);
    }

    // Checks if enemy has died
    private void CheckEnemyDeath(GameObject dmgObj)
    {
        if (enemy == null)
        {
            dmgObj.transform.parent = null;
        }
    }

    // Sets damage indicator visibility
    private void SetAlpha(float alpha, TMP_Text dmgText)
    {
        Color dmgColor = dmgText.color;
        dmgColor.a = alpha;
        dmgText.color = dmgColor;
    } 
}
