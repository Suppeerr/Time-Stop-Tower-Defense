using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    // Tower manager instance
    public static TowerManager Instance;

    // List of all active towers
    private List<GameObject> activeTowers = new List<GameObject>();

    // List of splitter costs 
    private List<int> regularSplitterCosts = new List<int> {5, 5, 5, 6, 8, 8, 10};
    private List<int> hardSplitterCosts = new List<int> {5, 5, 6, 8, 12, 12, 15};

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

        if (GameInstance.LevelDifficulty != GameInstance.DifficultyType.Hard)
        {
            if (activeSplitterCount >= regularSplitterCosts.Count)
            {
                currentCost = 10;
            }
            else 
            {
                currentCost = regularSplitterCosts[activeSplitterCount];
            }
        }
        else
        {
            if (activeSplitterCount >= hardSplitterCosts.Count)
            {
                currentCost = 15;
            }
            else 
            {
                currentCost = hardSplitterCosts[activeSplitterCount];
            }
        }
        
        
        return currentCost;
    }

}
