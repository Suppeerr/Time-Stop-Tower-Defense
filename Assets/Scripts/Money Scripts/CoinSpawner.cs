using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }
    [SerializeField] private GameObject coinPrefab;
    // [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private GameObject moneyManagerObject;
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

    void Update()
    {
        // if (Time.timeSinceLevelLoad >= timer)
        // {
        //     timer += spawnInterval;
        //     SpawnCoin();
        // }
    }

    public void SpawnCoin(Vector3 enemyPos)
    {
        // GameObject clone = Instantiate(
        //     coinPrefab,
        //     new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)),
        //     Quaternion.Euler(0, 0, 90)
        // );

            GameObject clone = Instantiate(
            coinPrefab,
            enemyPos,
            Quaternion.Euler(0, 0, 90)
            );

        CoinLogic coinLogic = clone.GetComponent<CoinLogic>();
        coinLogic.moneyManagerObject = moneyManagerObject;
        droneScript.changeCoin(droneScript.FindClosestCoin());
    }
}
