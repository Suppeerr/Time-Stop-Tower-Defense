using UnityEngine;
using System.Collections.Generic;

public class ProjectileManager : MonoBehaviour
{
    // Projectile manager instance
    public static ProjectileManager Instance;

    // Projectile sound effects
    [SerializeField] private AudioSource parrySFX;
    [SerializeField] private AudioSource explosionSFX;
    [SerializeField] private AudioSource normalHitSFX;

    // List of currently active projectiles in the scene
    private List<Rigidbody> activeProjectiles = new List<Rigidbody>();

    // Dictionary mapping projectile types to their respective prefab
    [SerializeField] private Dictionary<ProjectileType, GameObject> projectileDict = new Dictionary<ProjectileType, GameObject>();

    // Projectile stats container
    [SerializeField] private ProjectileStatsContainer statsContainer;

    private void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }

        FillProjectileDictionary();
    }

    // Fills the projectile lookup dictionary with projectile prefabs
    private void FillProjectileDictionary()
    {
        foreach (ProjectileType type in statsContainer.GetProjectileTypes())
        {
            GameObject prefab = statsContainer.GetStats(type).projectilePrefab;

            if (prefab != null)
            {
                projectileDict[type] = prefab;
            }
            else
            {
                Debug.LogWarning("Projectile prefab for " + type + " not found!");
            }
        }
    }

    // Gets a projectile prefab from the lookup dictionary using a specified projectile type
    public GameObject GetProjectilePrefab(ProjectileType type)
    {
        if (projectileDict.TryGetValue(type, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning("Projectile prefab for " + type + " not found!");
        return null;
    }

    // Registers new projectiles in a list of active projectiles
    public void RegisterProjectile(Rigidbody projectile)
    {
        if (!activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Add(projectile);
        }

        if (projectile.CompareTag("Homing Projectile") && !TimeStop.Instance.IsFrozen)
        {
            parrySFX.Play();
        }
    }

    // Unregisters destroyed projectiles from the active projectile list
    public void UnregisterProjectile(Rigidbody projectile)
    {
        activeProjectiles.Remove(projectile);
    }

    // Gets a list of all the active normal projectiles above a specified height
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

    // Gets a random projectile from the list of active normal projectiles
    public GameObject GetRandomNormalProjectile()
    {
        List<GameObject> normalProjectiles = GetNormalProjectileList(13f);

        if (normalProjectiles.Count == 0)
        {
            return null;
        }

        return normalProjectiles[Random.Range(0, normalProjectiles.Count)];
    }

    // Blinks or unblinks every active normal projectile
    public void ToggleNormalBlink(bool shouldBlink)
    {
        List<GameObject> normalProjectiles = GetNormalProjectileList(0f);

        foreach (GameObject normalProj in normalProjectiles)
        {
            normalProj.GetComponent<NormalProjectile>().UpdateBlinking(shouldBlink);
        }
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

    // Clears all active projectiles in the scene
    public void DestroyAllProjectiles()
    {
        foreach (var p in activeProjectiles)
        {
            Destroy(p.gameObject);
        }

        activeProjectiles.Clear();
    }
}
