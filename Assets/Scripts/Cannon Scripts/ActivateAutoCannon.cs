using UnityEngine;
using System.Collections;

public class ActivateAutoCannon : MonoBehaviour
{
    private Vector3 originalPos;
    private float activationTime = 1.5f;
    private bool isActivated = false;
    public BallSpawner ballSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = transform.position;
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Upgrader.AutoCannonBought && !isActivated)
        {
            foreach (Transform child in transform) 
            {
                child.gameObject.SetActive(true);
            }
            isActivated = true;
            StartCoroutine(ActivateCannon());
        }
    }

    private IEnumerator ActivateCannon()
    {
        // towerPlaceSFX?.Play();

        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
        isActivated = true;

        float elapsedDelay = 0f;

        while (elapsedDelay < activationTime)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / activationTime;
            transform.position = Vector3.Lerp(originalPos, originalPos + Vector3.left * 0.95f, t);

            yield return null;

            if (ballSpawner != null)
            {
                ballSpawner.enabled = true;
            }
        }
    }
}
