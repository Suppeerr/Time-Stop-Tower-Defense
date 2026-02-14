using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NewMonoBehaviourScript : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private GameObject thing;

    private bool OffDetect = false;
/*
    void Awake()
    {
        //Debug.Log("TEST\n");
        EventSystem.current.SetSelectedGameObject(thing);
    }

*/

    void Awake()
    {

    }

    void OnMouseDown()
    {
        if (TimeStop.Instance.IsFrozen)
        {
            return;
        }

        Debug.Log("Click Registered");
        if (thing.activeSelf){
            //EventSystem.current.SetSelectedGameObject(thing);
            thing.SetActive(!thing.activeSelf);
            OffDetect = false;
        } else {
            //EventSystem.current.SetSelectedGameObject(thing);
            thing.SetActive(!thing.activeSelf);
            OffDetect = true;
            //EventSystem.current.SetSelectedGameObject(thing);
        }
        //StartCoroutine(clickOffCheck());
    }
/*
    private IEnumerator clickOffCheck(){
        bool curr = thing.activeSelf;
        yield return new WaitForSeconds(0.1f);
        thing.SetActive(!curr);
    }
*/
    public void OnDeselect(BaseEventData eventData)
    {
        //if (thing.activeSelf)
            //return;
/*
        Debug.Log("Value of Counter: " + counter);
        if (counter == 0){
            counter++;
            return;
        } else {
            counter--;
        }
*/
        Debug.Log("Clicked Outside UI");
    }

    private void HideIfClickedOutside(GameObject panel) {
        if (Input.GetMouseButtonDown(0) && panel.activeSelf && 
            !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(), 
                Input.mousePosition, 
                Camera.main) && OffDetect) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                if (hit.collider.gameObject != this.gameObject){
                    Debug.Log("Hiding UI");
                    OffDetect = false;
                    panel.SetActive(false);
                }
        }
    }

    void Update(){
        HideIfClickedOutside(thing);
    }
}
