using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    // Health bar visuals
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Gradient healthGradient;

    // Max enemy health
    private float maxHP;

    // Health bar fade fields
    private float fadeDelay = 3f;
    private float fadeDuration = 0.5f;

    // Coroutine field
    private Coroutine hideCoroutine;

    // Gets enemy health and becomes invisible
    public void Init(float maxHP)
    {
        this.maxHP = maxHP;
        SetAlpha(0f);
    }

    // Updates enemy health bar UI when damage is taken
    public void UpdateHealth(float currentHP)
    {
        float percentHP = currentHP / maxHP;
        fillImage.fillAmount = percentHP;
        fillImage.color = healthGradient.Evaluate(percentHP);

        SetAlpha(1f);

        // Restart healthbar fade timer
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(FadeAfterDelay());
    }

    // Fades the healthbar away after a few seconds
    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float startAlpha = backgroundImage.color.a;
        float elapsedFade = 0f;

        while (elapsedFade < fadeDuration)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            elapsedFade += Time.deltaTime;
            float percentElapsed = elapsedFade / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, 0f, percentElapsed);

            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    // Sets healthbar visibility
    private void SetAlpha(float alpha)
    {
        Color bgColor = backgroundImage.color;
        bgColor.a = alpha;
        backgroundImage.color = bgColor;

        Color fillColor = fillImage.color;
        fillColor.a = alpha;
        fillImage.color = fillColor;
    } 
}
