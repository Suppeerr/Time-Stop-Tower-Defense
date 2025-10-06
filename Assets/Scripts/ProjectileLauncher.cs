using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public float initialXVel = 15f;
    public float initialYVel = 15f;
    public float initialZVel = 0f;

    public BallSpawner ballSpawner;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
