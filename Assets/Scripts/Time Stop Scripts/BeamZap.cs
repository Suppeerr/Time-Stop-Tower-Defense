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
    public BallSpawner ballSpawner;
    public AudioSource zapSFX;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if ((Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && ProjectileManager.IsFrozen)
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
        Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        float radius = 0.8f;
        int projectileLayers = LayerMask.GetMask("Normal Projectile", "Homing Projectile");

        if (Physics.SphereCast(ray, radius, out hit, Mathf.Infinity, projectileLayers))
        {
            GameObject hitProj = hit.collider.gameObject;
            NormalProjectile normalProjScript = hitProj.GetComponent<NormalProjectile>();
            HomingProjectile homingProjScript = hitProj.GetComponent<HomingProjectile>();

            if (homingProjScript != null)
            {
                homingProjScript.IncrementChargeLevel();
                homingProjScript.ChangeLayer();
                homingProjScript.EnableUpgradedEffects();

                if (homingProjScript.type == ProjectileType.PrimaryHoming)
                {
                    homingProjScript.AddDamage(400);
                }
            }

            beam.enabled = true;
            endPos = hit.point;
            beam.SetPosition(0, firePoint.position);
            beam.SetPosition(1, endPos);
            zapSFX?.Play();

            if (normalProjScript != null)
            {
                normalProjScript.MarkDestroyedByParry();

                switch (normalProjScript.type)
                {
                    case ProjectileType.PrimaryNormal:
                        ballSpawner?.SpawnHomingRock(ProjectileType.PrimaryHoming, hitProj.transform.position, hitProj.transform.rotation);
                        break;
                    case ProjectileType.SecondaryNormal:
                        ballSpawner?.SpawnHomingRock(ProjectileType.SecondaryHoming, hitProj.transform.position, hitProj.transform.rotation);
                        break;
                }
                Destroy(hitProj); 
            }
        }

        // Disable beam
        yield return new WaitForSeconds(zapDuration);
        beam.enabled = false;
    }

    void OnDisable()
    {
        beam.enabled = false;
    }
}
