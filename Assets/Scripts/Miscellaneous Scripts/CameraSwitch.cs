using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance;
    [SerializeField] private List<Camera> cameras;
    [SerializeField] private Camera transitionCam;
    [SerializeField] private Camera overlayCam;
    private int previousActiveCamNum;
    
    public bool IsCameraMoving { get; private set; }
    public static int ActiveCam { get; private set; }
    public static Camera CurrentCamera { get; private set; }

    // Avoids duplicates of this object
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ActiveCam = 1;
        SetActiveCamera(ActiveCam);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsCameraMoving = false; 
        SyncOverlayCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && !IsCameraMoving)
        {
            if (Tutorial.IsTutorialActive)
            {
                return;
            }

            StartCoroutine(SwitchActiveCam());
        }
    }

    private IEnumerator SwitchActiveCam()
    {
        int previousCam = ActiveCam;
        ActiveCam++;

        if (ActiveCam == cameras.Count)
        {
            ActiveCam = 1;
        }

        Time.timeScale = 0.1f;
        yield return StartCoroutine(MoveCamera(previousCam, ActiveCam, 2f));
        SyncOverlayCamera();

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    private void SetActiveCamera(int camNum)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(camNum == i);
        }

        ActiveCam = camNum;
        CurrentCamera = cameras[camNum];
    }

    private void SyncOverlayCamera()
    {
        overlayCam.transform.position = CurrentCamera.transform.position;
        overlayCam.transform.rotation = CurrentCamera.transform.rotation;
        overlayCam.fieldOfView = CurrentCamera.fieldOfView;
    }

    private IEnumerator MoveCamera(int fromCam, int toCam, float cameraMoveTime)
    {
        IsCameraMoving = true;
        overlayCam.enabled = false;

        Camera from = cameras[fromCam];
        Camera to = cameras[toCam];

        Vector3 fromCamPos = from.transform.position;
        Vector3 toCamPos = to.transform.position;

        Quaternion fromCamRot = from.transform.rotation;
        Quaternion toCamRot = to.transform.rotation;

        float fromFOV = from.fieldOfView;
        float toFOV = to.fieldOfView;

        transitionCam.transform.position = fromCamPos;
        transitionCam.transform.rotation = fromCamRot;
        transitionCam.fieldOfView = fromFOV;
        transitionCam.gameObject.SetActive(true);

        from.gameObject.SetActive(false);
        
        float elapsedDelay = 0f;

        while (elapsedDelay < cameraMoveTime)
        {
            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / cameraMoveTime;
            transitionCam.transform.position = Vector3.Lerp(fromCamPos, toCamPos, t);
            transitionCam.transform.rotation = Quaternion.Slerp(fromCamRot, toCamRot, t);
            transitionCam.fieldOfView = Mathf.Lerp(fromFOV, toFOV, t);

            yield return null;
        }

        transitionCam.transform.position = toCamPos;
        transitionCam.transform.rotation = toCamRot;
        transitionCam.fieldOfView = toFOV;

        yield return null;

        SetActiveCamera(toCam);
        transitionCam.gameObject.SetActive(false);

        IsCameraMoving = false;
        overlayCam.enabled = true;
    }

    private IEnumerator MoveToTutorialCamera()
    {
        Tutorial.Instance.UpdateScreenUI(false);
        previousActiveCamNum = ActiveCam;
        
        yield return StartCoroutine(MoveCamera(ActiveCam, 0, 3f));

        SyncOverlayCamera();
        
        Tutorial.Instance.UpdateTutorialUI(true);
    }

    private IEnumerator MoveAwayFromTutorialCamera()
    {
        SyncOverlayCamera();
        Tutorial.Instance.UpdateTutorialUI(false);

        yield return StartCoroutine(MoveCamera(0, previousActiveCamNum, 3f));

        Tutorial.Instance.UpdateImage();
        Tutorial.Instance.UpdateScreenUI(true);
    }

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
