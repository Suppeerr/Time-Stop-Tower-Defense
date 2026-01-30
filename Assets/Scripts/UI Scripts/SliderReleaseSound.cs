using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderReleaseSound : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private Slider slider;
    private float oldSliderValue;

    void Start()
    {
        // Gets the slider component of this game object
        slider = this.GetComponent<Slider>();
    }

    // When the slider starts being dragged, store the old slider value
    public void OnBeginDrag(PointerEventData eventData)
    {
        oldSliderValue = slider.value;
    }

    // When the slider stops being dragged, play a corresponding click sound
    public void OnEndDrag(PointerEventData eventData)
    {
        float currentSliderValue = slider.value;

        if (currentSliderValue < oldSliderValue)
        {
            UISoundManager.Instance.PlayClickSound(true);
        }
        else
        {
            UISoundManager.Instance.PlayClickSound(false);
        }
    }
}