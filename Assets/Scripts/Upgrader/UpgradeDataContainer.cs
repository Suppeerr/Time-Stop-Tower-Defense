using UnityEngine;

// Stores the types of upgraders
public enum UpgraderType
{
    AutoCannon,
    TimeStop
}

// Stores the types of upgrades
public enum UpgradeType
{
    AutoCannon,
    TimeStop,
    PreCharge,
    MultiCharge
}

// Stores data for each upgrade type
[System.Serializable]
public class UpgradeData
{
    public UpgradeType upgradeType;
    public UpgraderType upgraderType;
    public string text;
    public int moneyCost;
    public int secondsCost;
}

// Defines the upgrade data container
[CreateAssetMenu(fileName = "UpgradeDataContainer", menuName = "Upgrades/DataContainer")]
public class UpgradeDataContainer : ScriptableObject
{
    // List of data for each upgrade
    public UpgradeData[] upgrades;

    // Gets the data for a specified upgrade type
    public UpgradeData GetData(UpgradeType type)
    {
        foreach (var d in upgrades)
        {
            if (d.upgradeType == type) 
            {
                return d;
            }
        }
        
        return null; 
    }
}