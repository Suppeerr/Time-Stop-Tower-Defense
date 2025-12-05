using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOff : MonoBehaviour
{

[SerializeField] private GameObject TPUI;
[SerializeField] private GameObject WSGameObject;
/*

    void Update()
 {
        //Check for mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                   //Click method
                    Debug.Log("CLICKING");
                    CurrentClickedGameObject(raycastHit.transform.gameObject);
                }
            }
        }
 }

public void CurrentClickedGameObject(GameObject gameObject)
{
    if(gameObject.tag != "Workshop UI")
    {
        Debug.Log(gameObject.tag);
        this.gameObject.SetActive(false);
    }
}

*//*
    void Awake()
    {
        //Debug.Log("TEST\n");
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("Clicked Outside UI");
    }
    */

    private void HideIfClickedOutside(GameObject panel) {
        if (Input.GetMouseButton(0) && panel.activeSelf && 
            !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(), 
                Input.mousePosition, 
                Camera.main)) {
            Debug.Log("Hiding UI");
            panel.SetActive(false);
        }
    }

    void Update(){
        HideIfClickedOutside(TPUI);
    }
}
