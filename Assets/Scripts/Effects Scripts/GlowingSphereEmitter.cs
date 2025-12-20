using UnityEngine;

public class GlowingSphereEmitter : MonoBehaviour
{
    // Glowing sphere prefab
    [SerializeField] private GameObject spherePrefab;     

    // Sphere parameters
    [SerializeField] private int spheresPerSecond = 20;   
    [SerializeField] private float sphereSpeed = 2f;      
    [SerializeField] private float lifetime = 1f;         
    [SerializeField] private float spawnRadius = 0.1f;    
    private float spawnTimer = 0f;      
    
    void Awake()
    {
        this.enabled = false;
    }

    void Update()
    {
        // Rapidly spawns glowing spheres around a charged projectile 
        spawnTimer += Time.deltaTime;
        float interval = 1f / spheresPerSecond;

        while (spawnTimer >= interval)
        {
            spawnTimer -= interval;
            SpawnSphere();
        }
    }

    // Initializes and directs movement for each sphere
    void SpawnSphere()
    {
        // Randomize direction for each sphere
        Vector3 direction = Random.onUnitSphere;
        Vector3 spawnPos = transform.position + direction * spawnRadius;

        GameObject s = Instantiate(spherePrefab, spawnPos, Quaternion.identity);

        // Make the sphere move outward
        Rigidbody rb = s.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = direction * sphereSpeed;

        // Destroy after lifetime
        Destroy(s, lifetime);
    }
}
