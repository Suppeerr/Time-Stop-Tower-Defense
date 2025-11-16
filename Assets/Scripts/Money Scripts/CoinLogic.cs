using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    private MoneyManager moneyManagerScript;
    private Drone droneScript;
    private Transform launcher;


    private float floatSpeed = 0.1f;
    private bool isBeingVacuumed = false;
    [SerializeField] private bool isProjectile;

    void Awake()
    {
        droneScript ??= FindFirstObjectByType<Drone>();
        moneyManagerScript = GameObject.Find("Money Safe")?.GetComponent<MoneyManager>();
        launcher = GameObject.Find("Coin Launcher")?.transform;
        
        if (launcher == null)
        {
            Debug.LogError("Coin Launcher not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        if (isBeingVacuumed)
        {
            FloatIntoDrone();
        }

        // Spins coin
        transform.rotation *= Quaternion.Euler(-90 * Time.deltaTime, 0, 0);
    }

    // Call CollectCoin when the coin collides with drone collector
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Coin Vacuum" && !isBeingVacuumed && !isProjectile)
        {
            isBeingVacuumed = true;
            droneScript.ToggleCoinCollect(true);
        }
        else if (other.name == "Coin Collector" && !isProjectile)
        {
            droneScript.ToggleCoinCollect(false);
            droneScript.FindClosestCoin();
            Destroy(gameObject);
            CoinSpawner.Instance.SpawnCoin(true, launcherPos: launcher.position);
        }
        else if (other.name == "Safe Collector" && isProjectile)
        {
            moneyManagerScript.UpdateMoney();
            Destroy(gameObject);
        }
    }

    // Vacuums the coin into the drone
    private void FloatIntoDrone()
    {
        gameObject.transform.position += Vector3.up * floatSpeed;
    }
}

