using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // Main valley track
    [SerializeField] private AudioSource valleyTrack;
    
    // Volume and pitch fields
    private float originalVolume;
    private float normalPitch = 1.0f;
    private float frozenPitch = 0.6f;

    // Time stop transition volume/pitch fade duration
    private float fadeDuration = 1.6f;

    // Update gate and coroutine fields
    private bool lastFrozenState = false;
    private Coroutine fadeRoutine;
    

    void Start()
    {
        StartCoroutine(LoopSoundtrack());
    }

    private IEnumerator LoopSoundtrack()
    {
        // Waits till the game has started to play the soundtrack
        yield return new WaitUntil(() => LevelStarter.HasLevelStarted);

        // Loops soundtrack after it ends
        valleyTrack.loop = true;
        valleyTrack.Play();
    }

    void Update()
    {
        // Fades volume and pitch with time stop start or end
        if (ProjectileManager.Instance.IsFrozen != lastFrozenState)
        {
            if (ProjectileManager.Instance.IsFrozen)
            {
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                }

                fadeRoutine = StartCoroutine(FadeOut());
            }
            else
            {
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                }

                fadeRoutine = StartCoroutine(FadeIn());
            }

            lastFrozenState = ProjectileManager.Instance.IsFrozen;
        }
    }

    // Fades volume in while raising pitch 
    private IEnumerator FadeIn()
    {
        valleyTrack.volume = 0f;
        valleyTrack.UnPause();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            valleyTrack.volume = Mathf.Lerp(0f, originalVolume, elapsed / fadeDuration);
            valleyTrack.pitch = Mathf.Lerp(frozenPitch, normalPitch, elapsed / fadeDuration);

            yield return null;
        }

        valleyTrack.pitch = normalPitch;
        valleyTrack.volume = originalVolume;
    }

    // Fades volume out while lowering pitch 
    private IEnumerator FadeOut()
    {
        float startVolume = valleyTrack.volume;
        originalVolume = startVolume;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            valleyTrack.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            valleyTrack.pitch = Mathf.Lerp(normalPitch, frozenPitch, elapsed / fadeDuration);
            yield return null;
        }

        valleyTrack.volume = 0f;
        valleyTrack.pitch = frozenPitch;
        valleyTrack.Pause();
    }
}
