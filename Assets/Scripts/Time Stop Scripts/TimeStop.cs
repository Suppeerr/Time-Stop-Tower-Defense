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
    public float cooldown = 0f;
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
            {
                duration = maxDur;
            }
        }

        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            if (cooldown < 0 )
            {
                cooldown = 0;
            }
        }

        // Update UI
        if (durationText != null)
        {
            durationText.text = duration.ToString("F2");
            durationText.color = active ? activeColor : (cooldown > 0f ? cooldownColor : inactiveColor);
            durationText.gameObject.SetActive(duration < maxDur);
        }

        // Trigger key input
        if (!active && !isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame && cooldown == 0f)
        {
            StartCoroutine(StartTimeStopAfterDelay());
        }
        if (active && isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame)
        {
            StopAllCoroutines();
            active = false;
            timeStop?.Invoke(false);
            isCoroutineRunning = false;
            cooldown = 1.5f;
        }
    }

    private IEnumerator StartTimeStopAfterDelay()
    {
        isCoroutineRunning = true;

        timeStopSFX?.Play();

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
        cooldown = 1f;
    }
}
