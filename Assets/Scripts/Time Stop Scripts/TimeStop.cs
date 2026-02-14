using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TimeStop : MonoBehaviour
{
    // Time stop instance
    public static TimeStop Instance;

    // Time stop activation fields
    public bool IsFrozen { get; private set; }
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

    // Rune animation fields
    [SerializeField] private Color baseRuneColor;
    [SerializeField] private Color glowRuneColor;
    [SerializeField] private Color upgradedGlowRuneColor;
    private Color activeGlowColor;
    private float baseRuneIntensity = 1f;
    private float glowRuneIntensity = 2f;
    private float currentIntensity;
    private float runeTransitionTime = 1.5f;

    // Scripts and game objects
    [SerializeField] private List<TimeStopObject> timeStopObjects = new List<TimeStopObject>();
    [SerializeField] private GameObject beamSpawner;
    [SerializeField] private StoredTimeManager storedTimeManager;
    [SerializeField] private TimeStopOverlay timeStopOverlay;

    private void Awake()
    {
        // Avoids duplicates of this object
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There is a duplicate of the script " + this + "!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initializes text fields and beamspawner 
        durationText.enabled = false;
        cooldownText.enabled = false;
        beamSpawner.SetActive(false);

        // Initializes active rune glow color
        activeGlowColor = glowRuneColor;
    }

    // Once timestop is triggered, all animated objects freeze for the duration
    void Update()
    {
        if (!UpgradeManager.Instance.IsBought(UpgradeType.TimeStop) || 
            SettingsMenuOpener.Instance.MenuOpened || 
            BaseHealthManager.Instance.IsGameOver || 
            isTransitioning || CameraSwitcher.Instance.IsCameraMoving)
        {
            return;
        }
        else if (!textEnabled)
        {
            textEnabled = true;
            durationText.enabled = true;
            cooldownText.enabled = true;

            duration = 0f;
        }

        // Changes rune glow color if multi-charge has been bought
        if (UpgradeManager.Instance.IsBought(UpgradeType.MultiCharge) && activeGlowColor != upgradedGlowRuneColor)
        {
            duration = maxDur;
            activeGlowColor = upgradedGlowRuneColor;

            for (int i = 0; i < timeStopObjects.Count; i++)
            {
                StartCoroutine(AnimateRunes(currentIntensity, currentIntensity, glowRuneColor, upgradedGlowRuneColor, i));
            }

            UpdateRunes(true);
        }
        
        RechargeDuration();
        DecreaseCooldown();
        UpdateUI();

        // Starts or stops time stop when t key pressed
        ToggleTimeStop();
    }

    // Starts or stops time stop when t key pressed
    private void ToggleTimeStop()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame && cooldown == 0f)
        {
            if (active)
            {
                StopAllCoroutines();
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

        IsFrozen = true;
        cooldown = cd;
        ProjectileManager.Instance.ToggleNormalBlink(true);

        beamSpawner.SetActive(true);
        beamSpawner.GetComponent<BeamZap>().PreChargeProjectiles();

        UpdateParticles();

        yield return StartCoroutine(CountdownTimeStop());

        StartCoroutine(EndTimestop(3f));
    }

    // Ends time stop after a short transition
    private IEnumerator EndTimestop(float cd)
    {
        timeStopEndSFX?.Play();
        timeStopOverlay.StopTimeStopVFX();
        IsFrozen = false;
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

    // Recharge time stop duration when inactive
    private void RechargeDuration()
    {
        if (!active && duration < maxDur)
        {
            duration += rechargeRate * Time.deltaTime;
            if (duration > maxDur)
            {
                duration = maxDur;
            }

            UpdateRunes(true);
        }
    }

    // Counts down time stop duration when active 
    private IEnumerator CountdownTimeStop()
    {
        float secondsElapsed = 0f;

        while (duration > 0f)
        {
            while (SettingsMenuOpener.Instance.MenuOpened || CameraSwitcher.Instance.IsCameraMoving)
            {
                yield return null;
            }

            secondsElapsed += Time.unscaledDeltaTime;

            if (secondsElapsed >= 1f)
            {
                storedTimeManager.UpdateSeconds();
                secondsElapsed = 0f;
            }

            UpdateRunes(false);
            duration -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // Decrease time stop reactivation/deactivion cooldown outside of/during time stop
    private void DecreaseCooldown()
    {
        if (cooldown > 0f && !IsFrozen)
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
    }

    // Syncs rune glow with time stop duration
    private void UpdateRunes(bool isRecharging)
    {
        float tsPercent = duration / maxDur;

        for (int i = 0; i < timeStopObjects.Count; i++)
        {
            float runePercent = timeStopObjects[i].percent;
            bool runeActive = timeStopObjects[i].active;

            if (isRecharging)
            {
                if (tsPercent >= runePercent && !runeActive)
                {
                    timeStopObjects[i].SetObjectActive(true);
                    StartCoroutine(AnimateRunes(baseRuneIntensity, glowRuneIntensity, baseRuneColor, activeGlowColor, i));
                }
            }
            else
            {
                if (tsPercent <= runePercent && runeActive)
                {
                    timeStopObjects[i].SetObjectActive(false);
                    StartCoroutine(AnimateRunes(glowRuneIntensity, baseRuneIntensity, activeGlowColor, baseRuneColor, i));
                }
            }
        }
    }

    // Animates rune groups to glow or dim over time
    private IEnumerator AnimateRunes(float from, float to, Color fromColor, Color toColor, int num)
    { 
        float elapsed = 0f;

        List<GameObject> runes = timeStopObjects[num].runeObjects;

        while (elapsed < runeTransitionTime)
        {
            if (SettingsMenuOpener.Instance.MenuOpened)
            {
                yield return null;
            }

            if (from != to)
            {
                currentIntensity = Mathf.Lerp(from, to, elapsed / runeTransitionTime); 
            }
            
            Color curColor = Color.Lerp(fromColor, toColor, elapsed / runeTransitionTime);

            for (int i = 0; i < runes.Count; i++)
            {
                runes[i].GetComponent<Renderer>().material.SetColor("_Emission_Color", curColor * currentIntensity);
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        for (int i = 0; i < runes.Count; i++)
        {
            runes[i].GetComponent<Renderer>().material.SetColor("_Emission_Color", toColor * to);
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
