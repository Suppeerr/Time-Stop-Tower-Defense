using UnityEngine;

public enum UpgraderType
{
    AutoCannon,
    TimeStop
}

public enum UpgradeType
{
    AutoCannon,
    TimeStop,
    PreCharge,
    MultiCharge
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeType upgradeType;
    public UpgraderType upgraderType;
    public string text;
    public int moneyCost;
    public int secondsCost;
}

[CreateAssetMenu(fileName = "UpgradeDataContainer", menuName = "Upgrades/DataContainer")]
public class UpgradeDataContainer : ScriptableObject
{
    public UpgradeData[] upgrades;

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