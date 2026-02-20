using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    // Upgrade manager instance
    public static UpgradeManager Instance;

    // Hash set of all purchased upgrades
    private HashSet<UpgradeType> boughtUpgrades = new();

    // Activation booleans for all possible upgrades
    public bool AutoCannonBought { get; private set; }
    public bool TimeStopBought { get; private set; }
    public bool PreChargeBought { get; private set; }
    public bool TowerBoostBought { get; private set; }
    public bool MultiChargeBought { get; private set; }

    private void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }
    }

    // Returns whether a specified upgrade has been purchased 
    public bool IsBought(UpgradeType type)
    {
        return boughtUpgrades.Contains(type);
    }

    // Purchases a specified upgrade
    public void BuyUpgrade(UpgradeType type)
    {
        boughtUpgrades.Add(type);
    }
}
