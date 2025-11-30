using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Clickable : MonoBehaviour
{
    private Outline outline;
    private OutlineFlash outlineFlashScript;
    [SerializeField] private float outlineWidth;

    private Color baseColor = Color.white;

    private bool isHovered = false;
    private static LayerMask clickableLayers;

    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outlineFlashScript = GetComponent<OutlineFlash>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;

        if (clickableLayers == 0)
        {
            clickableLayers = LayerMask.GetMask("Tower", "Normal Projectile", "Upgradable", "Ignore Time Stop");
        }
    }

    void Update()
    {
        // Raycast to detect hovering
        Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        bool hoveringThisFrame = false;

        if (outline.OutlineColor != baseColor && outlineFlashScript.enabled == false)
        {
            outline.OutlineColor = baseColor;
        }

        if (gameObject.layer == LayerMask.NameToLayer("Normal Projectile"))
        {
            float radius = 0.5f;
            if (Physics.SphereCast(ray, radius, out RaycastHit hit, Mathf.Infinity, clickableLayers))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    hoveringThisFrame = true;
                }
            }
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (outlineFlashScript != null)
                    {
                        outlineFlashScript.StopFlashing();
                        outlineFlashScript.enabled = false;
                    }

                    hoveringThisFrame = true;
                }
            }
        }

        // Update hover state
        if (hoveringThisFrame && !isHovered)
        {
            isHovered = true;
            OutlineManager.Instance.SetOutline(this);
        }
        else if (!hoveringThisFrame && isHovered)
        {
            isHovered = false;
            OutlineManager.Instance.RemoveOutline(this);
        }
    }

    // Called by OutlineManager
    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetOutlineWidth(float width)
    {
        outline.OutlineWidth = width; 
    }

    private void OnEnable()
    {
        isHovered = false;
    }

    private void OnDisable()
    {
        DisableOutline();
    }
}
