using UnityEngine;
using System.Collections.Generic;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;
    public AudioSource parrySFX;
    public AudioSource explosionSFX;
    public AudioSource normalHitSFX;

    public bool IsFrozen { get; private set; } = false;
    public List<Rigidbody> activeProjectiles = new List<Rigidbody>();
    Dictionary<Rigidbody, Vector3> savedVelocities = new Dictionary<Rigidbody, Vector3>();

    // Avoids duplicates of this object
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Registers new projectiles in a list of projectiles
    public void RegisterProjectile(Rigidbody projectile)
    {
        if (!activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Add(projectile);
        }
        if (projectile.CompareTag("Homing Projectile") && !IsFrozen)
        {
            parrySFX.Play();
        }
    }

    // Unregisters destroyed projectiles
    public void UnregisterProjectile(Rigidbody projectile)
    {
        activeProjectiles.Remove(projectile);
    }

    // Receives timestop event
    private void OnEnable()
    {
        TimeStop.TimeStopEvent += HandleToggle;
    }

    // Ends timestop event
    private void OnDisable()
    {
        TimeStop.TimeStopEvent -= HandleToggle;
    }

    public List<GameObject> GetNormalProjectileList(float minHeight)
    {
        List<GameObject> normalProjectiles = new List<GameObject>();
        
        foreach (var proj in activeProjectiles)
        {
            if (proj.gameObject.CompareTag("Tower Projectile") && proj.transform.position.y > minHeight)
            {
                normalProjectiles.Add(proj.gameObject);
            }
        }

        return normalProjectiles;
    }

    public GameObject GetRandomNormalProjectile()
    {
        List<GameObject> normalProjectiles = GetNormalProjectileList(13f);

        if (normalProjectiles.Count == 0)
        {
            return null;
        }

        return normalProjectiles[Random.Range(0, normalProjectiles.Count)];
    }

    public void ToggleNormalBlink(bool shouldBlink)
    {
        List<GameObject> normalProjectiles = GetNormalProjectileList(0f);

        foreach (GameObject normalProj in normalProjectiles)
        {
            normalProj.GetComponent<NormalProjectile>().UpdateBlinking(shouldBlink);
        }
    }

    // Freezes or unfreezes based on event state
    void HandleToggle(bool state)
    {
        IsFrozen = state;
        if (state)
        {
            FreezeProjectiles();
        }
        else
        {
            UnfreezeProjectiles();
        }
    }

    // Freezes all registered projectiles
    void FreezeProjectiles()
    {
        foreach (Rigidbody projectile in activeProjectiles)
        {
            savedVelocities[projectile] = projectile.linearVelocity;
            projectile.linearVelocity = Vector3.zero;
            projectile.useGravity = false;
        }
    }

    // Unfreezes all registered projectiles
    void UnfreezeProjectiles()
    {
        foreach (Rigidbody projectile in activeProjectiles)
        {
            if (savedVelocities.ContainsKey(projectile))
            {
                projectile.linearVelocity = savedVelocities[projectile];
                projectile.useGravity = true;

            }
        }
        savedVelocities.Clear();
    }

    // Plays explosion sound 
    public void PlayExplosionSound()
    {
        explosionSFX.Play();
    }

    // Plays normal hit sound 
    public void PlayNormalHitSound()
    {
        normalHitSFX.Play();
    }

    // Clears all projectiles on game over
    public void DestroyAllProjectiles()
    {
        foreach (var p in activeProjectiles)
        {
            Destroy(p.gameObject);
        }

        activeProjectiles.Clear();
    }
}
