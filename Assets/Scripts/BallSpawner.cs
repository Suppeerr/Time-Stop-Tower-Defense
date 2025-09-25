using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject cannon_projectile;
    public float spawnPerSecond = 5f;
    private float spawnRate;
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRate = 1f / spawnPerSecond;
        spawnBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawnBall();
            timer = 0;
        }
    }

    void spawnBall()
    {
        Instantiate(cannon_projectile, transform.position, transform.rotation);

    }
}
