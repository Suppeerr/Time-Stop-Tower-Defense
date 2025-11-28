using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HomingProjectile : MonoBehaviour
{
    // Targeting
    protected Transform target;
    private Transform lastHitEnemy;
    private int hitEnemies = 0;
    private bool isPiercing = false;
    private LevelInstance level;
    private Collider myCollider;
    
    // Arc / Steering
    protected float steerSpeed = 10f;
    protected float maxSpeed = 10f;
    public float arcBoost = 2f;

    // Behavior / Type
    public float destroyAfter = 15f;
    public ProjectileType type;

    // Effects
    public GameObject normalLightningRingPrefab;
    public GameObject upgradedLightningRingPrefab;
    public GlowingSphereEmitter sphereEmitter;
    public GameObject normalExplosionPrefab;
    public GameObject upgradedExplosionPrefab;
    private int chargeLevel = 0;

    // Stats
    public ProjectileStatsContainer statsContainer;
    private int damage;
    private float aoe;

    protected Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ProjectileManager.Instance.RegisterProjectile(rb);
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;
        level = LevelInstance.Instance;
        myCollider = GetComponent<Collider>();

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

        if (type == ProjectileType.PrimaryHoming)
        {
            AssignNearestEnemy();
        }
        else if (type == ProjectileType.SecondaryHoming)
        {
            AssignFirstEnemy();
        }

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

        if ((target == null && !isPiercing) || BaseHealthManager.IsGameOver)
        {
            Destroy(gameObject);
            return;
        }

        HomeToTarget();
    }

    // Homes the projectile to the given target
    protected virtual void HomeToTarget()
    {
        if (!target)
        {
            if (isPiercing)
            {
                AssignSecondEnemy();
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }

        // Vector to target
        Vector3 disp = target.position - rb.position;
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
        if (type == ProjectileType.PrimaryHoming)
        {
            if (chargeLevel <= 1)
            {
                DoAoEDamage(0f);
                SpawnNormalExplosion();
            }
            else
            {
                DoAoEDamage(2f);
                SpawnUpgradedExplosion();
            }

            Destroy(gameObject);
            return;
        }

        // Single target damage
        DoSingleTargetDamage(collision);
        SpawnNormalExplosion();

        if (chargeLevel == 2 && hitEnemies < 1)
        {
            Collider enemyCol = collision.collider;
            Physics.IgnoreCollision(myCollider, enemyCol, true);
            lastHitEnemy = enemyCol.transform;
            isPiercing = true;

            hitEnemies++;
            
            target = null;
            AssignSecondEnemy();
            
            return;
        }    

        Destroy(gameObject);
    }

    // Assigns the first enemy to the projectile
    private void AssignFirstEnemy()
    {
        BaseEnemy firstEnemy = level.GetFirstEnemy();
        if (firstEnemy != null)
        {
            target = firstEnemy.visualObj.transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Assigns the second enemy to the projectile
    private void AssignSecondEnemy()
    {
        BaseEnemy secondEnemy = level.GetSecondEnemy();
        if (secondEnemy != null)
        {
            target = secondEnemy.visualObj.transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Assigns the nearest enemy to the projectile
    private void AssignNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject e in enemies)
        {
            Transform t = e.transform;
            if (t == lastHitEnemy)
            {
                continue;
            }

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

    // Sets the homing projectile's target manually
    public void SetTarget(GameObject newTarget)
    {
        if (target == null && newTarget != null)
        {
            target = newTarget.transform;
        }
    }

    // Enables normal lightning and sphere emitter effects
    public void EnableNormalEffects()
    {
        if (!ProjectileManager.IsFrozen)
        {
            return;
        }

        if (normalLightningRingPrefab != null)
        {
            Instantiate(normalLightningRingPrefab, transform);
        }

        if (sphereEmitter != null)
        {
            sphereEmitter.enabled = true;
        }
    }

    // Enables upgraded lightning effects
    public void EnableUpgradedEffects()
    {
        if (!ProjectileManager.IsFrozen)
        {
            return;
        }

        if (upgradedLightningRingPrefab != null)
        {
            Instantiate(upgradedLightningRingPrefab, transform);
        }
    }

    // Spawns normal explosion on collision with enemy
    private void SpawnNormalExplosion()
    {
        if (type == ProjectileType.PrimaryHoming)
        {
            ProjectileManager.Instance.PlayExplosionSound();
        }
        
        if (normalExplosionPrefab != null)
        {
            GameObject normEx = Instantiate(normalExplosionPrefab, transform.position, transform.rotation);
            Destroy(normEx, 0.8f);
        }
    }

    // Spawns upgraded explosion on collision with enemy
    private void SpawnUpgradedExplosion()
    {
        if (type == ProjectileType.PrimaryHoming)
        {
            ProjectileManager.Instance.PlayExplosionSound();
        }

        if (upgradedExplosionPrefab != null)
        {
            GameObject upgradedEx = Instantiate(upgradedExplosionPrefab, transform.position, transform.rotation);
            Destroy(upgradedEx, 1f);
        }
    }

    // Does damage to hit targets in a radius
    private void DoAoEDamage(float aoeBoost)
    {
        // Get all colliders in radius - AoE damage
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoe + aoeBoost);
            
            foreach (Collider col in hitColliders)
            {
                // Check up the hierarchy for an EnemyProxy
                EnemyProxy proxy = col.GetComponent<EnemyProxy>();
                if (proxy == null)
                {
                    proxy = col.GetComponentInParent<EnemyProxy>();
                }  

                if (proxy != null && proxy.enemyData != null)
                {
                    proxy.enemyData.TakeDamage(new DamageInstance(damage));
                }
            }
        
    }

    // Does damage to a single hit target
    private void DoSingleTargetDamage(Collision collision)
    {
        // Check up the hierarchy for an EnemyProxy
        EnemyProxy proxy = collision.collider.GetComponent<EnemyProxy>();
        if (proxy == null)
        {
            proxy = collision.collider.GetComponentInParent<EnemyProxy>();
        }  

        if (proxy != null && proxy.enemyData != null)
        {
            proxy.enemyData.TakeDamage(new DamageInstance(damage));
        }
    }

    // Returns the charge level on the projectile
    public int GetChargeLevel()
    {
        return chargeLevel;
    }

    // Increments the charge level on the projectile by one
    public void IncrementChargeLevel()
    {
        chargeLevel++;
    }

    // Changes layer mask of projectile to ignore raycast
    public void ChangeLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // Adds damage to the projectile
    public void AddDamage(int dmg)
    {
        damage += dmg;
    }

    // Unregisters projectiles when destroyed
    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}

