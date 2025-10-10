using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = true;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    /*
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
        }
    }


        void OnMouseDrag()
        {
            if (isDragging)
            {
                // Ray from camera to mouse
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Raycast into the scene
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if we hit the ground
                    if (hit.collider.CompareTag("Ground"))
                    {
                        // Move object to hit point (XZ plane at ground)
                        transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    }
                }
            }
        }
    */
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
    }
}
