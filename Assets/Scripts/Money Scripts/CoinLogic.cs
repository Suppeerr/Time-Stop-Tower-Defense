using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    //private MoneyManagement moneyManager;

    // Assign the MoneyManager via Inspector or at runtime
    public GameObject moneyManagerObject;

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        // Spins coin
        transform.rotation *= Quaternion.Euler(-90 * Time.deltaTime, 0, 0);
    }

    // Call CollectCoin when the coin collides with drone collector
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collector")) 
        {
            moneyManagerObject.GetComponent<MoneyManagement>().CollectCoin();
            Destroy(gameObject);
        }
    }
}

