using UnityEngine;

public class FreezeSway : MonoBehaviour
{
    private bool wasFrozen = false;
    private float originalSwaySpeed;
    private Material swayMat;

    void Start()
    {
        swayMat = GetComponent<Renderer>().material;
        originalSwaySpeed = swayMat.GetFloat("_SwaySpeed");
    }

    // Update is called once per frame
    void Update()
    {
        bool isFrozen = ProjectileManager.IsFrozen;
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
