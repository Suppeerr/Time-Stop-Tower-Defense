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
    private bool textEnabled = false;
    private bool active = false;
    private bool isTransitioning = false;
    public GameObject beamSpawner;
    private float secondsElapsed = 0f;
    private float timeStopTransitionTime = 1.5f;
    [SerializeField] private StoredTimeManager storedTimeManager;
    [SerializeField] private TimeStopOverlay timeStopOverlay;

    void Start()
    {
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

        if (cooldown > 0f)
        {
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

        UpdateUI();

        if (cooldown > 0f)
        {
            return;
        }

        // Trigger key input
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (active)
            {
                StartCoroutine(EndTimestop(3f));
                return;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(StartTimeStopAfterDelay());
            }
        } 
    }

    private IEnumerator StartTimeStopAfterDelay()
    {
        isTransitioning = true;
        timeStopStartSFX?.Play();
        timeStopOverlay.StartTimeStopVFX();
        active = true;
        
        float elapsedDelay = 0f;

        while (elapsedDelay < timeStopTransitionTime)
        {
            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / timeStopTransitionTime;

            Time.timeScale = Mathf.Lerp(1, 0, t);

            yield return null;
        }

        isTransitioning = false;
        Time.timeScale = 1f;

        TimeStopEvent?.Invoke(true);
        cooldown = 2f;
        ProjectileManager.Instance.BlinkNormalProjectiles();
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

        StartCoroutine(EndTimestop(3f));
    }

    // Ends the timestop
    private IEnumerator EndTimestop(float cd)
    {
        float elapsedDelay = 0f;
        timeStopEndSFX?.Play();
        timeStopOverlay.StopTimeStopVFX();
        TimeStopEvent?.Invoke(false);
        active = false;
        isTransitioning = true;
        ProjectileManager.Instance.UnblinkNormalProjectiles();

        while (elapsedDelay < timeStopTransitionTime)
        {
            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / timeStopTransitionTime;

            Time.timeScale = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        Time.timeScale = 1f;

        StopAllCoroutines();
        isTransitioning = false;
        
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
