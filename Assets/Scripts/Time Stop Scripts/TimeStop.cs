using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using TMPro;

public class TimeStop : MonoBehaviour
{
    public static event Action<bool> TimeStopEvent;
    public AudioSource timeStopStartSFX;
    public AudioSource timeStopEndSFX;
    public AudioSource cooldownEndSFX;
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
    private float secondsElapsed = 0f;
    public StoredTimeManager storedTimeManager;

    // Once timestop is triggered, all animated objects freeze for the duration
    void Update()
    {
        if (!LevelStarter.HasLevelStarted || BaseHealthManager.IsGameOver)
        {
            return;
        }
        
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
                cooldownEndSFX.Play();
                cooldown = 0;
            }
        }

        UpdateUI();

        // Trigger key input
        if (!active && !isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame && cooldown == 0f)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimeStopAfterDelay());
        }
        if (active && isCoroutineRunning && Keyboard.current.tKey.wasPressedThisFrame)
        {
            EndTimestop(3f);
        }
    }

    private IEnumerator StartTimeStopAfterDelay()
    {
        isCoroutineRunning = true;
        timeStopStartSFX?.Play();

        yield return new WaitForSecondsRealtime(delayAfterSFX);

        active = true;
        TimeStopEvent?.Invoke(true);
        beamSpawner.SetActive(true);

        UpdateParticles();

        while (duration > 0f)
        {
            secondsElapsed += Time.deltaTime;

            if (secondsElapsed >= 1f)
            {
                storedTimeManager.UpdateSeconds();
                secondsElapsed = 0f;
            }

            duration -= Time.deltaTime;
            yield return null;
        }
        EndTimestop(3f);
    }

    // Ends the timestop
    private void EndTimestop(float cd)
    {
        StopAllCoroutines();
        active = false;
        TimeStopEvent?.Invoke(false);
        isCoroutineRunning = false;
        timeStopEndSFX?.Play();
        cooldown = cd;
        beamSpawner.SetActive(false);

        UpdateParticles();
    }

    // Updates timestop timers
    private void UpdateUI()
    {
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
    }

    private void UpdateParticles()
    {
        ParticleSystem[] allParticles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (var ps in allParticles)
        {
            if (ps.gameObject.layer == LayerMask.NameToLayer("Ignore Time Stop"))
            {
                continue;
            }

            if (active)
            {
                if (ps.isPlaying)
                {
                    ps.Pause(true);
                }
            }
            else
            {
                if (ps.isPaused)
                {
                    ps.Play(true);
                }

            }
            
        }
    }
}
