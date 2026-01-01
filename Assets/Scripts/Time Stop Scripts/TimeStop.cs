using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using TMPro;

public class TimeStop : MonoBehaviour
{
    // Time stop activation fields
    public static event Action<bool> TimeStopEvent;
    private bool active = false;

    // UI fields
    public TMP_Text durationText;
    public TMP_Text cooldownText;
    private bool textEnabled = false;
    private Color activeColor = new Color(.8f, .8f, 0.1f);
    private Color inactiveColor = Color.white;
    private Color cooldownColor = Color.red;

    // Duration and recharge fields
    public float duration = 5f;
    public float maxDur = 5f;
    public float rechargeRate = 1f;
    private float cooldown;
    
    // Transition fields
    private bool isTransitioning = false;
    private float timeStopTransitionTime = 1.5f;

    // Sound effect fields
    public AudioSource timeStopStartSFX;
    public AudioSource timeStopEndSFX;
    public AudioSource cooldownEndSFX;

    // Scripts and gameobjects
    [SerializeField] GameObject beamSpawner;
    [SerializeField] private StoredTimeManager storedTimeManager;
    [SerializeField] private TimeStopOverlay timeStopOverlay;

    void Start()
    {
        // Initializes text fields and beamspawner 
        durationText.enabled = false;
        cooldownText.enabled = false;
        beamSpawner.SetActive(false);
    }

    // Once timestop is triggered, all animated objects freeze for the duration
    void Update()
    {
        if (!Upgrader.TimeStopBought || BaseHealthManager.IsGameOver || isTransitioning)
        {
            return;
        }
        else if (!textEnabled)
        {
            textEnabled = true;
            durationText.enabled = true;
            cooldownText.enabled = true;
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

        if (cooldown > 0f && !ProjectileManager.IsFrozen)
        {
            // Decreases reactivation cooldown with time outside of time stop
            cooldown -= Time.deltaTime;
            if (cooldown < 0f)
            {
                if (!active)
                {
                    cooldownEndSFX.Play();
                }
                 
                cooldown = 0f;
            }
        }
        else
        {
            // Decreases deactivation cooldown with time during time stop
            cooldown -= Time.unscaledDeltaTime;
            if (cooldown < 0f)
            { 
                cooldown = 0f;
            }
        }

        UpdateUI();

        // Trigger key input
        if (Keyboard.current.tKey.wasPressedThisFrame && cooldown == 0f)
        {
            if (active)
            {
                StartCoroutine(EndTimestop(3f));
                return;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(StartTimeStop(2f));
            }
        } 
    }

    // Starts time stop after a short transition
    private IEnumerator StartTimeStop(float cd)
    {
        timeStopStartSFX?.Play();
        timeStopOverlay.StartTimeStopVFX();
        active = true;
        
        yield return StartCoroutine(TransitionTimeStop(1f, 0f));

        TimeStopEvent?.Invoke(true);
        cooldown = cd;
        ProjectileManager.Instance.ToggleNormalBlink(true);
        beamSpawner.SetActive(true);

        UpdateParticles();

        yield return StartCoroutine(CountdownTimeStop());

        StartCoroutine(EndTimestop(3f));
    }

    // Ends time stop after a short transition
    private IEnumerator EndTimestop(float cd)
    {
        timeStopEndSFX?.Play();
        timeStopOverlay.StopTimeStopVFX();
        TimeStopEvent?.Invoke(false);
        active = false;
        ProjectileManager.Instance.ToggleNormalBlink(false);

        yield return StartCoroutine(TransitionTimeStop(0f, 1f));

        Time.timeScale = 1f;

        StopAllCoroutines();        
        cooldown = cd;
        beamSpawner.SetActive(false);

        UpdateParticles();
    }

    // Smoothly transitions between time stop and normal states
    private IEnumerator TransitionTimeStop(float startScale, float endScale)
    {
        isTransitioning = true;

        float elapsedDelay = 0f;
        while (elapsedDelay < timeStopTransitionTime)
        {
            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / timeStopTransitionTime;

            Time.timeScale = Mathf.Lerp(startScale, endScale, t);

            yield return null;
        }

        isTransitioning = false;
    }

    // Counts down time stop duration when active 
    private IEnumerator CountdownTimeStop()
    {
        float secondsElapsed = 0f;

        while (duration > 0f)
        {
            secondsElapsed += Time.unscaledDeltaTime;

            if (secondsElapsed >= 1f)
            {
                storedTimeManager.UpdateSeconds();
                secondsElapsed = 0f;
            }

            duration -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // Updates timestop text indicators
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

    // Freezes and unfreezes particle animations 
    private void UpdateParticles()
    {
        ParticleSystem[] allParticles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);

        foreach (var ps in allParticles)
        {
            if (ps.gameObject.layer == LayerMask.NameToLayer("Ignore Time Stop"))
            {
                continue;
            }

            if (active && ps.isPlaying)
            {
                ps.Pause(true);
            }
            else if (!active && ps.isPaused)
            {
                ps.Play(true);
            } 
        }
    }
}
