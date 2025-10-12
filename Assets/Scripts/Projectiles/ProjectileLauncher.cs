using UnityEngine;
using static UnityEngine.Random;

public class ProjectileLauncher : MonoBehaviour
{
    public BallSpawner ballSpawner;
    private float initialXVel = 0f;
    private float initialYVel = 11f;
    private float initialZVel = 0f;
    private float gravityMultiplier = 0.8f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialXVel = Random.Range(-0.8f, 0.8f);
        initialZVel = Random.Range(-0.8f, 0.8f);
        rb = GetComponent<Rigidbody>();
        ProjectileManager.Instance.RegisterProjectile(rb);
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = new Vector3(initialXVel, initialYVel, initialZVel);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cannon Projectile"))
        {
            if (ballSpawner != null)
            {
                ballSpawner.SpawnBall(transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}
