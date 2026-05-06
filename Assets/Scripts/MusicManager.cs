using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Layers")]
    [SerializeField] private AudioSource layerHigh;
    [SerializeField] private AudioSource layerMid;
    [SerializeField] private AudioSource layerLow;

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 2f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Optional: ensure all layers loop
        layerLow.loop = true;
        layerMid.loop = true;
        layerHigh.loop = true;
    }

    // ------------------------
    // BASIC CONTROLS
    // ------------------------

    public void PlayLayer(AudioSource source)
    {
        if (!source.isPlaying)
            source.Play();
    }

    public void StopLayer(AudioSource source)
    {
        source.Stop();
    }

    public void SetVolume(AudioSource source, float volume)
    {
        source.volume = Mathf.Clamp01(volume);
    }

    public void SetClip(AudioSource source, AudioClip clip, bool playImmediately = false)
    {
        source.clip = clip;

        if (playImmediately)
            source.Play();
    }

    // ------------------------
    // FADING
    // ------------------------

    public void FadeIn(AudioSource source, float duration = -1f, float targetVolume = 1f)
    {
        if (duration < 0) duration = defaultFadeDuration;
        StartCoroutine(FadeInCoroutine(source, duration, targetVolume));
    }

    public void FadeOut(AudioSource source, float duration = -1f)
    {
        if (duration < 0) duration = defaultFadeDuration;
        StartCoroutine(FadeOutCoroutine(source, duration));
    }

    private IEnumerator FadeInCoroutine(AudioSource source, float duration, float targetVolume)
    {
        if (!source.isPlaying)
            source.Play();

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

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    // ------------------------
    // OPTIONAL: QUICK ACCESS HELPERS
    // ------------------------

    public AudioSource GetLowLayer() => layerLow;
    public AudioSource GetMidLayer() => layerMid;
    public AudioSource GetHighLayer() => layerHigh;
}