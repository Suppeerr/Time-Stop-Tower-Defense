using UnityEngine;
using System.Collections;

public class BarrelAim : MonoBehaviour
{
    // Shoot location
    [SerializeField] private Transform shootPoint;
    
    // Target direction
    private Vector3 targetDirection;        

    // Aims at given target whenever called
    public void AimAtTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        // Calculates desired rotation
        Vector3 direction = (target.position + Vector3.up * 5f) - shootPoint.position;

        // If direction is very similar, skip the update
        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        // Rotate to right angle
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = targetRot;
    }
}
