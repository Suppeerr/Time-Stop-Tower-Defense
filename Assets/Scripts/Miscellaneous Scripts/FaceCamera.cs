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
        transform.LookAt
        (transform.position + CameraSwitch.CurrentCamera.transform.rotation * Vector3.forward,
         CameraSwitch.CurrentCamera.transform.rotation * Vector3.up);
    }
}