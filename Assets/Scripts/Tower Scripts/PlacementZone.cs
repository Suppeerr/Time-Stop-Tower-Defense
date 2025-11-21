using UnityEngine;

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private GameObject zoneVisual;

    void Start()
    {
        HideZone();
    }

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
        zoneVisual.SetActive(true);
    }

    private void HideZone()
    {
        zoneVisual.SetActive(false);
    }
}
