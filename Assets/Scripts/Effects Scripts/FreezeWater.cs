using UnityEngine;

public class FreezeWater : MonoBehaviour
{
    // Update check variable
    private bool wasFrozen = false;

    // Water animation speed and material
    private float originalFlowSpeed;
    private float originalFoamSpeed;
    private Material waterMat;

    void Start()
    {
        // Assigns the animation speed and material
        waterMat = GetComponent<Renderer>().material;
        originalFlowSpeed = waterMat.GetFloat("_FlowSpeed");
    }

    void Update()
    {
        // Freezes the water animation during time stop
        bool isFrozen = ProjectileManager.IsFrozen;
        if (isFrozen != wasFrozen)
        {
            if (isFrozen)
            {
                waterMat.SetFloat("_FlowSpeed", 0f);
            }
            else
            {
                waterMat.SetFloat("_FlowSpeed", originalFlowSpeed);
            }
            
            wasFrozen = isFrozen;
        }
    }
}
