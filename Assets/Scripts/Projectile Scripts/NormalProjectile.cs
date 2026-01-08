using UnityEngine;
using static UnityEngine.Random;

public class NormalProjectile : MonoBehaviour
{
    // Ball spawner script
    [SerializeField] private BallSpawner ballSpawner;

    // Clickable and outline flash scripts
    private Clickable clickableScript;
    private OutlineFlash outlineFlashScript;

    // Projectile lifetime and type
    private float timeAfterSpawn = 0f;
    [HideInInspector] public ProjectileType type;

    // Initial spawn coordinates
    private float initialXVel;
    [SerializeField] private float initialYVel = 0f;
    private float initialZVel;

    // Gravity multiplier
    private float gravityMultiplier = 0.7f;

    // Split fields
    private float splitTimer = 3f;
    [SerializeField] private int splitCount = 2;
    [SerializeField] private float spreadAngle = 30f;
    
    // Parry field
    private bool parryDeath = false;

    // Projectile rigidbody
    private Rigidbody rb;
    
    void Start()
    {
        // Randomizes spawn position, registers projectile, and initializes fields
        initialXVel = Random.Range(-0.8f, 0.8f);
        initialZVel = Random.Range(-0.8f, 0.8f);

        clickableScript = this.GetComponent<Clickable>();
        outlineFlashScript = this.GetComponent<OutlineFlash>();
        
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode =  CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = new Vector3(initialXVel, initialYVel, initialZVel);

        ProjectileManager.Instance.RegisterProjectile(rb);
    }

    void FixedUpdate()
    {
        // Splits primary projectiles after some time
        if (this.type == ProjectileType.PrimaryNormal)
        {
            timeAfterSpawn += Time.deltaTime;
        }

        if (timeAfterSpawn >= splitTimer)
        {
            SplitAndDestroy();
        }

        // Shifts the gravity on projectiles
        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
    }

    // Handles collisions between the projectile and other objects
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

    // Spawns a homing projectile where the normal projectile was destroyed
    void BecomeHoming()
    {
        Destroy(gameObject);

        if (ballSpawner != null)
        {
            switch (this.type)
            {
                case ProjectileType.PrimaryNormal:
                    ballSpawner.SpawnHomingRock(ProjectileType.PrimaryHoming, transform.position, transform.rotation, true);
                    break;
                case ProjectileType.SecondaryNormal:
                    ballSpawner.SpawnHomingRock(ProjectileType.SecondaryHoming, transform.position, transform.rotation, true);
                    break;
            }
        }
    }

    // Marks the projectile as destroyed by a parry/zap, not the splitting
    public void MarkDestroyedByParry()
    {
        parryDeath = true;
    }

    // Splits the primary projectile into multiple secondary projectiles
    public void SplitAndDestroy()
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

    // Updates the projectile's outline flash
    public void UpdateBlinking(bool blinkCon)
    {
        if (blinkCon)
        {
            outlineFlashScript.StartFlashing();
        }
        else
        {
            outlineFlashScript.StopFlashing(false);
        }
    }

    // Unregisters the projectile upon its destruction
    void OnDestroy()
    {
        ProjectileManager.Instance?.UnregisterProjectile(rb);
    }
}
