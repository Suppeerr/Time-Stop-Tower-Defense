using UnityEngine;
using System.Collections;

public class PlacedTower : MonoBehaviour
{
    // ballSpawner script
    private BallSpawner ballSpawner;

    // Tower placement animation
    private float placementTime = 0.3f;
    private float placementHeight = 3f;
    public ParticleSystem dirtBurstPrefab;
    public ParticleSystem impactRingPrefab;

    // Tower placement audio
    public AudioSource towerPlaceSFX;

    void Awake()
    {
        ballSpawner = GetComponent<BallSpawner>();
        ballSpawner.enabled = false;

    }

    // Places the tower with animations
    public IEnumerator PlaceTower(Vector3 endPos)
    {
        towerPlaceSFX?.Play();
        float elapsedDelay = 0f;
        Vector3 startPos = endPos + Vector3.up * placementHeight;

        while (elapsedDelay < placementTime)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / placementTime;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // Plays dirt burst animation
        if (dirtBurstPrefab != null)
        {
            ParticleSystem d = Instantiate(dirtBurstPrefab, endPos + new Vector3(0f, -0.8f, 0f), Quaternion.Euler(-90, 0, 0));
            d.Play();
            Destroy(d.gameObject, d.main.duration);
        }

        // Plays ring impact animation
        if (dirtBurstPrefab != null)
        {
            ParticleSystem r = Instantiate(impactRingPrefab, endPos + new Vector3(0f, -0.8f, 0f), Quaternion.Euler(-90, 0, 0));
            r.Play();
            Destroy(r.gameObject, r.main.duration);
        }

        if (ballSpawner != null)
        {
            ballSpawner.enabled = true;
        }
    }
}
