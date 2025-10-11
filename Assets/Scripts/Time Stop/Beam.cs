using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class BeamZap : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer beam;
    public float maxDistance = 50f;
    public LayerMask hitLayers;
    public float zapDuration = 0.05f; // how long it stays visible
    private Camera mainCamera;
    public BallSpawner ballSpawner;

    void Start()
    {
        beam.enabled = false;
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
            beam.enabled = true;
            GameObject hitObject = hit.collider.gameObject;
            endPos = hit.point;
            beam.SetPosition(0, firePoint.position);
            beam.SetPosition(1, endPos);
            beam.enabled = true;
            Destroy(hitObject);
            if (ballSpawner != null)
            {
                ballSpawner.SpawnBall(hitObject.transform.position, hitObject.transform.rotation);
                while (ProjectileManager.IsFrozen)
                {
                    
                }
            }
            
        yield return new WaitForSeconds(zapDuration);
            // Optional: apply damage to target
        }

        // Draw beam
        yield return new WaitForSeconds(zapDuration);

        beam.enabled = false;
    }
}
