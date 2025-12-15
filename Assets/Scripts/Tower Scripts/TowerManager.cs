using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    private List<GameObject> activeTowers = new List<GameObject>();
    private List<int> splitterCosts = new List<int> {5, 5, 6, 8, 8, 10};
    private int activeSplitterCount = 0;

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

    // Registers new towers in a list of towers
    public void RegisterTower(GameObject tower)
    {
        if (!activeTowers.Contains(tower))
        {
            activeTowers.Add(tower);
            activeSplitterCount++;
        }
    }

    // Unregisters sold towers
    public void UnregisterTower(GameObject tower)
    {
        activeTowers.Remove(tower);
        activeSplitterCount--;
    }

    public int GetTowerCount()
    {
        return activeSplitterCount;
    }

    public int GetSplitterCost()
    {
        int currentCost = 0;
        if (activeSplitterCount == 0)
        {
            currentCost = 5;
        }
        else if (activeSplitterCount >= splitterCosts.Count)
        {
            currentCost = 10;
        }
        else 
        {
            currentCost = splitterCosts[activeSplitterCount];
        }
        
        Debug.Log("Current Tower Cost: " + currentCost);
        Debug.Log("Current Tower Count: " + activeSplitterCount);
        return currentCost;
    }

}
