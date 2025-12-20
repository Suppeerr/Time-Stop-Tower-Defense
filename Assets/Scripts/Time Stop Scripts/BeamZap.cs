using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    private bool wasFrozen = false;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        bool isFrozen = ProjectileManager.IsFrozen;
        if (Upgrader.PreChargeBought && isFrozen != wasFrozen)
        {
            Debug.Log($"isFrozen:{isFrozen}, wasFrozen:{wasFrozen}");

            if (isFrozen)
            {
                PreChargeProjectiles();
            }
            
            wasFrozen = isFrozen;
        }

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
        int projectileLayers = LayerMask.GetMask("Normal Projectile");

        if (Upgrader.MultiChargeBought)
        {
            projectileLayers |= 1 << LayerMask.NameToLayer("Homing Projectile");
        }

        if (Physics.SphereCast(ray, radius, out hit, Mathf.Infinity, projectileLayers))
        {
            GameObject hitProj = hit.collider.gameObject;
            NormalProjectile normalProjScript = hitProj.GetComponent<NormalProjectile>();
            HomingProjectile homingProjScript = hitProj.GetComponent<HomingProjectile>();

            beam.enabled = true;
            endPos = hit.point;
            beam.SetPosition(0, firePoint.position);
            beam.SetPosition(1, endPos);
            zapSFX?.Play();

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
            if (normalProjScript != null)
            {
                SpawnHomingProjectile(hitProj, normalProjScript);
                Destroy(hitProj);
            }   
        }

        // Disable beam
        yield return new WaitForSeconds(zapDuration);
        beam.enabled = false;
    }

    private void SpawnHomingProjectile(GameObject proj, NormalProjectile normalProjScript)
    {
        if (normalProjScript != null)
            {
                normalProjScript.MarkDestroyedByParry();

                switch (normalProjScript.type)
                {
                    case ProjectileType.PrimaryNormal:
                        ballSpawner?.SpawnHomingRock(ProjectileType.PrimaryHoming, proj.transform.position, proj.transform.rotation);
                        break;
                    case ProjectileType.SecondaryNormal:
                        ballSpawner?.SpawnHomingRock(ProjectileType.SecondaryHoming, proj.transform.position, proj.transform.rotation);
                        break;
                }
            }
    }

    private void PreChargeProjectiles()
    {
        List<GameObject> normalProjList = ProjectileManager.Instance.GetNormalProjectileList(0f);

        foreach (GameObject proj in normalProjList)
        {
            int random = Random.Range(0, 9);

            if (random <= 1)
            {
                SpawnHomingProjectile(proj, proj.GetComponent<NormalProjectile>());
                Destroy(proj);
            }
        }
    }

    void OnEnable()
    {
        wasFrozen = false;
    }

    void OnDisable()
    {
        beam.enabled = false;
    }
}
