using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class BeamZap : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer beam;
    public float maxDistance = 50f;
    public LayerMask hitLayers;
    private float zapDuration = 0.1f; // how long it stays visible
    private Camera mainCamera;
    public BallSpawner ballSpawner;

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

        if (Physics.SphereCast(ray, radius, out hit) && hit.collider.gameObject.CompareTag("Tower Projectile"))
        {
            GameObject hitProj = hit.collider.gameObject;
            NormalProjectile normalProj = hitProj.GetComponent<NormalProjectile>();
            HomingProjectile homingProj = hitProj.GetComponent<HomingProjectile>();

            if (homingProj != null)
            {
                homingProj.EnableEffects();
            }

            beam.enabled = true;
            endPos = hit.point;
            beam.SetPosition(0, firePoint.position);
            beam.SetPosition(1, endPos);

            if (normalProj != null)
            {
                normalProj.MarkDestroyedByParry();

                switch (normalProj.type)
                {
                    case ProjectileType.PrimaryNormal:
                        normalProj.MarkDestroyedByParry();
                        ballSpawner?.SpawnHoming(ProjectileType.PrimaryHoming, hitProj.transform.position, hitProj.transform.rotation);
                        break;
                    case ProjectileType.SecondaryNormal:
                        ballSpawner?.SpawnHoming(ProjectileType.SecondaryHoming, hitProj.transform.position, hitProj.transform.rotation);
                        break;
                }
            }

            Destroy(hitProj); 
        }


        // Disable beam
        yield return new WaitForSeconds(zapDuration);
        beam.enabled = false;
    }
}
