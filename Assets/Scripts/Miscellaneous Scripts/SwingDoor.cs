using UnityEngine;
using System.Collections;

public class SwingWindow : MonoBehaviour
{
    // Window opened boolean
    private bool windowOpened = false;

    // Window swing fields
    [SerializeField] private float swingDegree;
    private float rotateSpeed = 90f;

    void Update()
    {
        // Opens the auto cannon window when the upgrade is bought
        if (UpgradeManager.Instance.IsBought(UpgradeType.AutoCannon) && !windowOpened)
        {
            windowOpened = true;
            StartCoroutine(OpenWindow());
        }
    }

    // Smoothly swings open the swinging auto cannon window
    private IEnumerator OpenWindow()
    {
        Quaternion target = Quaternion.Euler(-90f, swingDegree, 180f);
        while (Quaternion.Angle(transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, rotateSpeed * Time.deltaTime);
            
            yield return null;
        }
    }
}
