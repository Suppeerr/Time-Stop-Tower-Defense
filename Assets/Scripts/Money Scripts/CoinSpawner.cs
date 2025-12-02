using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject homingCoinPrefab;
    // [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Drone droneScript;

    // private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnCoin(bool isCoinLauncher, Vector3? enemyPos = null, Vector3? launcherPos = null)
    {
        Vector3 spawnPos = Vector3.zero;

        if (enemyPos.HasValue)
        {
            spawnPos = enemyPos.Value;
        }
        else if (launcherPos.HasValue)
        {
            spawnPos = launcherPos.Value;
        }

        if (isCoinLauncher)
        {
            GameObject clone = Instantiate(
            homingCoinPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, 90)
            );
        }
        else
        {
            GameObject droppedCoin = Instantiate(
            coinPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, 90)
            );

            droneScript.FindClosestCoin();
        }  
    }
}
