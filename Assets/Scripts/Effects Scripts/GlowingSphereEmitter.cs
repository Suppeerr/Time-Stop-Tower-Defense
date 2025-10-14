using UnityEngine;

public class GlowingSphereEmitter : MonoBehaviour
{
    public GameObject spherePrefab;     // Glowing sphere prefab
    public int spheresPerSecond = 20;   // Emission rate
    public float sphereSpeed = 2f;      // Speed of outward movement
    public float lifetime = 1f;         // How long each sphere lasts
    public float spawnRadius = 0.1f;    // Small offset from projectile center
    private float spawnTimer = 0f;      // Time between sphere spawns

    void Update()
    {
        // Spawns glowing spheres around a projectile
        spawnTimer += Time.deltaTime;
        float interval = 1f / spheresPerSecond;

        while (spawnTimer >= interval)
        {
            spawnTimer -= interval;
            SpawnSphere();
        }
    }

    void SpawnSphere()
    {
        // Random direction for each sphere
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
