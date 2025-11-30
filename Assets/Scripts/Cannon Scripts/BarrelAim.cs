using UnityEngine;
using System.Collections;

public class BarrelAim : MonoBehaviour
{
    public Transform shootPoint;            // Shoot location
    private Vector3 targetDirection;        // Direction of target

    // Aims at target whenever called
    public void AimAtTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }
        // Calculates desired rotation
        Vector3 direction = (target.position + Vector3.up * 5f) - shootPoint.position;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(direction);

        // Rotate to right angle
        transform.rotation = targetRot;
    }
}
