using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LightningRing : MonoBehaviour
{
    [Range(3, 128)]
    public int points = 32;
    public float radius = 1f;
    public float jitterAmount = 0.05f;
    public float jitterSpeed = 10f;

    private LineRenderer lr;
    private Vector3[] basePositions;
    private Vector3[] currentPositions;

    void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = points;

        basePositions = new Vector3[points];
        currentPositions = new Vector3[points];

        GenerateBaseCircle();
    }

    void Update()
    {
        // Make it flicker over time
        AnimateLightning();
    }

    void GenerateBaseCircle()
    {
        for (int i = 0; i < points; i++)
        {
            float angle = (2 * Mathf.PI / points) * i;
            basePositions[i] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    void AnimateLightning()
    {
        for (int i = 0; i < points; i++)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * jitterSpeed, i * 1.3f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(Time.time * jitterSpeed + 100, i * 2.1f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * jitterSpeed + 200, i * 3.7f) - 0.5f;

            // jitter in all directions, including Y (vertical)
            Vector3 offset = new Vector3(noiseX, noiseY, noiseZ) * 2f * jitterAmount;

            currentPositions[i] = basePositions[i] + offset;
        }

        lr.SetPositions(currentPositions);
    }

    // Optional — lets you toggle visibility when firing
    public void SetVisible(bool visible)
    {
        lr.enabled = visible;
    }
}