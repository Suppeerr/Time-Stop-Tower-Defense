using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    void Update()
    {
        // Makes UI face the current active camera
        if (CameraSwitcher.Instance.CurrentCamera == null) 
        {
            return;
        }

        Vector3 dir = transform.position - CameraSwitcher.Instance.CurrentCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}