using UnityEngine;
using UnityEngine.InputSystem;


public class BallSpawner : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject projectile;
    public AudioSource audioSource;
    public AudioClip parrySFX;
    public float spawnPerSecond = 5f;
    public bool isCannon = false;
    private float spawnRate;
    private float timer = 0f;

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

    public void SpawnBall(Vector3 position, Quaternion rotation)
    {
        audioSource.PlayOneShot(parrySFX);
        Instantiate(projectile, position, rotation);
    }

    public void SpawnBall(GameObject target)
    {
        GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
        proj.GetComponent<Homing>().SetTarget(target);
        timer = 0;
        Debug.Log("Spawned Ball!");
    }

    public void SpawnBall()
    {
        Instantiate(projectile, transform.position, transform.rotation);
        timer = 0;
    }
}
