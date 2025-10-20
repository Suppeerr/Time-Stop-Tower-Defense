using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public EnemyType type;   
    public int hp;
    public float spd;
    public int def;       
}

[CreateAssetMenu(fileName = "EnemyStatsContainer", menuName = "Enemies/StatsContainer")]
public class EnemyStatsContainer : ScriptableObject
{
    public EnemyStats[] stats;

    public EnemyStats GetStats(EnemyType type)
    {
        foreach (var s in stats)
        {
            if (s.type == type) return s;
        }
        return null; 
    }
}