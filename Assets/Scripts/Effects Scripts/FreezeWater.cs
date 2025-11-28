using UnityEngine;

public class FreezeWater : MonoBehaviour
{
    private bool wasFrozen = false;
    private float originalFlowSpeed;
    private float originalFoamSpeed;
    private Material waterMat;

    void Start()
    {
        waterMat = GetComponent<Renderer>().material;
        originalFlowSpeed = waterMat.GetFloat("_FlowSpeed");
    }

    // Update is called once per frame
    void Update()
    {
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
