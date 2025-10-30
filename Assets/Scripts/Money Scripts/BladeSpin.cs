using UnityEngine;

public class BladeSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000f;

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
