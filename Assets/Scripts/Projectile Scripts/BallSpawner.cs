using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class BallSpawner : MonoBehaviour
{
    // Projectile Prefabs
    public GameObject homingPrimaryPrefab;
    public GameObject homingSecondaryPrefab;
    public GameObject normalPrimaryPrefab;
    public GameObject cannonBallPrefab;

    // Spawn Settings
    public float spawnPerSecond = 5f;
    public bool isCannon = false;
    private float spawnRate;
    private float timer = 0f;
    public Transform shootPoint;

    // Effects and Audio
    public AudioSource parrySFX;
    public BarrelAim barrelAim;
    public ParticleSystem muzzleFlash;

    // Ray Trace Camera
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRate = 1f / spawnPerSecond;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        SpawnNormal();
    }

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }

        timer += Time.deltaTime;
        if (isCannon)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && timer >= spawnRate)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                float radius = 1f;
                if (Physics.SphereCast(ray, radius, out var hit) && hit.collider.gameObject.tag == "Tower Projectile")
                {
                    SpawnCannonBall(hit.collider.gameObject, cannonBallPrefab);
                }
            }
        }
        else if (timer >= spawnRate)
        {
            SpawnNormal();
        }

    }

    // Cannon Ball Spawner
    public void SpawnCannonBall(GameObject target, GameObject projectilePrefab)
    {
        StartCoroutine(FireWithAim(target, projectilePrefab));
    }

    private IEnumerator FireWithAim(GameObject target, GameObject projectilePrefab)
    {
        yield return StartCoroutine(barrelAim.AimAtTarget(target));
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        proj.GetComponent<HomingProjectile>()?.SetTarget(target);
        muzzleFlash?.Play();
        timer = 0f;
    }

    // Normal Rock Spawner
    public void SpawnNormal()
    {
        Instantiate(normalPrimaryPrefab, transform.position, transform.rotation);
        timer = 0;
    }

    // Homing Rock Spawner
    public void SpawnHoming(ProjectileType type, Vector3 position, Quaternion rotation)
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

        HomingProjectile homingScript = homingProjectile.GetComponent<HomingProjectile>();
        homingScript.EnableEffects();
    }
}
