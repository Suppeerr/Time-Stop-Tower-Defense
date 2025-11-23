using UnityEngine;
using static UnityEngine.Random;

public class NormalProjectile : MonoBehaviour
{
    public BallSpawner ballSpawner;
    public ProjectileType type;
    private float initialXVel = -1f;
    public float initialYVel = 0f;
    private float initialZVel = -1f;
    private float gravityMultiplier = 0.7f;
    private int splitCount = 2;
    public float spreadAngle = 30f;
    private float destroyAfter = 3f;
    private float lifetime = 0f;
    private bool parryDeath = false;
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
        else if (this.type == ProjectileType.PrimaryNormal)
        {
            lifetime += Time.deltaTime;
        }

        if (lifetime >= destroyAfter)
        {
            SplitAndDestroy();
        }

        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cannon Projectile"))
        {
            parryDeath = true;
            BecomeHoming();
        }
        else if (collision.collider.CompareTag("Ground") && type == ProjectileType.PrimaryNormal)
        {
            SplitAndDestroy();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BecomeHoming()
    {
        // Spawn homing projectile
        Destroy(gameObject);

        if (ballSpawner != null)
        {
            switch (this.type)
            {
                case ProjectileType.PrimaryNormal:
                    ballSpawner.SpawnHomingRock(ProjectileType.PrimaryHoming, transform.position, transform.rotation);
                    break;
                case ProjectileType.SecondaryNormal:
                    ballSpawner.SpawnHomingRock(ProjectileType.SecondaryHoming, transform.position, transform.rotation);
                    break;
            }
        }
    }

    public void MarkDestroyedByParry()
    {
        parryDeath = true;
    }

    void SplitAndDestroy()
    {
        if (this.type == ProjectileType.PrimaryNormal && !parryDeath)
        {
            for (int i = 0; i < splitCount; i++)
            {
                float angle = ((float)i / (splitCount - 1) - 0.5f) * spreadAngle;
                Quaternion rot = Quaternion.Euler(0, angle, 0) * transform.rotation;
                ballSpawner.SpawnNormalRock(ProjectileType.SecondaryNormal, transform.position, rot);
            }
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}
