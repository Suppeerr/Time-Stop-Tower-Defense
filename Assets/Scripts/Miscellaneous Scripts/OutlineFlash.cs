using UnityEngine;
using System.Collections;

public class OutlineFlash : MonoBehaviour
{
    private Outline outline;
    private Coroutine flashCoroutine;
    private Color baseOutlineColor = Color.white;
    private float flashDuration = 0.5f;

    void OnEnable()
    {
        outline = GetComponent<Outline>();

        StartFlashing();
    }

    public void StartFlashing()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        outline.enabled = true;
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlashing()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        float halfDuration = flashDuration / 2f;
        while (true)
        {
            // Fade in
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0f, baseOutlineColor.a, t / halfDuration);
                outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);
                yield return null;
            }

            // Fade out
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(baseOutlineColor.a, 0f, t / halfDuration);
                outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);
                yield return null;
            }
        }
    }
}
