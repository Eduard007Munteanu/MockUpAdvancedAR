using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // These will be populated by AddComponent in Awake
    public AudioSource ambientAudioSource;
    public AudioSource thematicAudioSource;

    // This AudioSource needs to be assigned in the Inspector
    public AudioSource soundEffectAudioSource;

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AudioManager instance is null. Ensure an AudioManager GameObject is in your scene, active, and its script execution order is set appropriately.");
            }
            return _instance;
        }
    }

    private Coroutine themeFadeCoroutine;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure soundEffectAudioSource is assigned if you expect it to be
            if (soundEffectAudioSource == null)
            {
                Debug.LogWarning("SoundEffectAudioSource is not assigned in the AudioManager Inspector. If you intend to use it, please assign an AudioSource component.");
            }
        }
        else if (_instance != this)
        {
            Debug.LogWarning("Another instance of AudioManager already exists. Destroying this new one.");
            Destroy(gameObject);
            return; 
        }
    }

    public void PlayAmbientSound(AudioClip clip)
    {
        if (ambientAudioSource == null) { Debug.LogError("Ambient AudioSource is missing!"); return; }
        if (clip == null) { Debug.LogWarning("PlayAmbientSound called with a null AudioClip."); return; }

        if (ambientAudioSource.clip != clip || !ambientAudioSource.isPlaying)
        {
            ambientAudioSource.clip = clip;
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }
    }

    public void PlayTheme(float fadeDuration = 1.0f)
    {
        if (thematicAudioSource == null || thematicAudioSource.clip == null)
        {
            Debug.LogError("Thematic AudioSource or its clip is missing!");
            return;
        }

        // If already fading, stop previous fade
        if (themeFadeCoroutine != null)
            StopCoroutine(themeFadeCoroutine);

        themeFadeCoroutine = StartCoroutine(RestartThemeWithFade(fadeDuration));
    }

    private IEnumerator RestartThemeWithFade(float fadeDuration)
    {
        // Fade out if playing
        if (thematicAudioSource.isPlaying)
            yield return StartCoroutine(FadeAudioSource(thematicAudioSource, 0f, fadeDuration / 2f));

        thematicAudioSource.Stop();
        thematicAudioSource.volume = 0f;
        thematicAudioSource.Play();

        // Fade in
        yield return StartCoroutine(FadeAudioSource(thematicAudioSource, 1f, fadeDuration / 2f));
    }

    private IEnumerator FadeAudioSource(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    public void StopTheme()
    {
        if (thematicAudioSource != null && thematicAudioSource.isPlaying)
        {
            thematicAudioSource.Stop();
        }
    }

    public void SetAmbientVolume(float volume)
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.volume = Mathf.Clamp01(volume); // Good practice to clamp volume
        }
    }

    public void SetThemeVolume(float volume)
    {
        if (thematicAudioSource != null)
        {
            thematicAudioSource.volume = Mathf.Clamp01(volume);
        }
    }

    // Example method for playing a sound effect using the soundEffectAudioSource
    public void PlaySoundEffect()
    {
        var effectClip = soundEffectAudioSource.clip; // Assuming you want to play the current clip assigned to soundEffectAudioSource

        if (soundEffectAudioSource == null) { Debug.LogError("SoundEffect AudioSource is not assigned!"); return; }
        if (effectClip == null) { Debug.LogWarning("PlaySoundEffect called with a null AudioClip."); return; }

        // PlayOneShot is often good for sound effects as it doesn't interrupt the source's current clip
        // and can overlap. If you want to change the clip and play, use:
        // soundEffectAudioSource.clip = effectClip;
        // soundEffectAudioSource.Play();
        soundEffectAudioSource.PlayOneShot(effectClip);
    }

    public void SetSoundEffectVolume(float volume)
    {
        if (soundEffectAudioSource != null)
        {
            soundEffectAudioSource.volume = Mathf.Clamp01(volume);
        }
    }
}