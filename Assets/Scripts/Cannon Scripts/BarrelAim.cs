using UnityEngine;
using System.Collections;

public class BarrelAim : MonoBehaviour
{
    public Transform shootPoint;
    private float rotationSpeed = 1f;

    private Vector3 targetDirection;

    public IEnumerator AimAtTarget(GameObject target)
    {
        if (target == null)
        {
            yield break;
        }
        // Calculates desired rotation
        Quaternion targetRot = Quaternion.LookRotation((target.transform.position + Vector3.up * 1.0f) - shootPoint.position);

        // Rotate to right angle
        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        yield return null;
    }
}
