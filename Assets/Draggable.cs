using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    
    // mat/rend vars
    private Renderer rend;
    private Material originalMat;
    public Material transparentMat; // assign in inspector

    void Awake()
    {
        mainCamera = Camera.main;
        rend = GetComponent<Renderer>();
        originalMat = rend.material; // store the original opaque material
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(
                    hit.point.x + offset.x,
                    transform.position.y,   // lock Y height
                    hit.point.z + offset.z
                );
            }

            // stop dragging if mouse released
            if (Input.GetMouseButtonUp(0))
            {
                StopDrag();
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

            // switch to transparent material
            if (transparentMat != null)
                rend.material = transparentMat;
        }
    }

    private void StopDrag()
    {
        isDragging = false;
        
        rend.material = originalMat;
    }

    // 🔹 Call this from BlockSource to force dragging immediately
    public void StartDragAtCursor()
    {
        BeginDrag();
    }
}
