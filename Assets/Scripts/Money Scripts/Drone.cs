using UnityEngine;
using System.Collections;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Drone : MonoBehaviour
{
    public float speed = 10.0f;
    private GameObject Coin;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Coin = FindClosestCoin();
        if (Coin != null)
        {
            Vector3 coinXZ = Coin.transform.position - new Vector3(0, Coin.transform.position.y, 0);
            Vector3 currXZ = (transform.position - new Vector3(0, transform.position.y, 0));
            Vector3 direction = coinXZ - currXZ;
            direction.Normalize();
            float rotateStep = 45 * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotateStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private GameObject FindClosestCoin()
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
        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
        }
    }
}