using UnityEngine;

public class Draggable : MonoBehaviour
{
    // Placement layer and checks
    public LayerMask groundMask;
    public string[] invalidTags = {"Tower", "GameController", "Enemy"};
    
    // Drag check
    private bool isDragging = false;
    private Vector3 offset;

    // Placement variables
    private float placementRadius = 2f;
    private bool canPlace = true;
    public bool isPlaced = false;
    
    private Camera mainCamera;

    // Renderers and ballSpawner script
    private Renderer[] rends;
    private Color[][] originalColors;
    private BallSpawner ballSpawner;

    void Awake()
    {
        mainCamera = Camera.main;
        rends = GetComponentsInChildren<Renderer>(true);
        ballSpawner = GetComponent<BallSpawner>();

        // Store original tower colors
        originalColors = new Color[rends.Length][];
        for (int i = 0; i < rends.Length; i++)
        {
            Material[] mats = rends[i].materials;
            originalColors[i] = new Color[mats.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                originalColors[i][j] = mats[j].color;
            }
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                Vector3 desiredPos = hit.point + offset + Vector3.up * 0.85f;
                transform.position = desiredPos;

                // Check for nearby objects
                Collider[] nearby = Physics.OverlapSphere(desiredPos, placementRadius);
                canPlace = true;

                // Update color based on placement validity
                foreach (Collider col in nearby)
                {
                    foreach (string invTag in invalidTags)
                    {
                        if (col.CompareTag(invTag) && col.gameObject != gameObject)
                        {
                            canPlace = false;
                            break;
                        }
                    }
                    if (!canPlace)
                    {
                        break;
                    }
                }

                if (canPlace)
                {
                    ApplyColor(0.6f);
                }
                else
                {
                    ApplyColor(0.6f, Color.red, false);
                }
            }

            // Stop dragging if mouse released
            if (Input.GetMouseButtonUp(0))
            {
                if (canPlace)
                {
                    StopDrag();
                }
                else
                {
                    CancelDrag();
                }
            }

            // Cancel drag if time stopped or x pressed
            if (ProjectileManager.IsFrozen || Input.GetKeyDown(KeyCode.X))
            {
                CancelDrag();
            }
        }
    }

    void OnMouseDown()
    {
        if (!isPlaced)
        {
            BeginDrag();
        }
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
        isPlaced = true;
        isDragging = false;
        ApplyColor(1f);

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

    private void ApplyColor(float alpha = 1f, Color? tint = null, bool useOriginal = true)
    {
        int i = 0;
        foreach (Renderer rend in rends)
        {
            int j = 0;
            foreach (Material mat in rend.materials)
            {
                Color baseColor = useOriginal ? originalColors[i][j] : (tint ?? originalColors[i][j]);
                Color finalColor = baseColor;
                finalColor.a = alpha;
                mat.color = finalColor;

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
                    mat.SetFloat("_Surface", 0f); // Opaque
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = -1;
                }
                    
                j++;
            }
            i++;
        }
    }

    // 🔹 Call this from BlockSource to force dragging immediately
    public void StartDragAtCursor()
    {
        BeginDrag();
    }
}
