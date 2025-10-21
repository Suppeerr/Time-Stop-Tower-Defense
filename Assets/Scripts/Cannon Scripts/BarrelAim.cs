using UnityEngine;
using System.Collections;

public class BarrelAim : MonoBehaviour
{
    public Transform shootPoint;            // Shoot location
    private float rotationSpeed = 1f;       // Cannon rotate speed
    private Vector3 targetDirection;        // Direction of target

    // Aims at target whenever called
    public IEnumerator AimAtTarget(GameObject target)
    {
        if (target == null)
        {
            yield break;
        }
        // Calculates desired rotation
        Quaternion targetRot = Quaternion.LookRotation(target.transform.position + Vector3.up * 5f - shootPoint.position);

        // Rotate to right angle
        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        yield return null;
    }
}
