using UnityEngine;

public class CoinSpawnTest : MonoBehaviour
{
    private float timer = 0.0f;
    [SerializeField] private GameObject coin;
    [SerializeField] private float spawnInterval = 5.0f;

    [SerializeField] private GameObject moneyManagerObject;

    private GameObject clone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timer)
        {
            timer += spawnInterval;
            spawnCoin();
        }
    }

    public void spawnCoin()
    {
        clone = Instantiate(coin, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
        clone.transform.rotation *= Quaternion.Euler(0, 0, 90);
        clone.GetComponent<CoinLogic>().MoneyManagerObject = moneyManagerObject;
    }
}
