using UnityEngine;
using System.Collections;

public class ActivateAutoCannon : MonoBehaviour
{
    // Activation fields 
    private Vector3 originalPos;
    private float activationTime = 1.5f;
    private bool isActivated = false;

    // Ball spawner script
    public BallSpawner ballSpawner;

    void Start()
    {
        // Originally sets the cannon's children inactive
        originalPos = transform.position;
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Checks to see if cannon upgrade has been bought
        if (UpgradeManager.Instance.IsBought(UpgradeType.AutoCannon) && !isActivated)
        {
            foreach (Transform child in transform) 
            {
                child.gameObject.SetActive(true);
            }
            isActivated = true;
            StartCoroutine(ActivateCannon());
        }
    }

    // Activates the cannon after a short animation
    private IEnumerator ActivateCannon()
    {
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
        isActivated = true;

        float elapsedDelay = 0f;

        while (elapsedDelay < activationTime)
        {
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
