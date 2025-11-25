using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource valleyTrack;
    private Coroutine fadeRoutine;
    private float originalVolume;
    private float fadeDuration = 1.6f;
    private float normalPitch = 1.0f;
    private float frozenPitch = 0.6f;
    private bool lastFrozenState = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoopSoundtrack());
    }

    void Update()
    {
        if (ProjectileManager.IsFrozen != lastFrozenState)
        {
            if (ProjectileManager.IsFrozen)
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

            lastFrozenState = ProjectileManager.IsFrozen;
        }
        
    }

    private IEnumerator LoopSoundtrack()
    {
        // Waits till the game has started to play the soundtrack
        yield return new WaitUntil(() => LevelStarter.HasLevelStarted);

        // Loops soundtrack after it ends
        valleyTrack.loop = true;
        valleyTrack.Play();
    }

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
}
