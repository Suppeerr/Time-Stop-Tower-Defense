using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Set outline on a Clickable object
    public void SetOutlineVisibility(Clickable clickable, bool visible)
    {
        if (BaseHealthManager.IsGameOver)
        {
            return;
        }

        OutlineFlash outlineFlash = clickable.GetComponent<OutlineFlash>();

        if (outlineFlash != null && outlineFlash.IsFlashing)
        {
            outlineFlash.SetVisualOverride(visible);
        }

        if (visible)
        {
            clickable.Outline.OutlineColor = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);
        }
        else
        {
            clickable.Outline.OutlineColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
        }
    }

    public void SetOutlineActive(Clickable clickable, bool active)
    {
        if (BaseHealthManager.IsGameOver)
        {
            return;
        }

        clickable.Outline.enabled = active;
    }
}
