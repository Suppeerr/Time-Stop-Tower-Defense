using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    // Scripts and transform
    private MoneyManager moneyManagerScript;
    private Drone droneScript;
    private Transform launcher;

    // Coin fields
    private float floatSpeed = 10f;
    private bool isBeingVacuumed = false;
    [SerializeField] private bool isProjectile;

    void Awake()
    {
        // Initialize fields
        droneScript ??= FindFirstObjectByType<Drone>();
        moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();
        launcher = GameObject.Find("Coin Launcher")?.transform;
    }

    void Update()
    {
        if (ProjectileManager.Instance.IsFrozen)
        {
            return;
        }

        // Floats the coin up if it touches the vacuum collider
        if (isBeingVacuumed)
        {
            FloatIntoDrone();
        }

        // Spins coin
        transform.rotation *= Quaternion.Euler(-90 * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tells drone that its vacuum is active when the coin touches the collider
        if (other.name == "Coin Vacuum" && !isBeingVacuumed && !isProjectile)
        {
            isBeingVacuumed = true;
            droneScript.ToggleCoinCollect(true);
            return;
        }

        // Collects the coin when it collides with drone collector
        if (other.name == "Coin Collector" && !isProjectile)
        {
            droneScript.ToggleCoinCollect(false);
            droneScript.FindClosestCoin();
            Destroy(gameObject);
            CoinSpawner.Instance.SpawnCoin(launcherPos: launcher.position);
            return;
        }

        // Increases money when the coin projectile hits the safe collector
        if (other.name == "Safe Collector" && isProjectile)
        {
            moneyManagerScript.UpdateMoney();
            Destroy(gameObject);
            return;
        }
    }

    // Vacuums the coin into the drone
    private void FloatIntoDrone()
    {
        gameObject.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}

