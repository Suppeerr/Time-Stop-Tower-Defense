using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    // Tower manager instance
    public static TowerManager Instance;

    // List of all active towers
    private List<GameObject> activeTowers = new List<GameObject>();

    // List of splitter costs 
    private List<int> splitterCosts = new List<int> {5, 5, 5, 6, 8, 8, 10};

    // Count of the number of active splitter towers
    private int activeSplitterCount = 0;

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

    // Registers new towers in a list of towers
    public void RegisterTower(GameObject tower)
    {
        activeTowers.Add(tower);
        activeSplitterCount++;
    }

    // Unregisters removed towers
    public void UnregisterTower(GameObject tower)
    {
        activeTowers.Remove(tower);
        activeSplitterCount--;
    }

    // Returns the number of active towers on the map
    public int GetTowerCount()
    {
        return activeTowers.Count;
    }

    // Returns the cost of the next splitter tower
    public int GetSplitterCost()
    {
        int currentCost = 0;

        if (activeSplitterCount >= splitterCosts.Count)
        {
            currentCost = 10;
        }
        else 
        {
            currentCost = splitterCosts[activeSplitterCount];
        }
        
        return currentCost;
    }

}
