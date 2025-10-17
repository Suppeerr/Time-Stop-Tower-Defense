using UnityEngine;

[System.Serializable]
public class ProjectileStats
{
    public ProjectileType type;   // links stats to a type
    public float damage;
    public float speed;
    public float aoeRadius;       // 0 if single target
    // add more fields as needed
}

[CreateAssetMenu(fileName = "ProjectileStatsContainer", menuName = "Projectiles/StatsContainer")]
public class ProjectileStatsContainer : ScriptableObject
{
    public ProjectileStats[] stats;

    public ProjectileStats GetStats(ProjectileType type)
    {
        foreach (var s in stats)
        {
            if (s.type == type) return s;
        }
        return null; // or a default
    }
}