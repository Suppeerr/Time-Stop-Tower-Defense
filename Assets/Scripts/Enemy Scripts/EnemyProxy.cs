using UnityEngine;

// Connects enemy data with enemy instances
public class EnemyProxy : MonoBehaviour
{
    public BaseEnemy enemyData;

    public void Init(BaseEnemy data)
    {
        enemyData = data;
    }
}