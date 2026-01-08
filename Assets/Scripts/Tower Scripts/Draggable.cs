using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Draggable : MonoBehaviour
{
    // Ground layer and invalid placement tags 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private string[] invalidTags = {"Tower", "GameController", "Enemy"};

    // Drag check boolean
    private bool isDragging = false;

    // Placement variables
    private float placementRadius = 2f;
    private bool isPlaced = false;
    private Vector3 offset;
    
    // Renderers and ball spawner script
    private Renderer[] rends;
    private Color[][] originalColors;
    [SerializeField] private GameObject placedTowerPrefab;

    void Awake()
    {
        // Grabs tower renderers
        rends = GetComponentsInChildren<Renderer>(true);

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
        DragToCursor();
    }

    // Drags the tower to wherever the cursor is
    private void DragToCursor()
    {
        if (isDragging)
        {
            bool isColliding = false;
            bool insidePlacementZone = false;
            Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                Vector3 desiredPos = hit.point + offset;

                // Ensures correct Y position by sampling the ground height
                if (Physics.Raycast(desiredPos + Vector3.up * 10f, Vector3.down, out RaycastHit groundHit, 20f, groundMask))
                {
                    // Snap to the real ground height
                    desiredPos.y = groundHit.point.y + 0.67f;
                }
                transform.position = desiredPos;

                // Check for nearby objects
                Collider[] nearby = Physics.OverlapSphere(desiredPos, placementRadius);

                // Update color based on placement validity
                foreach (Collider col in nearby)
                {
                    if (col.CompareTag("Placement Zone"))
                    {
                        insidePlacementZone = true;
                    }

                    foreach (string invTag in invalidTags)
                    {
                        if (col.CompareTag(invTag) && col.gameObject != gameObject)
                        {
                            isColliding = true;
                            break;
                        }
                    }
                    
                    if (isColliding)
                    {
                        break;
                    }
                }

                if (!isColliding && insidePlacementZone)
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
                if (!isColliding && insidePlacementZone)
                {
                    StopDrag(true);
                }
                else
                {
                    StopDrag(false);
                }
            }

            // Cancel drag if time stopped or x pressed
            if (TimeStop.Instance.IsFrozen || Keyboard.current.xKey.wasPressedThisFrame)
            {
                StopDrag(false);
            }
        }
    }

    // When left mouse button is clicked and held, begin dragging the tower
    void OnMouseDown()
    {
        if (!isPlaced)
        {
            BeginDrag();
        }
    }

    // Starts dragging the tower
    public void BeginDrag()
    {
        Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        isDragging = true;
        PlacementZoneManager.Instance.ShowZone();

        if (Physics.Raycast(ray, out hit))
        {
            offset = transform.position - hit.point;
        }
    }

    // Stops dragging the tower
    private void StopDrag(bool canPlace)
    {
        isDragging = false;
        PlacementZoneManager.Instance.HideZone();

        // Cancels dragging
        if (!canPlace)
        {
            Destroy(gameObject);
            return;
        }

        if (!isPlaced)
        {
            GameObject placedTower = Instantiate(placedTowerPrefab);
            PlacedTower placedTowerScript = placedTower.GetComponent<PlacedTower>();

            if (placedTowerScript != null)
            {
                placedTowerScript.StartCoroutine(placedTowerScript.PlaceTower(gameObject.transform.position));
            }

            Destroy(gameObject);
        }
    }

    // Changes the color and transparency of the draggable tower 
    private void ApplyColor(float alpha = 1f, Color? tint = null, bool useOriginal = true)
    {
        for (int i = 0; i < rends.Length; i++)
        {
            int originalMaterialCount = originalColors[i].Length;

            for (int j = 0; j < rends[i].materials.Length; j++)
            {
                Material mat = rends[i].material;
                Color baseColor = useOriginal 
                ? originalColors[i][Mathf.Min(j, originalMaterialCount - 1)] 
                : (tint ?? originalColors[i][Mathf.Min(j, originalMaterialCount - 1)]);

                Color finalColor = baseColor;
                finalColor.a = alpha;
                mat.color = finalColor;
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                if (alpha < 1f)
                {
                    mat.SetFloat("_Surface", 1f); // Transparent
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else
                {
                    mat.SetFloat("_Surface", 0f); // Opaque
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.renderQueue = -1;
                }  
            }
        }
    }
}
