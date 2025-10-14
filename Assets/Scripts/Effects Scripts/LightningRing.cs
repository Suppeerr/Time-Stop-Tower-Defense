using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LightningRing : MonoBehaviour
{
    [Range(3, 128)]
    public int points = 32;                  // Number of points the lightning ring has
    public float radius = 1f;                // Horizontal distance from projectile to ring
    public float jitterAmount = 0.05f;       // How much the ring jitters
    public float jitterSpeed = 10f;          // How fast the ring jitters

    private LineRenderer lr;                 // LineRenderer for the ring
    private Vector3[] basePositions;         // Regular positions of each point
    private Vector3[] currentPositions;      // Updated positions of each point

    void OnEnable()
    {
        // Initializes variables
        lr = GetComponent<LineRenderer>();
        lr.positionCount = points;

        basePositions = new Vector3[points];
        currentPositions = new Vector3[points];

        GenerateBaseCircle();
    }

    void Update()
    {
        // Make the ring flicker over time
        AnimateLightning();
    }

    void GenerateBaseCircle()
    {
        // Creates a ring around the projectile
        for (int i = 0; i < points; i++)
        {
            float angle = (2 * Mathf.PI / points) * i;
            basePositions[i] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    void AnimateLightning()
    {
        // Creates a random jittering effect
        for (int i = 0; i < points; i++)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * jitterSpeed, i * 1.3f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(Time.time * jitterSpeed + 100, i * 2.1f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * jitterSpeed + 200, i * 3.7f) - 0.5f;

            // Jitter in all directions, including Y (vertical)
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