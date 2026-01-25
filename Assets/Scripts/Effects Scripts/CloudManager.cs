using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudManager : MonoBehaviour
{
    // Cloud manager instance
    public static CloudManager Instance;

    // List of different cloud prefabs
    [SerializeField] private List<GameObject> cloudPrefabs;

    // List of active clouds
    private List<GameObject> clouds = new List<GameObject>();

    // Cloud spawning interval and lifetime
    private float spawnInterval = 60f;
    private float cloudLifetime = 120f;
    
    // Minimum and maximum cloud movement speeds
    private float minCloudSpeed = 0.4f;
    private float maxCloudSpeed = 0.7f;

    // Timing field
    private float elapsed;

    void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnRandomClouds(8);
    }

    void Update()
    {
        if (elapsed < spawnInterval)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            elapsed = 0f;
            SpawnRandomClouds(2);
        }

        MoveClouds();
    }

    // Spawns clouds at random positions
    private void SpawnRandomClouds(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int cloudNum = Random.Range(0, cloudPrefabs.Count - 1);

            float xSpawnPos = Random.Range(-70f, 70f);
            float ySpawnPos = 75f;
            float zSpawnPos = Random.Range(20f, 150f);

            GameObject newCloud = Instantiate(cloudPrefabs[cloudNum], new Vector3(xSpawnPos, ySpawnPos, zSpawnPos), Quaternion.identity);
            clouds.Add(newCloud);

            StartCoroutine(DestroyCloud(newCloud));
            Destroy(newCloud, cloudLifetime);
        }
    }

    // Moves the clouds a bit each frame
    private void MoveClouds()
    {
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.position += new Vector3(0f, 0f, -Random.Range(minCloudSpeed, maxCloudSpeed) * Time.deltaTime);
        }
    }

    // Destroys a cloud after some time
    private IEnumerator DestroyCloud(GameObject cloud)
    {
        yield return new WaitForSeconds(cloudLifetime);

        if (cloud != null)
        {
            clouds.Remove(cloud);
            Destroy(cloud);
        }
    }
}
