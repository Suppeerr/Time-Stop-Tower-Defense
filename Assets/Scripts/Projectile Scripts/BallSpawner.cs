using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class BallSpawner : MonoBehaviour
{
    // Spawn fields
    public float spawnPerSecond = 5f;
    private float spawnRate;
    private float timer = 1f;

    // Spawner fields
    private Transform shootPoint;
    public bool isCannon = false;
    public bool isAutoCannon = false;
    private bool isPrePlaced = false;
    
    // Effects and Audio
    private BarrelAim barrelAim;
    public ParticleSystem muzzleFlash;
    public AudioSource cannonBlastSFX;

    void Start()
    {
        // Initializes fields
        barrelAim = GetComponentInParent<BarrelAim>();
        shootPoint = this.transform;
        
        spawnRate = 1f / spawnPerSecond;
    }

    void Update()
    {
        // Frozen if time stopped, the level has not started, or the level is over
        if (TimeStop.Instance.IsFrozen || !LevelStarter.HasLevelStarted || SettingsMenuOpener.Instance.MenuOpened || BaseHealthManager.IsGameOver)
        {
            return;
        }

        timer += Time.deltaTime;

        CheckIfCannon();
    }

    // Spawns cannon ball if spawner is a cannon and a normal rock otherwise
    private void CheckIfCannon()
    {
        if (isCannon)
        {
            // Fires a cannon ball to a clicked projectile
            if ((Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && timer >= spawnRate)
            {
                Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                int projectileLayer = LayerMask.GetMask("Normal Projectile");
                float radius = 0.5f;

                if (Physics.SphereCast(ray, radius, out var hit, Mathf.Infinity, projectileLayer))
                {
                    SpawnCannonBall(hit.collider.gameObject);
                }
            }
        }
        else if (isAutoCannon && timer >= spawnRate)
        {
            // Fires a cannon ball to a random projectile
            SpawnCannonBall(ProjectileManager.Instance.GetRandomNormalProjectile());
        }
        else if (timer >= spawnRate)
        {
            if (isPrePlaced)
            {
                timer = 1f;
                isPrePlaced = false;
                return;
            }
            SpawnNormalRock(ProjectileType.PrimaryNormal, transform.position, transform.rotation);
        }
    }

    // Spawns a cannon ball that homes to the specified target
    public void SpawnCannonBall(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        GameObject prefabToSpawn = ProjectileManager.Instance.GetProjectilePrefab(ProjectileType.CannonBall);

        barrelAim?.AimAtTarget(target.transform);

        GameObject proj = Instantiate(prefabToSpawn, shootPoint.position, shootPoint.rotation);
        proj.GetComponent<HomingProjectile>()?.SetTarget(target);
        cannonBlastSFX?.Play();
        muzzleFlash?.Play();
        timer = 0f;
    }

    // Spawns a normal rock at a specified location
    public void SpawnNormalRock(ProjectileType type, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToSpawn = null;

        switch (type)
        {
            case ProjectileType.PrimaryNormal:
                prefabToSpawn = ProjectileManager.Instance.GetProjectilePrefab(ProjectileType.PrimaryNormal);
                break;
            case ProjectileType.SecondaryNormal:
                prefabToSpawn = ProjectileManager.Instance.GetProjectilePrefab(ProjectileType.SecondaryNormal);;
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

    // Spawns a homing projectile at a specified location
    public void SpawnHomingRock(ProjectileType type, Vector3 position, Quaternion rotation, bool cannonParried = false)
    {
        GameObject prefabToSpawn = null;

        switch (type)
        {
            case ProjectileType.PrimaryHoming:
                prefabToSpawn = ProjectileManager.Instance.GetProjectilePrefab(ProjectileType.PrimaryHoming);;
                break;
            case ProjectileType.SecondaryHoming:
                prefabToSpawn = ProjectileManager.Instance.GetProjectilePrefab(ProjectileType.SecondaryHoming);;
                break;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned for this homing type!");
            return;
        }

        GameObject homingProjectile = Instantiate(prefabToSpawn, position, rotation);
        HomingProjectile homingProjScript = homingProjectile.GetComponent<HomingProjectile>();
        
        // Checks for whether the projectile was parried or zapped
        if (cannonParried)
        {
            homingProjScript.ChangeLayer();
        }
        else
        {
            homingProjScript.EnableChargeEffects(1);
            homingProjScript.IncrementChargeLevel();
        }
    }

    // Prevents pre-placed towers from immediately spawning a projectile after the level starts
    void OnEnable()
    {
        if (!LevelStarter.HasLevelStarted)
        {
            isPrePlaced = true;
        }
    }
}
