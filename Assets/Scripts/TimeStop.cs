using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class TimeStop : MonoBehaviour
{
    public static event Action<bool> timeStop;
    public AudioSource timeStopSFX;
    public TMP_Text durationText;
    private Color activeColor = Color.yellow;
    private Color inactiveColor = Color.gray;
    private Color cooldownColor = Color.red;
    public float duration = 5f;
    public float maxDur = 5f;
    public float rechargeRate = 1f;
    public float waitTime = 5f;
    private bool active = false;
    private float delayAfterSFX = .3f;
    private bool isCoroutineRunning = false;


    // Once timestop is triggered, all animated objects freeze for the duration
    void Update()
    {
        // Recharge duration when inactive
        if (!active && duration < maxDur)
        {
            duration += rechargeRate * Time.deltaTime;
            if (duration > maxDur)
                duration = maxDur;
        }

        // Update UI
        if (durationText != null)
        {
            durationText.text = duration.ToString("F2");
            durationText.color = active ? activeColor : (duration <= 1f ? cooldownColor : inactiveColor);
            durationText.gameObject.SetActive(duration < maxDur);
        }

        // Trigger key input
        if (!active && !isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame && duration > waitTime)
        {
            StartCoroutine(StartTimeStopAfterDelay());
        }
    }

    private IEnumerator StartTimeStopAfterDelay()
    {
        isCoroutineRunning = true;

        if (timeStopSFX != null)
            timeStopSFX.Play();

        yield return new WaitForSecondsRealtime(delayAfterSFX);

        active = true;
        timeStop?.Invoke(true);

        while (duration > 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        active = false;
        timeStop?.Invoke(false);
        isCoroutineRunning = false;
    }
}
