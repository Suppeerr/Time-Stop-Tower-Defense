using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    // Outline manager instance
    public static OutlineManager Instance;

    private void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }
    }

    // Updates outline visibility on a clickable object
    public void UpdateOutlineVisibility(Clickable clickable, bool visible)
    {
        if (BaseHealthManager.Instance.IsGameOver)
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

    // Toggles the outline of a clickable object on or off
    public void ToggleOutlineActive(Clickable clickable, bool active)
    {
        if (BaseHealthManager.Instance.IsGameOver)
        {
            return;
        }

        clickable.Outline.enabled = active;
    }
}
