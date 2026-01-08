using UnityEngine;
using System.Collections;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Drone : MonoBehaviour
{
    // Coin object
    private GameObject coin;

    // Drone speed
    [SerializeField] private float speed;
    
    // Coin collecting fields
    private bool isCollecting = false;
    private float checkInterval = 1f;
    private float elapsed = 0f;

    void Update()
    {
        if (isCollecting)
        {
            return;
        }

        // Checks for uncollected coins after each interval
        if (elapsed < checkInterval)
        {
            elapsed += Time.deltaTime;
        }
        else 
        {
            FindClosestCoin();
            elapsed = 0f;
        }

        MoveDrone();
    }

    // Searches for the closest coin 
    public void FindClosestCoin()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Collectable");;

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 dronePos = transform.position;

        foreach (GameObject c in coins)
        {
            Vector3 diff = c.transform.position - dronePos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = c;
                distance = curDistance;
            }
        }
        coin = closest;
    }

    // Moves drone horizontally toward the nearest uncollected coin
    private void MoveDrone()
    {
        if (coin != null)
        {
            float fixedY = transform.position.y;

            // Uses XZ positions only
            Vector3 coinXZ = new Vector3(coin.transform.position.x, 0f, coin.transform.position.z);
            Vector3 currXZ = new Vector3(transform.position.x, 0f, transform.position.z);

            Vector3 direction = coinXZ - currXZ;
            float distance = direction.magnitude;

            if (distance > 0.001f)
            {
                direction.Normalize();

                // Rotates the drone in the direction of motion
                float rotateStep = 45f * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotateStep, 0f);
                transform.rotation = Quaternion.LookRotation(newDirection);

                // Moves drone
                float moveStep = speed * Time.deltaTime;
                if (moveStep > distance)
                {
                    moveStep = distance;
                }
                transform.position += transform.forward * moveStep;

                // Keep Y level fixed
                transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
            }
        }
    }

    // Toggles the drone's collecting state
    public void ToggleCoinCollect(bool isCollect)
    {
        isCollecting = isCollect;
    }
}