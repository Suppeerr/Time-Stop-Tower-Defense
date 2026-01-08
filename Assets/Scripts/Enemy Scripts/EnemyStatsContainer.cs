using UnityEngine;

// Defines enemy types
public enum EnemyType
{
    NormalBandit,
    SpeedyBandit,
    WaveBandit
}

// Defines the stats for each enemy type
[System.Serializable]
public class EnemyStats
{
    public EnemyType type;   
    public int hp;
    public float spd;
    public int def;       
}

// Defines the enemy stats container 
[CreateAssetMenu(fileName = "EnemyStatsContainer", menuName = "Enemies/StatsContainer")]
public class EnemyStatsContainer : ScriptableObject
{
    // List of stats for each enemy
    public EnemyStats[] stats;

    // Gets the stats of a specified enemy type
    public EnemyStats GetStats(EnemyType type)
    {
        foreach (var s in stats)
        {
            if (s.type == type) return s;
        }
        return null; 
    }
}