using UnityEngine;

public class EnemyProxy : MonoBehaviour
{
    public BaseEnemy enemyData;

    public void Init(BaseEnemy data)
    {
        enemyData = data;
    }
}