using UnityEngine;

public class ClickOffPlacement : MonoBehaviour
{
   [SerializeField] private GameObject TPUI;

    public void closeTowerPlacement(){
        TPUI.SetActive(false);
    }
}
