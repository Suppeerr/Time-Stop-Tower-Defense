using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject thing;

    void OnMouseDown(){
        StartCoroutine(clickOffCheck());
    }

    private IEnumerator clickOffCheck(){
        bool curr = thing.activeSelf;
        yield return new WaitForSeconds(0.1f);
        thing.SetActive(!curr);
    }
}
