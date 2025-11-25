using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    // Initiates camera to the main camera
    void Start()
    {
    }
    
    // Updates to make UI face camera
    void Update()
    {
        if (CameraSwitch.CurrentCamera == null) 
        {
            return;
        }

        Vector3 dir = transform.position - CameraSwitch.CurrentCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}