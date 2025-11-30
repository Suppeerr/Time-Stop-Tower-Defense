using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class BallSpawner : MonoBehaviour
{
    // Projectile Manager Script
    private ProjectileManager projectileManagerScript;

    // Projectile Prefabs
    public GameObject homingPrimaryPrefab;
    public GameObject homingSecondaryPrefab;
    public GameObject normalPrimaryPrefab;
    public GameObject normalSecondaryPrefab;
    public GameObject cannonBallPrefab;

    // Spawn Settings
    public float spawnPerSecond = 5f;
    public bool isCannon = false;
    public bool isAutoCannon = false;
    private float spawnRate;
    private float timer = 1f;
    private Transform shootPoint;

    // Effects and Audio
    private BarrelAim barrelAim;
    public ParticleSystem muzzleFlash;
    public AudioSource cannonBlastSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barrelAim = GetComponentInParent<BarrelAim>();
        shootPoint = this.transform;

        projectileManagerScript = ProjectileManager.Instance.GetComponent<ProjectileManager>();
        
        spawnRate = 1f / spawnPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen || !LevelStarter.HasLevelStarted)
        {
            return;
        }

        timer += Time.deltaTime;

        CheckIfCannon();
    }

    // Spawns cannonball if spawner is a cannon and a normal rock otherwise
    private void CheckIfCannon()
    {
        if (isCannon)
        {
            if ((Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && timer >= spawnRate)
            {
                Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                int projectileLayer = LayerMask.GetMask("Normal Projectile");
                float radius = 0.5f;
                if (Physics.SphereCast(ray, radius, out var hit, Mathf.Infinity, projectileLayer))
                {
                    SpawnCannonBall(hit.collider.gameObject, cannonBallPrefab);
                }
            }
        }
        else if (isAutoCannon && timer >= spawnRate)
        {
            SpawnCannonBall(projectileManagerScript.GetRandomNormalProjectile(), cannonBallPrefab);
        }
        else if (timer >= spawnRate)
        {
            SpawnNormalRock(ProjectileType.PrimaryNormal, transform.position, transform.rotation);
        }
    }

    // Cannon Ball Spawner
    public void SpawnCannonBall(GameObject target, GameObject projectilePrefab)
    {
        FireWithAim(target, projectilePrefab);
    }

    private void FireWithAim(GameObject target, GameObject projectilePrefab)
    {
        if (target == null)
        {
            return;
        }

        barrelAim?.AimAtTarget(target.transform);
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        proj.GetComponent<HomingProjectile>()?.SetTarget(target);
        cannonBlastSFX?.Play();
        muzzleFlash?.Play();
        timer = 0f;
    }

    // Normal Rock Spawner
    public void SpawnNormalRock(ProjectileType type, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToSpawn = null;

        switch (type)
        {
            case ProjectileType.PrimaryNormal:
                prefabToSpawn = normalPrimaryPrefab;
                break;
            case ProjectileType.SecondaryNormal:
                prefabToSpawn = normalSecondaryPrefab;
                break;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned for this normal type!");
            return;
        }

        Instantiate(prefabToSpawn, position + Vector3.up * 0.1f, rotation);
        timer = 0f;
    }

    // Homing Rock Spawner
    public void SpawnHomingRock(ProjectileType type, Vector3 position, Quaternion rotation, bool cannonParried = false)
    {
        GameObject prefabToSpawn = null;

        switch (type)
        {
            case ProjectileType.PrimaryHoming:
                prefabToSpawn = homingPrimaryPrefab;
                break;
            case ProjectileType.SecondaryHoming:
                prefabToSpawn = homingSecondaryPrefab;
                break;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned for this homing type!");
            return;
        }

        GameObject homingProjectile = Instantiate(prefabToSpawn, position, rotation);
        HomingProjectile homingProjScript = homingProjectile.GetComponent<HomingProjectile>();
        
        if (cannonParried)
        {
            homingProjScript.ChangeLayer();
        }
        homingProjScript.EnableNormalEffects();
        homingProjScript.IncrementChargeLevel();
    }
}
