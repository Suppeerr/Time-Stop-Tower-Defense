using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance { get; private set; }

    private Clickable currentOutlined;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Set outline on a Clickable object, disabling previous one if necessary
    public void SetOutline(Clickable clickable)
    {
        if (currentOutlined == clickable) return;

        if (currentOutlined != null)
        {
            currentOutlined.DisableOutline();
        }

        currentOutlined = clickable;

        if (currentOutlined != null)
        {
            currentOutlined.EnableOutline();
        }
    }

    // Remove outline from object if it's currently outlined
    public void RemoveOutline(Clickable clickable)
    {
        if (currentOutlined == clickable)
        {
            currentOutlined.DisableOutline();
            currentOutlined = null;
        }
    }

    // Force remove any outline
    public void RemoveOutline()
    {
        if (currentOutlined != null)
        {
            currentOutlined.DisableOutline();
            currentOutlined = null;
        }
    }
}
