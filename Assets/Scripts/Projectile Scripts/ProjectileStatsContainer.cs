using UnityEngine;
using System.Collections.Generic;

// Stores a list of all projectile types
public enum ProjectileType
{
    CannonBall,
    PrimaryHoming,
    PrimaryNormal,
    SecondaryHoming,
    SecondaryNormal,
    Coin
}

// Stores stats for each projectile type
[System.Serializable]
public class ProjectileStats
{
    public ProjectileType type;   
    public GameObject projectilePrefab;
    public int damage;
    public float speed;
    public float aoeRadius;       // *0 if single target
}

// Defines the projectile stats container
[CreateAssetMenu(fileName = "ProjectileStatsContainer", menuName = "Projectiles/StatsContainer")]
public class ProjectileStatsContainer : ScriptableObject
{
    // List of stats for each projectile
    public ProjectileStats[] stats;

    // Gets the stats of a specified projectile type
    public ProjectileStats GetStats(ProjectileType type)
    {
        foreach (var s in stats)
        {
            if (s.type == type) 
            {
                return s;
            }
        }
        return null;
    }

    // Returns a list of all projectile types
    public List<ProjectileType> GetProjectileTypes()
    {
        List<ProjectileType> projTypes = new List<ProjectileType>();

        foreach (var s in stats)
        {
            projTypes.Add(s.type);
        }

        return projTypes;
    }
}