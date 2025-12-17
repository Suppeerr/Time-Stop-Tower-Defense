using UnityEngine;
using System.Collections;

public class OutlineFlash : MonoBehaviour
{
    private Outline outline;
    private Coroutine flashCoroutine;
    private Color baseOutlineColor = Color.white;
    private float flashDuration = 0.5f;
    public bool IsFlashing { get; private set; }
    private bool visualOverride = false;
    
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void StartFlashing()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        IsFlashing = true;
        StartCoroutine(FlashRoutine());
    }

    public void StopFlashing(bool clicked)
    {
        IsFlashing = false;
        flashCoroutine = null;
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

        yield return null;

        float halfDuration = flashDuration / 2f;
        while (IsFlashing)
        {
            // Fade in
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                if (visualOverride)
                {
                    break;
                }

                float alpha = Mathf.Lerp(0f, baseOutlineColor.a, t / halfDuration);
                outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);
                yield return null;
            }

            // Fade out
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                if (visualOverride)
                {
                    break;
                }

                float alpha = Mathf.Lerp(baseOutlineColor.a, 0f, t / halfDuration);
                outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);
                yield return null;
            }

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
