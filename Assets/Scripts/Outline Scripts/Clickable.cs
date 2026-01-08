using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Clickable : MonoBehaviour
{
    // Outline flash script
    private OutlineFlash outlineFlashScript;

    // Outline width and color
    [SerializeField] private float outlineWidth;
    private Color baseColor = Color.white;

    // Outline hovering
    private bool isHovered = false;
    
    // Interactable layers
    private static LayerMask clickableLayers;

    // Outline script
    public Outline Outline { get; private set; }

    // Clickable bool
    public bool ClickableEnabled { get; private set; }

    void Awake()
    {
        // Adds an outline to the object and gets an outline flash script
        Outline = gameObject.AddComponent<Outline>();
        outlineFlashScript = GetComponent<OutlineFlash>();
        SetOutlineWidth(outlineWidth);
    }

    void Start()
    {
        // Sets the outline mode and initializes fields
        Outline.OutlineMode = Outline.Mode.OutlineAll;
        OutlineManager.Instance.ToggleOutlineActive(this, true);
        OutlineManager.Instance.UpdateOutlineVisibility(this, false);

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
        Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        bool hoveringThisFrame = false;
        bool clickedThisFrame = false;

        if (gameObject.layer == LayerMask.NameToLayer("Normal Projectile"))
        {
            // Sphere cast for normal projectiles
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
            // Raycast for all other objects
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
            OutlineManager.Instance.UpdateOutlineVisibility(this, true);
        }
        else if (!hoveringThisFrame && isHovered)
        {
            isHovered = false;
            OutlineManager.Instance.UpdateOutlineVisibility(this, false);
        }

        // Stops flashing if object clicked
        if (clickedThisFrame && outlineFlashScript != null)
        {
            outlineFlashScript.StopFlashing(true);
        }
    }

    // Sets outline width to specified width
    public void SetOutlineWidth(float width)
    {
        if (!Outline.enabled)
        {
            return;
        }

        Outline.OutlineWidth = width; 
    }

    // Updates the outline's visiblity and interactability
    public void UpdateClickable(bool isVisible, bool isActive)
    {
        ClickableEnabled = isActive;
        OutlineManager.Instance.UpdateOutlineVisibility(this, isVisible);
        OutlineManager.Instance.ToggleOutlineActive(this, isActive);
    }
}
