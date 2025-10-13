using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class BallSpawner : MonoBehaviour
{
    public GameObject projectile;
    public AudioSource parrySFX;
    public Transform shootPoint;
    public BarrelAim barrelAim;
    public float spawnPerSecond = 5f;
    public bool isCannon = false;
    private float spawnRate;
    private float timer = 0f;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRate = 1f / spawnPerSecond;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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
                RaycastHit hit;
                float radius = 1f;
                if (Physics.SphereCast(ray, radius, out hit) && hit.collider.gameObject.tag == "Tower Projectile")
                {
                    SpawnBall(hit.collider.gameObject);
                }
            }
            else
            {
                return;
            }
        }
        else if (timer >= spawnRate)
        {
            SpawnBall();
        }

    }

    private IEnumerator FireWithAim(GameObject target)
    {
        yield return StartCoroutine(barrelAim.AimAtTarget(target));
        GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
        proj.GetComponent<Homing>().SetTarget(target);
        timer = 0;
    }

    // Homing Rock Spawner
    public void SpawnBall(Vector3 position, Quaternion rotation)
    {
        GameObject homingRock = Instantiate(projectile, position, rotation);
        Homing homingScript = homingRock.GetComponent<Homing>();
        homingScript.EnableEffects();
    }

    // Cannon Ball Spawner
    public void SpawnBall(GameObject target)
    {
        if (barrelAim != null && target != null)
        {
            StartCoroutine(FireWithAim(target));
        }
    }

    // Normal Rock Spawner
    public void SpawnBall()
    {
        Instantiate(projectile, transform.position, transform.rotation);
        timer = 0;
    }

    
}
