using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    void Update()
    {
        // Makes UI face the current active camera
        if (CameraSwitch.CurrentCamera == null) 
        {
            return;
        }

        Vector3 dir = transform.position - CameraSwitch.CurrentCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}