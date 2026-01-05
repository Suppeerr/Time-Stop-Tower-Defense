using UnityEngine;

public class BladeSpin : MonoBehaviour
{
    // Drone blade rotation speed
    [SerializeField] private float rotationSpeed = 1000f;

    void Update()
    {
        // Rotates the drone blades
        if (ProjectileManager.Instance.IsFrozen)
        {
            return;
        }
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
