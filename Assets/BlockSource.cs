using UnityEngine;

public class BlockSource : MonoBehaviour
{
    public GameObject draggablePrefab; // assign a cube prefab with Draggable.cs attached
    public float spawnOffset = 2f;     // distance to spawn from source

    private void OnMouseDown()
    {
        // Spawn new cube near the source
        Vector3 spawnPos = transform.position + new Vector3(spawnOffset, 0, 0);
        Instantiate(draggablePrefab, spawnPos, Quaternion.identity);
    }
}
