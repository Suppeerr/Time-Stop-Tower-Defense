using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Clickable : MonoBehaviour
{
    public Outline Outline { get; private set; }
    private OutlineFlash outlineFlashScript;
    [SerializeField] private float outlineWidth;

    private Color baseColor = Color.white;

    private bool isHovered = false;
    public bool ClickableEnabled { get; private set; }
    private static LayerMask clickableLayers;

    void Awake()
    {
        Outline = gameObject.AddComponent<Outline>();
        outlineFlashScript = GetComponent<OutlineFlash>();
        SetOutlineWidth(outlineWidth);
    }

    void Start()
    {
        Outline.OutlineMode = Outline.Mode.OutlineAll;
        OutlineManager.Instance.SetOutlineActive(this, true);
        OutlineManager.Instance.SetOutlineVisibility(this, false);

        ClickableEnabled = true;

        if (clickableLayers == 0)
        {
            clickableLayers = LayerMask.GetMask("Tower", "Normal Projectile", "Upgradable", "Ignore Time Stop");
        } 
    }
        
    void Update()
    {
        if (!ClickableEnabled)
        {
            return;
        }

        // Raycast to detect hovering
        Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        bool hoveringThisFrame = false;
        bool clickedThisFrame = false;

        if (gameObject.layer == LayerMask.NameToLayer("Normal Projectile"))
        {
            float radius = 0.5f;
            if (Physics.SphereCast(ray, radius, out RaycastHit hit, Mathf.Infinity, clickableLayers))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        clickedThisFrame = true;
                    }

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
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        clickedThisFrame = true;
                    }

                    hoveringThisFrame = true;
                }
            }
        }

        // Update hover state
        if (hoveringThisFrame && !isHovered)
        {
            isHovered = true;
            OutlineManager.Instance.SetOutlineVisibility(this, true);
        }
        else if (!hoveringThisFrame && isHovered)
        {
            isHovered = false;
            OutlineManager.Instance.SetOutlineVisibility(this, false);
        }

        if (clickedThisFrame && outlineFlashScript != null)
        {
            outlineFlashScript.StopFlashing(true);
        }
    }

    public void SetOutlineWidth(float width)
    {
        if (!Outline.enabled)
        {
            return;
        }

        Outline.OutlineWidth = width; 
    }

    public void UpdateClickable(bool isVisible, bool isActive)
    {
        ClickableEnabled = isActive;
        OutlineManager.Instance.SetOutlineVisibility(this, isVisible);
        OutlineManager.Instance.SetOutlineActive(this, isActive);
    }
}
