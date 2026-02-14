using UnityEngine;
using System.Collections;

public class OutlineFlash : MonoBehaviour
{
    // Outline script
    private Outline outline;

    // Base outline color
    private Color baseOutlineColor = Color.white;

    // Visual override bool
    private bool visualOverride = false;

    // Outline flashing fields
    private float flashDuration = 0.6f;
    public bool IsFlashing { get; private set; }
    
    void Start()
    {
        // Gets the object's outline script
        outline = GetComponent<Outline>();
    }

    // Starts flashing the outline
    public void StartFlashing()
    {
        StopAllCoroutines();

        IsFlashing = true;
        StartCoroutine(FlashRoutine());
    }

    // Stops flashing the outline
    public void StopFlashing(bool clicked)
    {
        IsFlashing = false;
        visualOverride = false;
        StopAllCoroutines();

        outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, 0f);

        if (!clicked)
        {
            visualOverride = true;
        }
    }

    // Fades the outline in and out when active
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

    // Fades the outline's alpha from one value to another
    private IEnumerator FadeOutline(float from, float to)
    {
        float halfDuration = flashDuration / 2f;

        for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            while (SettingsMenuOpener.Instance.MenuOpened)
            {
                yield return null;
            }

            if (visualOverride || !IsFlashing)
            {
                break; 
            }

            float alpha = Mathf.Lerp(from, to, t / halfDuration);
            outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, alpha);

            yield return null;
        }
    } 

    // Overrides the outline flash if the object is hovered over
    public void SetVisualOverride(bool isOverriding)
    {
        visualOverride = isOverriding;

        if (isOverriding)
        {
            outline.OutlineColor = new Color(baseOutlineColor.r, baseOutlineColor.g, baseOutlineColor.b, 1f);
        }
    }
}
