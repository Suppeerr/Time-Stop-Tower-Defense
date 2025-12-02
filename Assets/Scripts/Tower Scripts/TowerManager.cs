using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    private List<GameObject> activeTowers = new List<GameObject>();

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
        }
    }

    // Unregisters sold towers
    public void UnregisterTower(GameObject tower)
    {
        activeTowers.Remove(tower);
    }

    public int GetTowerCount()
    {
        return activeTowers.Count;
    }

}
