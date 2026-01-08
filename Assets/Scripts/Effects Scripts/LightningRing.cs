using UnityEngine;
using System.Collections;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LightningRing : MonoBehaviour
{
    // Lightning ring parameters
    [Range(3, 128)]
    [SerializeField] private int points = 32;                  
    [SerializeField] private float radius = 1f;                
    [SerializeField] private float jitterAmount = 0.05f;       
    [SerializeField] private float jitterSpeed = 10f;          

    // LineRenderer for the ring
    private LineRenderer lr;                 

    // Positio fields
    private Vector3[] basePositions;         
    private Vector3[] currentPositions;      

    void OnEnable()
    {
        // Initializes line renderer and positions
        lr = GetComponent<LineRenderer>();
        lr.positionCount = points;

        basePositions = new Vector3[points];
        currentPositions = new Vector3[points];

        GenerateBaseCircle();
    }

    void Update()
    {
        // Make the ring flicker over time
        StartCoroutine(AnimateLightning());
    }

    // Generates the basic circle for the ring
    private void GenerateBaseCircle()
    {
        // Creates the ring using points
        for (int i = 0; i < points; i++)
        {
            float angle = (2 * Mathf.PI / points) * i;
            basePositions[i] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    // Animates the ring to appear like lightning
    private IEnumerator AnimateLightning()
    {
        // Creates a random jittering effect
        for (int i = 0; i < points; i++)
        {
            while (SettingsMenuOpener.Instance.MenuOpened)
            {
                yield return null;
            }

            float noiseX = Mathf.PerlinNoise(Time.unscaledTime * jitterSpeed, i * 1.3f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(Time.unscaledTime * jitterSpeed + 100, i * 2.1f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.unscaledTime * jitterSpeed + 200, i * 3.7f) - 0.5f;

            // Jitter in all directions
            Vector3 offset = new Vector3(noiseX, noiseY, noiseZ) * 2f * jitterAmount;

            currentPositions[i] = basePositions[i] + offset;
        }

        lr.SetPositions(currentPositions);
    }

    // Toggles visibility when firing
    public void SetVisible(bool visible)
    {
        lr.enabled = visible;
    }
}