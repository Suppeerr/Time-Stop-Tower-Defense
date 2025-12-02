using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOff : MonoBehaviour
{

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


}
