using UnityEngine;

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private GameObject zoneVisual;

    private void OnEnable()
    {
        Draggable.OnDragStart += ShowZone;
        Draggable.OnDragEnd += HideZone;
    }

    private void OnDisable()
    {
        Draggable.OnDragStart -= ShowZone;
        Draggable.OnDragEnd -= HideZone;
    }

    private void ShowZone()
    {
        if (zoneVisual != null)
        {
            zoneVisual.SetActive(true);
        }
    }

    private void HideZone()
    {
        if (zoneVisual != null)
        {
            zoneVisual.SetActive(false);
        }    
    }
}
