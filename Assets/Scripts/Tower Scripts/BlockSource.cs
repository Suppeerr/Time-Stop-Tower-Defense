using UnityEngine;

public class BlockSource : MonoBehaviour
{
    public GameObject draggablePrefab; // assign your prefab in Inspector

    private void OnMouseDown()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        // Get a spawn position under the cursor (XZ plane)
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 spawnPos = transform.position; // fallback position

        if (Physics.Raycast(ray, out hit))
        {
            spawnPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        // Spawn the block
        GameObject newBlock = Instantiate(draggablePrefab, spawnPos, Quaternion.identity);

        // Tell it to start dragging right away
        Draggable draggable = newBlock.GetComponent<Draggable>();
        if (draggable != null)
        {
            draggable.StartDragAtCursor();
        }
        else
        {
            Debug.LogError("Spawned prefab is missing Draggable script!");
        }
    }
}
