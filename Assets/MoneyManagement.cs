using UnityEngine;

public class MoneyManagement : MonoBehaviour
{
    public int money;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        money = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CollectCoin()
    {
        money += 1;
    }
}
