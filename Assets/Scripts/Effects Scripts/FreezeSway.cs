using UnityEngine;

public class FreezeSway : MonoBehaviour
{
    // Update check variable
    private bool wasFrozen = false;

    // Swaying animation speed and material
    private float originalSwaySpeed;
    private Material swayMat;

    void Start()
    {
        // Assigns the animation speed and material
        swayMat = GetComponent<Renderer>().material;
        originalSwaySpeed = swayMat.GetFloat("_SwaySpeed");
    }

    void Update()
    {
        // Freezes the swaying animation during time stop
        bool isFrozen = ProjectileManager.Instance.IsFrozen;
        if (isFrozen != wasFrozen)
        {
            if (isFrozen)
            {
                swayMat.SetFloat("_SwaySpeed", 0f);
            }
            else
            {
                swayMat.SetFloat("_SwaySpeed", originalSwaySpeed);
            }
            
            wasFrozen = isFrozen;
        }
    }
}
