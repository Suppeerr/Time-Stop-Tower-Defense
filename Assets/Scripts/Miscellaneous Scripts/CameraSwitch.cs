using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance;
    [SerializeField] private Camera mainCam1;
    [SerializeField] private Camera mainCam2;
    [SerializeField] private Camera mainCam3;
    [SerializeField] private Camera tutorialCam;

    [SerializeField] private Camera overlayCam;

    private float cameraMoveTime = 3f;
    private Vector3 currentCameraPosition;
    private Vector3 tutorialCameraPosition;
    private Quaternion currentCameraRotation;
    private Quaternion tutorialCameraRotation;
    private int previousActiveCamera;

    public bool CameraMoving { get; private set; }

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraMoving = false; 

        tutorialCameraPosition = tutorialCam.transform.position;
        tutorialCameraRotation = tutorialCam.transform.rotation;

        ActiveCam = 1;
        SetActiveCamera(ActiveCam);
        SyncOverlayCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && !CameraMoving)
        {
            if (Tutorial.IsTutorialActive)
            {
                return;
            }

            SwitchActiveCam();

            SetActiveCamera(ActiveCam);
            SyncOverlayCamera();
        }
    }

    private void SwitchActiveCam()
    {
        if (ActiveCam == 1)
        {
            ActiveCam = 2;
            CurrentCamera = mainCam2;
        }
        else if (ActiveCam == 2)
        {
            ActiveCam = 3;
            CurrentCamera = mainCam3;
        }
        else
        {
            ActiveCam = 1;
            CurrentCamera = mainCam1;
        }
    }

    private int GetActiveCam()
    {
        if (CurrentCamera == mainCam1)
        {
            return 1;
        }
        else if (CurrentCamera == mainCam2)
        {
            return 2;
        }
        else 
        {
            return 3;
        }
    }

    private void SetActiveCamera(int camNum)
    {
        tutorialCam.gameObject.SetActive(camNum == 0);
        mainCam1.gameObject.SetActive(camNum == 1);
        mainCam2.gameObject.SetActive(camNum == 2);
        mainCam3.gameObject.SetActive(camNum == 3);

        if (camNum == 0)
        {
            ActiveCam = 0;
            CurrentCamera = tutorialCam;
        }
        else if (camNum == 1)
        {
            ActiveCam = 1;
            CurrentCamera = mainCam1;
        }
        else if (camNum == 2)
        {
            ActiveCam = 2;
            CurrentCamera = mainCam2;
        }
        else if (camNum == 3)
        {
            ActiveCam = 3;
            CurrentCamera = mainCam3;
        }
    }

    private void SyncOverlayCamera()
    {
        overlayCam.transform.position = CurrentCamera.transform.position;
        overlayCam.transform.rotation = CurrentCamera.transform.rotation;
        overlayCam.fieldOfView = CurrentCamera.fieldOfView;
    }

    private IEnumerator MoveToTutorialCamera()
    {
        float elapsedDelay = 0f;
        previousActiveCamera = GetActiveCam();
        CameraMoving = true;
        overlayCam.gameObject.SetActive(false);
        currentCameraPosition = CurrentCamera.transform.position;
        currentCameraRotation = CurrentCamera.transform.rotation;
        Tutorial.Instance.UpdateScreenUI(false);

        while (elapsedDelay < cameraMoveTime)
        {
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / cameraMoveTime;
            CurrentCamera.transform.position = Vector3.Lerp(currentCameraPosition, tutorialCameraPosition, t);
            CurrentCamera.transform.rotation = Quaternion.Slerp(currentCameraRotation, tutorialCameraRotation, t);

            yield return null;
        }

        CurrentCamera.transform.position = tutorialCameraPosition;
        CurrentCamera.transform.rotation = tutorialCameraRotation;

        overlayCam.gameObject.SetActive(true);
        tutorialCam.gameObject.SetActive(true);
        CameraMoving = false;

        SetActiveCamera(0);
        SyncOverlayCamera();
        
        Tutorial.Instance.UpdateTutorialUI(true);
        yield return null;

    }

    private IEnumerator MoveAwayFromTutorialCamera()
    {
        float elapsedDelay = 0f;
        CameraMoving = true;
        overlayCam.gameObject.SetActive(false);
        SetActiveCamera(previousActiveCamera);
        SyncOverlayCamera();
        Tutorial.Instance.UpdateTutorialUI(false);

        while (elapsedDelay < cameraMoveTime)
        {
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / cameraMoveTime;
            CurrentCamera.transform.position = Vector3.Lerp(tutorialCameraPosition, currentCameraPosition, t);
            CurrentCamera.transform.rotation = Quaternion.Slerp(tutorialCameraRotation, currentCameraRotation, t);

            yield return null;
        }
        CurrentCamera.transform.position = currentCameraPosition;
        CurrentCamera.transform.rotation = currentCameraRotation;

        overlayCam.gameObject.SetActive(true);
        CameraMoving = false;
        Tutorial.Instance.UpdateImage();
        Tutorial.Instance.UpdateScreenUI(true);
        
        yield return null;
    }

    public void ToggleTutorial(bool active)
    {
        if (active)
        {
            ActiveCam = 0;
            StartCoroutine(MoveToTutorialCamera());
        }
        else
        {
            ActiveCam = 1;
            StartCoroutine(MoveAwayFromTutorialCamera());
        }
    }
}
