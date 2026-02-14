using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    // Health bar visuals
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Gradient healthGradient;

    // Enemy health fields
    private int maxHP;
    private float percentHP;
    private float oldPercentHP;

    // Health bar transition field
    private float transitionDuration = 0.2f;

    // Health bar fade fields
    private float fadeDelay = 3f;
    private float fadeDuration = 0.5f;

    // Coroutine field
    private Coroutine hideCoroutine;

    // Gets enemy health and becomes invisible
    public void Init(int maxHP)
    {
        this.maxHP = maxHP;
        SetAlpha(0f);
    }

    // Updates enemy health bar UI when damage is taken
    public void UpdateHealth(int current, int old)
    {
        oldPercentHP = (float) old / maxHP;
        percentHP = (float) current / maxHP;

        StartCoroutine(TransitionHealthBar());

        SetAlpha(1f);

        // Restart health bar fade timer
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(FadeAfterDelay());
    }

    // Smoothly transitions the healthbar color 
    private IEnumerator TransitionHealthBar()
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float percentElapsed = elapsed / transitionDuration;

            fillImage.fillAmount = Mathf.Lerp(oldPercentHP, percentHP, percentElapsed);
            fillImage.color = healthGradient.Evaluate(Mathf.Lerp(oldPercentHP, percentHP, percentElapsed));

            yield return null;
        }

        fillImage.fillAmount = percentHP;
        fillImage.color = healthGradient.Evaluate(percentHP);
    }

    // Fades the health bar away after a few seconds
    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float startAlpha = backgroundImage.color.a;
        float elapsedFade = 0f;

        while (elapsedFade < fadeDuration)
        {
            elapsedFade += Time.deltaTime;
            float percentElapsed = elapsedFade / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, 0f, percentElapsed);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(0f);
    }

    // Sets health bar visibility
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
