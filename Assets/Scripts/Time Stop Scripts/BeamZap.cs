using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BeamZap : MonoBehaviour
{
    // Ball spawner script
    [SerializeField] private BallSpawner ballSpawner;

    // Beam fields
    [SerializeField] private LineRenderer beam;
    [SerializeField] private Transform firePoint;
    private float zapDuration = 0.1f;

    // Layers the beam can hit
    [SerializeField] private LayerMask hitLayers;

    // Pre-charge fields
    private int preChargePercentage = 25;
    
    // Zap sound and visual effect
    [SerializeField] private AudioSource zapSFX;
    [SerializeField] private GameObject zappedVFX;

    void Update()
    {
        // Zaps a projectile when it is clicked
        if (InputManager.Instance.Input.Gameplay.ZapParry.triggered && TimeStop.Instance.IsFrozen)
        {
            StartCoroutine(FireZap());
        }
    }

    // Zaps a clicked projectile
    private IEnumerator FireZap()
    {        
        // Defines the layers a zap can hit
        int zapLayers = LayerMask.GetMask("Normal Projectile", "Vine Tower");

        // Allows zapped homing projectiles to be zapped again and multi-charged
        if (UpgradeManager.Instance.IsBought(UpgradeType.MultiCharge))
        {
            zapLayers |= 1 << LayerMask.NameToLayer("Homing Projectile");
        }

        // Sphere casts to a projectile 
        Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        float radius = 0.8f;

        if (Physics.SphereCast(ray, radius, out hit, Mathf.Infinity, zapLayers))
        {
            GameObject hitObj = hit.collider.gameObject;
            NormalProjectile normalProjScript = hitObj.GetComponent<NormalProjectile>();
            HomingProjectile homingProjScript = hitObj.GetComponent<HomingProjectile>();
            VineGrower vineGrowerScript = hitObj.GetComponent<VineGrower>();

            beam.enabled = true;
            beam.SetPosition(0, firePoint.position);
            beam.SetPosition(1, hit.point);
            zapSFX?.Play();

            if (zappedVFX != null)
            {
                Vector3 beamDir = (hit.point - firePoint.position).normalized;
                Quaternion rot = Quaternion.LookRotation(-beamDir);
                Instantiate(zappedVFX, hit.point - beamDir * 0.35f, rot);
            }

            // Normal charging
            if (normalProjScript != null)
            {
                SpawnHomingProjectile(hitObj, normalProjScript);
                Destroy(hitObj);
            }

            // Multi-charging 
            if (homingProjScript != null)
            {
                MultiChargeProjectiles(homingProjScript);
            }

            // Vine charging
            if (vineGrowerScript != null)
            {
                vineGrowerScript.RechargeVines();
            }       
        }

        yield return new WaitForSecondsRealtime(zapDuration);
        beam.enabled = false;
    }

    // Tells the ball spawner to spawn a homing rock where the zapped projectile was
    private void SpawnHomingProjectile(GameObject proj, NormalProjectile normalProjScript)
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
    
    // Randomly pre-charges several projectiles if the upgrade is bought
    public void PreChargeProjectiles()
    {
        if (!UpgradeManager.Instance.IsBought(UpgradeType.PreCharge))
        {
            return;
        }

        List<GameObject> normalProjList = ProjectileManager.Instance.GetNormalProjectileList(0f);

        foreach (GameObject proj in normalProjList)
        {
            if (Random.Range(0, 100) <= preChargePercentage)
            {
                SpawnHomingProjectile(proj, proj.GetComponent<NormalProjectile>());
                Destroy(proj);
            }
        }
    }

    // Multi-charges projectiles if the upgrade is bought
    private void MultiChargeProjectiles(HomingProjectile homingProj)
    {
        homingProj.IncrementChargeLevel();
        homingProj.ChangeLayer();
        homingProj.EnableChargeEffects(2);

        if (homingProj.type == ProjectileType.PrimaryHoming)
        {
            homingProj.AddDamage(400);
        }
    }
}
