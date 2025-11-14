using UnityEngine;
using System.Collections;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Drone : MonoBehaviour
{
    public float speed = 8.0f;
    private GameObject Coin;
    private bool isCollecting = false;
    private float checkInterval = 1f;
    private float elapsed = 0f;

    // Update is called once per frame
    void Update()
    {
        if (ProjectileManager.IsFrozen || isCollecting)
        {
            return;
        }

        if (elapsed < checkInterval)
        {
            elapsed += Time.deltaTime;
        }
        else 
        {
            FindClosestCoin();
            elapsed = 0f;
        }

        if (Coin != null)
        {
            Vector3 coinXZ = new Vector3(Coin.transform.position.x, Coin.transform.position.y, Coin.transform.position.z);
            Vector3 currXZ = (transform.position - new Vector3(0, transform.position.y, 0));
            Vector3 direction = coinXZ - currXZ;
            direction.Normalize();
            float rotateStep = 45 * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotateStep, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    public void FindClosestCoin()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Collectable");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        Coin = closest;
    }

    public void ChangeCoin(GameObject newCoin)
    {
        Coin = newCoin;
    }

    public void ToggleCoinCollect(bool isCollect)
    {
        isCollecting = isCollect;
    }
}