using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    public Camera mainCam1;
    public Camera mainCam2;
    public Camera mainCam3;

    public Camera overlayCam;

    public static int ActiveCam { get; private set; }

    public static Camera CurrentCamera { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActiveCam = 1;
        SetActiveCamera(ActiveCam);
        SyncOverlayCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (ActiveCam == 1)
            {
                ActiveCam = 2;
            }
            else if (ActiveCam == 2)
            {
                ActiveCam = 3;
            }
            else
            {
                ActiveCam = 1;
            }

            SetActiveCamera(ActiveCam);
            SyncOverlayCamera();
        }
    }

    private void SetActiveCamera(int camNum)
    {
        mainCam1.gameObject.SetActive(camNum == 1);
        mainCam2.gameObject.SetActive(camNum == 2);
        mainCam3.gameObject.SetActive(camNum == 3);
    }

    private void SyncOverlayCamera()
    {
        if (ActiveCam == 1)
        {
            CurrentCamera = mainCam1;
        }
        else if (ActiveCam == 2)
        {
            CurrentCamera = mainCam2;
        }
        else
        {
            CurrentCamera = mainCam3;
        }

        overlayCam.transform.position = CurrentCamera.transform.position;
        overlayCam.transform.rotation = CurrentCamera.transform.rotation;
        overlayCam.fieldOfView = CurrentCamera.fieldOfView;
    }
}
