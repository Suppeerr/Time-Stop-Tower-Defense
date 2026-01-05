using UnityEngine;
using System.Collections;

public class SwingDoor : MonoBehaviour
{
    private bool doorOpened = false;
    [SerializeField] private float swingDegree;
    private float rotateSpeed = 90f;

    void Update()
    {
        if (UpgradeManager.Instance.IsBought(UpgradeType.AutoCannon) && !doorOpened)
        {
            doorOpened = true;
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        Quaternion target = Quaternion.Euler(-90f, swingDegree, 180f);
        while (Quaternion.Angle(transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, rotateSpeed * Time.deltaTime);
            
            yield return null;
        }
    }
}
