using UnityEngine;

public class HomingCoin : HomingProjectile
{
    // Drone object
    private GameObject drone;
    
    // Barrel aim script
    private BarrelAim barrelAim;

    void Awake()
    {
        // Initializes fields and registers the coin projectile
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;
        drone = GameObject.Find("Colored Money Drone");
        barrelAim = drone.GetComponentInChildren<BarrelAim>();

        ProjectileManager.Instance.RegisterProjectile(rb);
    }

    void Start()
    {
        // Assigns a target for the coin and initializes stats
        AssignSafe();
        ProjectileStats stats = statsContainer.GetStats(type);
        steerSpeed = stats.speed;
        maxSpeed = stats.speed;

        Destroy(gameObject, destroyAfter);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void HomeToTarget()
    {
        base.HomeToTarget();
    }
    
    // Assigns the safe collector target to the coin
    private void AssignSafe()
    {
        if (target == null)
        {
            GameObject safe = GameObject.FindGameObjectWithTag("Safe Collector");
            target = safe.transform;
            barrelAim.AimAtTarget(target);
        }
    }
}
