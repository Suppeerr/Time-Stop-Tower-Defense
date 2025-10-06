using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Homing : MonoBehaviour
{
    // Targeting
    public Transform target;
    
    // Arc / Steering
    public float steerSpeed = 10f;
    public float maxSpeed = 10f;
    public float arcBoost = 2f;

    // Behavior
    public float destroyAfter = 10f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ProjectileManager.Instance.RegisterProjectile(rb);
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;

        // Homes onto nearest enemy if no target assigned inb BallSpawner
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

        // Destroys projectile after destroyAfter seconds
        if (destroyAfter > 0f)
        {
            Destroy(gameObject, destroyAfter);
        }
    }

    // Updates for projectile physics 
    void FixedUpdate()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

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
            horizontal = horizontal.normalized * maxSpeed;
        desiredVelocity.x = horizontal.x;
        desiredVelocity.z = horizontal.z;

        // Smoothly adjust current velocity toward desired
        float alpha = 1f - Mathf.Exp(-steerSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, alpha);

        // Rotate projectile to face movement
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget.transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    void HandleHit(Collider collider)
    {
        if (collider.transform == target)
        {
            //    Debug.Log("Projectile hit target!");
            // add damage logic here

            Destroy(gameObject);
        }
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}

