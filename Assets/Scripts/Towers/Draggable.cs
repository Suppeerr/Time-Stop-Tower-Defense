using UnityEngine;

public class Draggable : MonoBehaviour
{
    public LayerMask groundMask;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    public float placementRadius = 1f; 
    private bool canPlace = true;


    // mat/rend vars
    private Renderer[] rends;
    private BallSpawner ballSpawner;

    void Awake()
    {
        mainCamera = Camera.main;
        rends = GetComponentsInChildren<Renderer>(true);
        ballSpawner = GetComponent<BallSpawner>();
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                Vector3 desiredPos = hit.point + offset + Vector3.up * 0.7f;
                transform.position = desiredPos;

                // Check for nearby objects
                // Collider[] nearby = Physics.OverlapSphere(desiredPos, placementRadius);
                // canPlace = true;

                // foreach (Collider col in nearby)
                // {
                //     if (col.gameObject != this.gameObject) // ignore self
                //     {
                //         canPlace = false;
                //         break;
                //     }
                // }

                // Change color based on whether placement is valid
                // SetPlacementColor(canPlace);
            }

            // stop dragging if mouse released
            if (Input.GetMouseButtonUp(0))
            {
                StopDrag();
            }

            // cancel drag if time stopped or x pressed
            if (ProjectileManager.IsFrozen || Input.GetKeyDown(KeyCode.X))
            {
                CancelDrag();
            }
        }
    }

    void OnMouseDown()
    {
        BeginDrag();
    }

    void OnMouseUp()
    {
        StopDrag();
    }

    private void BeginDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            offset = transform.position - hit.point;
            isDragging = true;

            if (ballSpawner != null)
            {
                ballSpawner.enabled = false;
            }

            // make transparent
            SetTransparency(.6f);
        }
    }

    private void StopDrag()
    {
        if (!canPlace)
        {
            // Cancel placement
            CancelDrag();
            return;
        }
        isDragging = false;
        SetTransparency(1f);

        if (ballSpawner != null)
        {
            ballSpawner.enabled = true;
        }
    }

    private void CancelDrag()
    {
        isDragging = false;
        Destroy(gameObject);
    }

    private void SetTransparency(float alpha)
    {
        foreach (Renderer rend in rends)
        {
            foreach (Material mat in rend.materials)
            {
                if (!mat.HasProperty("_BaseColor"))
                    continue;

                // Get and update existing base color
                Color baseColor = mat.GetColor("_BaseColor");
                baseColor.a = alpha;
                mat.SetColor("_BaseColor", baseColor);

                // Force the material into transparent mode
                if (alpha < 1f)
                {
                    mat.SetFloat("_Surface", 1f); // Transparent
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else
                {
                    // Revert to opaque mode
                    mat.SetFloat("_Surface", 0f); // Opaque
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.renderQueue = -1;
                }
            }
        }
    }

    // 🔹 Call this from BlockSource to force dragging immediately
    public void StartDragAtCursor()
    {
        BeginDrag();
    }
}
