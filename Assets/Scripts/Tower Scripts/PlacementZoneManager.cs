using UnityEngine;

public class PlacementZoneManager : MonoBehaviour
{
    // Placement zone manager instance
    public static PlacementZoneManager Instance;

    // Placement zone visual
    [SerializeField] private GameObject zoneVisual;

    void Awake()
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

    // Shows the placement zone indicator
    public void ShowZone()
    {
        if (zoneVisual != null)
        {
            zoneVisual.SetActive(true);
        }
    }

    // Hides the placement zone indicator
    public void HideZone()
    {
        if (zoneVisual != null)
        {
            zoneVisual.SetActive(false);
        }    
    }
}
