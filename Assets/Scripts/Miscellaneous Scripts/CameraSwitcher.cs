using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    // Camera switcher instance
    public static CameraSwitcher Instance;

    // List of cameras
    [SerializeField] private List<Camera> cameras;

    // Specific camera fields
    [SerializeField] private Camera transitionCam;
    [SerializeField] private Camera overlayCam;

    // Saves the previously active camera when a camera switches
    private int previousActiveCamNum;
    
    // Camera moving boolean
    public bool IsCameraMoving { get; private set; }

    // Camera number and camera fields
    public int ActiveCam { get; private set; }
    public Camera CurrentCamera { get; private set; }

    private void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }

        // Sets camera to the default
        ActiveCam = 1;
        SetActiveCamera(ActiveCam);
    }

    void Start()
    {
        // Initializes moving and overlay camera fields
        IsCameraMoving = false; 
        SyncOverlayCamera();
    }

    void Update()
    {
        // Switches camera when c key pressed
        if (Keyboard.current.cKey.wasPressedThisFrame && !IsCameraMoving)
        {
            if (TutorialManager.Instance.IsTutorialActive)
            {
                return;
            }

            StartCoroutine(SwitchActiveCam());
        }
    }

    // Switches the active camera to the next camera in line
    private IEnumerator SwitchActiveCam()
    {
        previousActiveCamNum = ActiveCam;
        ActiveCam++;

        if (ActiveCam == cameras.Count)
        {
            ActiveCam = 1;
        }

        Time.timeScale = 0.1f;
        yield return StartCoroutine(MoveCamera(cameras[previousActiveCamNum], cameras[ActiveCam], 1.5f));
        SyncOverlayCamera();

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    // Sets the active camera to a specified camera via camera number
    private void SetActiveCamera(int camNum)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(camNum == i);
        }

        ActiveCam = camNum;
        CurrentCamera = cameras[camNum];
    }

    // Sets the active camera to a specified camera
    private void SetActiveCamera(Camera cam)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(cam == cameras[i]);

            if (cam == cameras[i])
            {
                ActiveCam = i;
                CurrentCamera = cam;
            }
        }
    }

    // Syncs the overlay camera with the current active camera
    private void SyncOverlayCamera()
    {
        overlayCam.transform.position = CurrentCamera.transform.position;
        overlayCam.transform.rotation = CurrentCamera.transform.rotation;
        overlayCam.fieldOfView = CurrentCamera.fieldOfView;
    }

    // Smoothly moves a camera to another camera
    private IEnumerator MoveCamera(Camera from, Camera to, float cameraMoveTime)
    {
        IsCameraMoving = true;
        overlayCam.enabled = false;

        Vector3 fromPos = from.transform.position;
        Vector3 toPos = to.transform.position;

        Quaternion fromRot = from.transform.rotation;
        Quaternion toRot = to.transform.rotation;

        float fromFOV = from.fieldOfView;
        float toFOV = to.fieldOfView;

        ApplyCameraState(transitionCam, fromPos, fromRot, fromFOV);
        transitionCam.gameObject.SetActive(true);

        from.gameObject.SetActive(false);
        
        float elapsedDelay = 0f;

        while (elapsedDelay < cameraMoveTime)
        {
            while (SettingsMenuOpener.Instance.MenuOpened)
            {
                yield return null;
            }

            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / cameraMoveTime;

            ApplyCameraState(
                transitionCam, 
                Vector3.Lerp(fromPos, toPos, t), 
                Quaternion.Slerp(fromRot, toRot, t), 
                Mathf.Lerp(fromFOV, toFOV, t));

            yield return null;
        }

        ApplyCameraState(transitionCam, toPos, toRot, toFOV);

        SetActiveCamera(to);
        transitionCam.gameObject.SetActive(false);

        IsCameraMoving = false;
        overlayCam.enabled = true;
    }

    // Applies a specified position, rotation, and FOV to a camera
    private void ApplyCameraState(Camera cam, Vector3 pos, Quaternion rot, float fov)
    {
        cam.transform.position = pos;
        cam.transform.rotation = rot;
        cam.fieldOfView = fov;
    }

    // Moves the camera to the tutorial camera
    private IEnumerator MoveToTutorialCamera()
    {
        TutorialManager.Instance.UpdateScreenUI(false);
        previousActiveCamNum = ActiveCam;
        
        yield return StartCoroutine(MoveCamera(cameras[ActiveCam], cameras[0], 2.5f));

        SyncOverlayCamera();
        
        TutorialManager.Instance.UpdateTutorialUI(true);
    }

    // Moves the camera away from the tutorial camera
    private IEnumerator MoveAwayFromTutorialCamera()
    {
        SyncOverlayCamera();
        TutorialManager.Instance.UpdateTutorialUI(false);

        yield return StartCoroutine(MoveCamera(cameras[0], cameras[previousActiveCamNum], 2.5f));

        TutorialManager.Instance.UpdateImage();
        TutorialManager.Instance.UpdateScreenUI(true);
    }

    // Switches camera according to whether the tutorial is active
    public void ToggleTutorial(bool active)
    {
        if (active)
        {
            StartCoroutine(MoveToTutorialCamera());
        }
        else
        {
            StartCoroutine(MoveAwayFromTutorialCamera());
        }
    }
}
