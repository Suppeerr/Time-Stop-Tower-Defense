using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    // Initiates camera to the main camera
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    // Updates to make UI face camera
    void Update()
    {
        transform.LookAt
        (transform.position + mainCamera.transform.rotation * Vector3.forward,
         mainCamera.transform.rotation * Vector3.up);
    }
}