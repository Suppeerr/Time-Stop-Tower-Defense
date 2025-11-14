using UnityEngine;

public class HomingCoin : HomingProjectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignSafe();
        ProjectileStats stats = statsContainer.GetStats(type);
        steerSpeed = stats.speed;
        maxSpeed = stats.speed;

        // Destroys coin after destroyAfter seconds
        if (destroyAfter > 0f)
        {
            Destroy(gameObject, destroyAfter);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void HomeToTarget()
    {
        base.HomeToTarget();
    }
    
    // Assigns the safe to the coin
    private void AssignSafe()
    {
        // Homes onto safe
        if (target == null)
        {
            GameObject safe = GameObject.FindGameObjectWithTag("Safe Collector");
            target = safe.transform;
        }
    }
}
