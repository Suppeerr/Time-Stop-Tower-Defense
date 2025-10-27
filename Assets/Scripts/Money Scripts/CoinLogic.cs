// using UnityEngine;

// public class CoinLogic : MonoBehaviour
// {
//     [HideInInspector] public GameObject MoneyManagerObject;
//     private MoneyManagement moneyManager;

//     public void Init(GameObject manager)
//     {
//         MoneyManagerObject = manager;
//         moneyManager = MoneyManagerObject.GetComponent<MoneyManagement>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // Spins coin
//         transform.rotation *= Quaternion.Euler(-90 * Time.deltaTime, 0, 0);
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Collector")) 
//         {
//             moneyManager?.CollectCoin();
//             Destroy(gameObject);
//         }
//     }
// }

using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    //private MoneyManagement moneyManager;

    // Assign the MoneyManager via Inspector or at runtime
    public GameObject moneyManagerObject;

    public void Init()
    {
        /*
        if (moneyManagerObject != null)
            moneyManager = moneyManagerObject;
        else
            Debug.LogError("MoneyManager not assigned on " + gameObject.name);
            */
    }

    // Update is called once per frame
    void Update()
    {
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

