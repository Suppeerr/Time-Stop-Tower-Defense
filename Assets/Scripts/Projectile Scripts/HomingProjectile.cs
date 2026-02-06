using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HomingProjectile : MonoBehaviour
{
    // Targeting fields
    protected Transform target;
    private Transform lastHitEnemy;
    private int hitEnemies = 0;
    private bool isPiercing = false;
    private LevelInstance level;
    private Collider myCollider;
    
    // Homing fields
    protected float steerSpeed = 10f;
    protected float maxSpeed = 10f;
    [SerializeField] private float arcBoost = 2f;

    // Projectile lifetime and type
    protected float destroyAfter = 15f;
    [HideInInspector] public ProjectileType type;

    // Charge fields
    [SerializeField] private GameObject normalLightningRingPrefab;
    [SerializeField] private GameObject upgradedLightningRingPrefab;
    [SerializeField] private GameObject glowingSpherePrefab;
    [SerializeField] private GameObject normalExplosionPrefab;
    [SerializeField] private GameObject upgradedExplosionPrefab;
    private int chargeLevel = 0;

    // Parry VFX
    [SerializeField] private GameObject parryVFX;

    // Projecitle stats
    [SerializeField] protected ProjectileStatsContainer statsContainer;
    private int damage;
    private float aoe;

    // Projectile rigidbody
    protected Rigidbody rb;

    void Awake()
    {
        // Initializes fields and registers the projectile
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;
        level = LevelInstance.Instance;
        myCollider = GetComponent<Collider>();

        ProjectileManager.Instance.RegisterProjectile(rb);
    }

    void Start()
    {
        // Assigns stat values to projectiles
        ProjectileStats stats = statsContainer.GetStats(type);
        damage = stats.damage;
        steerSpeed = stats.speed;
        maxSpeed = stats.speed;
        aoe = stats.aoeRadius;
        
        // Assigns a target to each projectile
        switch (type)
        {
            case ProjectileType.PrimaryHoming:
                AssignNearestEnemy();
                break;
            case ProjectileType.SecondaryHoming:
                AssignNthEnemy(1);
                break;
        }

        if (type != ProjectileType.CannonBall && target != null)
        {
            Vector3 dir = new Vector3();
            dir = (transform.position - target.transform.position).normalized;
            GameObject parryEffect = Instantiate(parryVFX, transform.position, Quaternion.LookRotation(dir));
            Destroy(parryEffect, 0.6f);
        }

        Destroy(gameObject, destroyAfter);
    }

    // Updates for projectile physics 
    protected virtual void FixedUpdate()
    {        
        if ((target == null && !isPiercing) || BaseHealthManager.Instance.IsGameOver)
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
                AssignNthEnemy(2);
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

        // Smoothly adjust current velocity toward desired velocity
        float alpha = 1f - Mathf.Exp(-steerSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, alpha);

        // Rotate projectile to face movement
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    // Detects collisions between the projectile and other objects
    void OnCollisionEnter(Collision collision)
    {
        if (type == ProjectileType.PrimaryHoming)
        {
            DoAoEDamage(chargeLevel, 2f);
            SpawnExplosion(chargeLevel);

            Destroy(gameObject);
            return;
        }
        else
        {
            // Single target damage
            DoSingleTargetDamage(collision);
            SpawnExplosion(chargeLevel);

            // Bounce to a second enemy after hitting one enemy
            if (chargeLevel == 2 && hitEnemies < 1)
            {
                Collider enemyCol = collision.collider;
                Physics.IgnoreCollision(myCollider, enemyCol, true);
                lastHitEnemy = enemyCol.transform;
                isPiercing = true;

                hitEnemies++;
                
                target = null;
                AssignNthEnemy(chargeLevel);
                
                return;
            }    
        }
        
        Destroy(gameObject);
    }

    // Assigns the nth enemy along the path to the projectile
    private void AssignNthEnemy(int n)
    {
        BaseEnemy enemy = null;

        if (n == 1)
        {
            enemy = level.GetFirstEnemy();
        }
        else
        {
            enemy = level.GetSecondEnemy();
        }
        
        if (enemy != null)
        {
            target = enemy.visualObj.transform;
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

    // Enables lightning and sphere emitter effects
    public void EnableChargeEffects(int charge)
    {
        if (glowingSpherePrefab != null)
        {
            Instantiate(glowingSpherePrefab, transform);
        }

        if (charge == 1)
        {
            if (normalLightningRingPrefab != null)
            {
                Instantiate(normalLightningRingPrefab, transform);
            }            
        }
        else if (charge == 2)
        {
            if (upgradedLightningRingPrefab != null)
            {
                Instantiate(upgradedLightningRingPrefab, transform);
            }
        }    
    }

    // Spawns explosion on collision with enemy
    private void SpawnExplosion(int chargeLevel)
    {
        if (type == ProjectileType.PrimaryHoming)
        {
            ProjectileManager.Instance.PlayExplosionSound();
        }
        else if (type == ProjectileType.SecondaryHoming)
        {   
            ProjectileManager.Instance.PlayNormalHitSound();
        }
        
        if (normalExplosionPrefab != null && chargeLevel < 2)
        {
            GameObject normEx = Instantiate(normalExplosionPrefab, transform.position, transform.rotation);
            Destroy(normEx, 0.8f);
        }
        else if (upgradedExplosionPrefab != null && chargeLevel == 2)
        {
            GameObject upgradedEx = Instantiate(upgradedExplosionPrefab, transform.position, transform.rotation);
            Destroy(upgradedEx, 1f);
        }
    }

    // Does damage to hit targets in a radius
    private void DoAoEDamage(int charge, float aoeBoost)
    {
        if (charge == 2)
        {
            aoe += aoeBoost;
        }

        // Get all colliders in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoe);
        
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

    // Unregisters the projecitle when destroyed
    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}

