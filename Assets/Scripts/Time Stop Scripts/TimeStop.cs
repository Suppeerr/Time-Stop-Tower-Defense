using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class TimeStop : MonoBehaviour
{
    public static event Action<bool> timeStop;
    public AudioSource timeStopStartSFX;
    public AudioSource timeStopEndSFX;
    public TMP_Text durationText;
    public TMP_Text cooldownText;
    private Color activeColor = new Color(.8f, .8f, 0.1f);
    private Color inactiveColor = Color.white;
    private Color cooldownColor = Color.red;
    public float duration = 5f;
    public float maxDur = 5f;
    public float rechargeRate = 1f;
    private float cooldown;
    private bool active = false;
    private float delayAfterSFX = .3f;
    private bool isCoroutineRunning = false;
    public GameObject beamSpawner;

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
            durationText.text = duration.ToString("F1");
            if (active)
            {
                durationText.color = activeColor;
                durationText.fontMaterial.SetFloat("_GlowPower", 0.5f);
            }
            else
            {
                durationText.color = inactiveColor;
                durationText.fontMaterial.SetFloat("_GlowPower", 0f);
            }
        }
        if (cooldownText != null)
        {
            cooldownText.text = cooldown.ToString("F1");
            cooldownText.color = cooldownColor;
            cooldownText.gameObject.SetActive(cooldown > 0f);
        }

        // Trigger key input
        if (!active && !isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame && cooldown == 0f)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimeStopAfterDelay());
        }
        if (active && isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame)
        {
            EndTimestop(2f);
        }
    }

    private IEnumerator StartTimeStopAfterDelay()
    {
        isCoroutineRunning = true;
        timeStopStartSFX?.Play();

        yield return new WaitForSecondsRealtime(delayAfterSFX);

        active = true;
        timeStop?.Invoke(true);
        beamSpawner.SetActive(true);

        while (duration > 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        EndTimestop(1f);
    }

    // Ends the timestop
    private void EndTimestop(float cd)
    {
        StopAllCoroutines();
        active = false;
        timeStop?.Invoke(false);
        isCoroutineRunning = false;
        timeStopEndSFX?.Play();
        cooldown = cd;
        beamSpawner.SetActive(false);
    }
}
