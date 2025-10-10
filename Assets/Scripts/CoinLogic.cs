using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    MoneyManagement moneyManager;
    public GameObject MoneyManagerObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneyManager = MoneyManagerObject.GetComponent<MoneyManagement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        moneyManager.CollectCoin();
    }
}
