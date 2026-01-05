using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    private HashSet<UpgradeType> boughtUpgrades = new();
    public bool AutoCannonBought { get; private set; }
    public bool TimeStopBought { get; private set; }
    public bool PreChargeBought { get; private set; }
    public bool TowerBoostBought { get; private set; }
    public bool MultiChargeBought { get; private set; }

    // Avoids duplicates of this object
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsBought(UpgradeType type)
    {
        return boughtUpgrades.Contains(type);
    }

    public void BuyUpgrade(UpgradeType type)
    {
        boughtUpgrades.Add(type);
    }
}
