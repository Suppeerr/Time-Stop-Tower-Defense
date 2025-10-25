using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Gradient healthGradient;
    private float maxHP;
    private float fadeDelay = 3f;
    private float fadeDuration = 0.5f;
    private Coroutine hideCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(float maxHP)
    {
        this.maxHP = maxHP;
        SetAlpha(0f);
    }

    // Updates enemy healthbar UI when damage is taken
    public void UpdateHealth(float currentHP)
    {
        float percentHP = currentHP / maxHP;
        fillImage.fillAmount = percentHP;
        fillImage.color = healthGradient.Evaluate(percentHP);

        // Healthbar shows up when enemy takes damage
        SetAlpha(1f);

        // Restart healthbar fade timer
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(FadeAfterDelay());
    }

    // Hides the healthbar after a few seconds
    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsed = 0f;
        Color bgColor = backgroundImage.color;
        Color fillColor = fillImage.color;

        float startAlpha = bgColor.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float percentElapsed = elapsed / fadeDuration;
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
