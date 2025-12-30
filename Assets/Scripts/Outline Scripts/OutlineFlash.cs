using UnityEngine;
using System.Collections;

public class OutlineFlash : MonoBehaviour
{
    private Outline outline;
    private Color baseOutlineColor = Color.white;
    private float flashDuration = 0.6f;
    public bool IsFlashing { get; private set; }
    private bool visualOverride = false;
    
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void StartFlashing()
    {
        StopAllCoroutines();

        IsFlashing = true;
        StartCoroutine(FlashRoutine());
    }

    public void StopFlashing(bool clicked)
    {
        IsFlashing = false;
        StopAllCoroutines();

        if (!clicked)
        {
            visualOverride = true;
            outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, 0f);
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (outline == null)
        {
            yield break;
        }
        
        while (IsFlashing)
        {
            // Fade in
            yield return StartCoroutine(FadeOutline(0f, baseOutlineColor.a));

            // Fade out
            yield return StartCoroutine(FadeOutline(baseOutlineColor.a, 0f));
        }
    }

    private IEnumerator FadeOutline(float from, float to)
    {
        float halfDuration = flashDuration / 2f;

        for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            if (visualOverride || !IsFlashing)
            {
                break;
            }

            float alpha = Mathf.Lerp(from, to, t / halfDuration);
            outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);

            yield return null;
        }
    } 

    public void SetVisualOverride(bool isOverriding)
    {
        visualOverride = isOverriding;

        if (isOverriding)
        {
            outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, 1f);
        }
    }
}
