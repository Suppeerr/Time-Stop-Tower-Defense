using UnityEngine;
using System.Collections;

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
    private bool isPlaced = false;
    
    private Camera mainCamera;

    // Renderers and ballSpawner script
    private Renderer[] rends;
    private Color[][] originalColors;
    private BallSpawner ballSpawner;

    // Tower placement animation
    private float placementTime = 0.4f;

    // Tower placement audio
    public AudioSource towerPlaceSFX;

    // Money manager script
    private MoneyManager moneyManagerScript;

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
        // Drags the tower to wherever the cursor is
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                Vector3 desiredPos = hit.point + offset + Vector3.up * 2f;
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

    // When left mouse button is clicked and held, begin dragging the tower
    void OnMouseDown()
    {
        if (!isPlaced)
        {
            BeginDrag();
        }
    }

    // When left mouse button is unheld, stop dragging the tower
    void OnMouseUp()
    {
        StopDrag();
    }

    // Starts dragging the tower
    public void BeginDrag(MoneyManager monManager = null)
    {
        moneyManagerScript = monManager; 
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

    // Stops dragging the tower
    private void StopDrag()
    {
        if (!canPlace)
        {
            // Cancel placement
            CancelDrag();
            return;
        }
        if (!isPlaced)
        {
            StartCoroutine(PlaceTower(gameObject.transform.position));
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
                    
                j++;
            }
            i++;
        }
    }

    // 
    private IEnumerator PlaceTower(Vector3 initialPos)
    {
        moneyManagerScript.DecreaseMoney(5);
        isPlaced = true;
        isDragging = false;
        ApplyColor(1f);
        towerPlaceSFX?.Play();
        float elapsedDelay = 0f;
        Vector3 startPos = initialPos + new Vector3(0, 3, 0);
        Vector3 endPos = initialPos;

        while (elapsedDelay < placementTime)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / placementTime;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        if (ballSpawner != null)
        {
            ballSpawner.enabled = true;
        }
    }
}
