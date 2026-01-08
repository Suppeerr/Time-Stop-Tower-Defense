using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // Coin spawner instance
    public static CoinSpawner Instance;

    // Coin prefabs
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject homingCoinPrefab;

    // Drone script
    [SerializeField] private Drone droneScript;

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
    }

    // Spawns a normal coin when enemies die or a projectile coin from the drone cannon
    public void SpawnCoin(Vector3? enemyPos = null, Vector3? launcherPos = null)
    {
        Vector3 spawnPos = Vector3.zero;

        if (enemyPos.HasValue)
        {
            // Spawns normal coin
            spawnPos = enemyPos.Value;
            GameObject droppedCoin = Instantiate(
            coinPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, 90)
            );

            droneScript.FindClosestCoin();
        }
        else if (launcherPos.HasValue)
        {
            // Spawns projectile coin
            spawnPos = launcherPos.Value;
            GameObject clone = Instantiate(
            homingCoinPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, 90)
            );
        } 
    }
}
