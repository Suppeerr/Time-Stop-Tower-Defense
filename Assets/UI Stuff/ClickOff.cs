using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOff : MonoBehaviour
{

    void Update(){
        if (Input.GetMouseButtonDown(0) && this.gameObject.activeSelf == true && !IsPointerOverUIElement()){
            Debug.Log(gameObject.activeSelf);
            Debug.Log("REMOVING UI");
            this.gameObject.SetActive(false);
        }
    }

    // Helper function to check if the pointer is over a UI element
    private bool IsPointerOverUIElement()
    {
        // Check if the current selected object is a UI element
        return EventSystem.current.currentSelectedGameObject != null;
    }


}
