// using UnityEngine;

// public class CoinSpawnTest : MonoBehaviour
// {
//     private float timer = 0.0f;
//     [SerializeField] private GameObject coin;
//     [SerializeField] private readonly float spawnInterval = 5.0f;

//     [SerializeField] private GameObject moneyManagerObject;

//     private GameObject clone;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Time.time >= timer)
//         {
//             timer += spawnInterval;
//             SpawnCoin();
//         }
//     }

//     public void SpawnCoin()
//     {
//         clone = Instantiate(coin, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
//         clone.transform.rotation *= Quaternion.Euler(0, 0, 90);
//         var coinLogic = clone.GetComponent<CoinLogic>();
//         coinLogic.Init(moneyManagerObject);
//     }
// }

using UnityEngine;

public class CoinSpawnTest : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private MoneyManagement moneyManagerObject;

    private float timer = 0f;

    void Update()
    {
        if (Time.time >= timer)
        {
            timer += spawnInterval;
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        GameObject clone = Instantiate(
            coinPrefab,
            new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)),
            Quaternion.Euler(0, 0, 90)
        );

        CoinLogic coinLogic = clone.GetComponent<CoinLogic>();
        if (coinLogic != null)
        {
            coinLogic.moneyManagerObject = moneyManagerObject;
        }
    }
}
