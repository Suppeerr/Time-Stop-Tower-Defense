using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Homing : MonoBehaviour
{
    // Targeting
    public Transform target;
    public string targetTag = "Enemy";

    // Arc / Steering
    public float timeToTarget = 1f;
    public float steerSpeed = 10f;
    public float maxSpeed = 1f;
    private float arcBoost = 2f;

    // Behavior
    public float destroyAfter = 10f;
    public LayerMask collisionMask = ~0;
    public float raycastSafetyPadding = 0.1f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;

        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            GameObject t = GameObject.FindWithTag(targetTag);
            if (t != null)
            {
                target = t.transform;
            }
        }
        if (target != null)
        {
            Vector3 disp = target.position - transform.position;
            float verticalDisp = disp.y;
        }

        if (destroyAfter > 0f)
        {
            Destroy(gameObject, destroyAfter);
        }
    }

    // Updates for projectile physics 
    void FixedUpdate()
    {
        if (target == null) return;

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

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider, collision.contacts[0].point, collision.contacts[0].normal);
    }

    void HandleHit(Collider collider, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (collider.transform == target)
        {
            Debug.Log("Projectile hit target!");
            // add damage logic here
        }
        else
        {
            Debug.Log("Projectile hit " + collider.name);
        }

        Destroy(gameObject);
    }
}

