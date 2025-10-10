using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class BeamZap : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public float maxDistance = 50f;
    public LayerMask hitLayers;
    public float zapDuration = 0.05f; // how long it stays visible
    private Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && ProjectileManager.IsFrozen)
        {
            FireZap();
        }
    }

    public void FireZap()
    { 
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        Vector3 endPos = Vector3.zero;

        // Raycast in the direction the tower is facing
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        float radius = 1f;
        if (Physics.SphereCast(ray, radius, out hit) && hit.collider.gameObject.tag == "Tower Projectile")
        {
            Debug.Log("Fired Beam!");
            endPos = hit.point;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, endPos);
            lineRenderer.enabled = true;

        yield return new WaitForSeconds(zapDuration);
            // Optional: apply damage to target
        }

        // Draw beam
        yield return new WaitForSeconds(zapDuration);

        lineRenderer.enabled = false;
    }
}
