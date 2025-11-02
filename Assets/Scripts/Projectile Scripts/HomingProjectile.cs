using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HomingProjectile : MonoBehaviour
{
    // Targeting
    protected Transform target;
    
    // Arc / Steering
    protected float steerSpeed = 10f;
    protected float maxSpeed = 10f;
    public float arcBoost = 2f;

    // Behavior / Type
    public float destroyAfter = 15f;
    public ProjectileType type;

    // Effects
    public LightningRing lightningRing;
    public GlowingSphereEmitter sphereEmitter;
    public ParticleSystem sparksPrefab;
    public ParticleSystem flashPrefab;

    // Stats
    public ProjectileStatsContainer statsContainer;
    private int damage;
    private float aoe;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ProjectileManager.Instance.RegisterProjectile(rb);
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;

        if (ProjectileManager.IsFrozen)
        {
            rb.useGravity = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assigns stat values to projectiles
        ProjectileStats stats = statsContainer.GetStats(type);
        damage = stats.damage;
        steerSpeed = stats.speed;
        maxSpeed = stats.speed;
        aoe = stats.aoeRadius;

        AssignNearestEnemy();

        // Destroys projectile after destroyAfter seconds
        if (destroyAfter > 0f)
        {
            Destroy(gameObject, destroyAfter);
        }
    }

    // Updates for projectile physics 
    protected virtual void FixedUpdate()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        else if (rb.useGravity == false)
        {
            rb.useGravity = true;
        }
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        HomeToTarget();
    }

    // Assigns the nearest enemy to the projectile
    private void AssignNearestEnemy()
    {
        // Homes onto nearest enemy if no target assigned in BallSpawner
        if (target == null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float minDist = Mathf.Infinity;
            Vector3 pos = transform.position;

            foreach (GameObject e in enemies)
            {
                float dist = (e.transform.position - pos).sqrMagnitude;
                if (dist < minDist)
                {
                    closest = e;
                    minDist = dist;
                }
            }
            if (closest != null)
            {
                target = closest.transform;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Homes the projectile to the given target
    protected virtual void HomeToTarget()
    {
        // Vector to target
        Vector3 disp = target.position - transform.position;
        float distance = disp.magnitude;

        // Estimate time to reach target
        float t = distance / Mathf.Max(0.1f, maxSpeed);

        // Compute velocity needed to reach target in t seconds
        Vector3 desiredVelocity = new Vector3(
            disp.x / t,
            (disp.y / t) - 0.5f * Physics.gravity.y * t,
            disp.z / t
        );
        desiredVelocity.y += arcBoost;

        // Clamp horizontal speed
        Vector3 horizontal = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);
        if (horizontal.magnitude > maxSpeed)
        {
            horizontal = horizontal.normalized * maxSpeed;
        }
            
        desiredVelocity.x = horizontal.x;
        desiredVelocity.z = horizontal.z;

        // Smoothly adjust current velocity toward desired
        float alpha = 1f - Mathf.Exp(-steerSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, alpha);

        // Rotate projectile to face movement
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    // Runs when projectile collides with a valid object
    void OnCollisionEnter(Collision collision)
    {
        SpawnExplosion();

         // Collider hit
        GameObject hitObj = collision.collider.gameObject;

        // Check up the hierarchy for an EnemyProxy
        EnemyProxy proxy = hitObj.GetComponent<EnemyProxy>();
        if (proxy == null)
            proxy = hitObj.GetComponentInParent<EnemyProxy>();

        if (proxy != null && proxy.enemyData != null)
        {
            proxy.enemyData.TakeDamage(new DamageInstance(damage));
        }

        Destroy(gameObject);
    }

    // Enables lightning and sphere emitter effects
    public void EnableEffects()
    {
        if (!ProjectileManager.IsFrozen)
        {
            return;
        }

        if (lightningRing != null)
        {
            lightningRing.enabled = true;
            lightningRing.SetVisible(true);
        }

        if (sphereEmitter != null)
        {
            sphereEmitter.enabled = true;
        }
    }

    // Spawns explosion on collision with enemy
    private void SpawnExplosion()
    {
        if (sparksPrefab != null)
        {
            ParticleSystem s = Instantiate(sparksPrefab, transform.position, transform.rotation);
            s.Play();
            Destroy(s.gameObject, s.main.duration);
        }

        if (flashPrefab != null)
        {
            ParticleSystem f = Instantiate(flashPrefab, transform.position, transform.rotation);
            f.Play();
            Destroy(f.gameObject, f.main.duration);
        }
    }

    // Sets the homing projectile's target manually
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget.transform;
    }

    // Unregisters projectiles when destroyed
    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}

