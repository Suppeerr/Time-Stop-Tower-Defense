using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = true;
    private Vector3 offset;
    
    // mat/rend vars
    private Renderer rend;
    private Material originalMat;
    public Material transparentMat; // assign in inspector

    void Start()
    {
        mainCamera = Camera.main;
        rend = GetComponent<Renderer>();
        originalMat = rend.material; // store the original opaque material
    }

    void OnMouseDown()
    {
        // Cast a ray from the camera through the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // If the ray hits the ground
        if (Physics.Raycast(ray, out hit))
        {
            // Calculate offset between object position and hit point
            offset = transform.position - hit.point;
            isDragging = true;

            // switch to transparent material
            if (transparentMat != null)
                rend.material = transparentMat;
        }
    }

    void OnMouseDrag()
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
        }
    }


    void OnMouseUp()
    {
        isDragging = false;

        // switch back to original opaque material
        rend.material = originalMat;
    }
}
