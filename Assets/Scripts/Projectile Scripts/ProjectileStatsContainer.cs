using UnityEngine;

public enum ProjectileType
{
    CannonBall,
    PrimaryHoming,
    PrimaryNormal,
    SecondaryHoming,
    SecondaryNormal,
    Coin
}

[System.Serializable]
public class ProjectileStats
{
    public ProjectileType type;   // links stats to a type
    public int damage;
    public float speed;
    public float aoeRadius;       // 0 if single target
}

[CreateAssetMenu(fileName = "ProjectileStatsContainer", menuName = "Projectiles/StatsContainer")]
public class ProjectileStatsContainer : ScriptableObject
{
    public ProjectileStats[] stats;

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
}