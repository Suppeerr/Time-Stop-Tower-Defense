using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Clickable : MonoBehaviour
{
    private Outline outline;
    private Camera mainCamera;
    [SerializeField] private float outlineWidth;

    private bool isHovered = false;

    private static LayerMask clickableLayers;

    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;

        if (clickableLayers == 0)
        {
            clickableLayers = LayerMask.GetMask("Tower", "Normal Projectile");
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Raycast to detect hovering
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool hoveringThisFrame = false;

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
}
