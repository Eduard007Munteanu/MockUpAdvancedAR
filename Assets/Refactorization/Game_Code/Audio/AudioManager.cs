using UnityEngine;

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

    public void PlayTheme(AudioClip themeClip)
    {
        if (thematicAudioSource == null) { Debug.LogError("Thematic AudioSource is missing!"); return; }
        if (themeClip == null) { Debug.LogWarning("PlayTheme called with a null AudioClip."); return; }

        thematicAudioSource.clip = themeClip;
        thematicAudioSource.Play();
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