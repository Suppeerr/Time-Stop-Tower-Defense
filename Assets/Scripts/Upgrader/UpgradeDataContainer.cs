using UnityEngine;

public enum UpgraderType
{
    AutoCannon,
    TimeStop
}

public enum UpgradeType
{
    AutoCannon,
    PreCharge,
    MultiCharge
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeType upgradeType;
    public UpgraderType upgraderType;
    public int upgradeNumber;
    public string text;
    public int moneyCost;
    public int secondsCost;
}

[CreateAssetMenu(fileName = "UpgradeDataContainer", menuName = "Upgrades/DataContainer")]
public class UpgradeDataContainer : ScriptableObject
{
    public UpgradeData[] data;

    public UpgradeData GetData(UpgradeType type)
    {
        foreach (var d in data)
        {
            if (d.upgradeType == type) 
            {
                return d;
            }
        }
        return null; 
    }
}