using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [SerializeField] private float defaultFadeTime = 2f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================
    // BASIC PLAYBACK
    // =========================

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        StopCurrentFade();

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = Mathf.Clamp01(volume);
        musicSource.Play();
    }

    public void StopMusic()
    {
        StopCurrentFade();
        musicSource.Stop();
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    // =========================
    // FADE FUNCTIONS
    // =========================

    public void FadeToMusic(AudioClip newClip, float fadeTime = -1f, bool loop = true, float targetVolume = 1f)
    {
        if (fadeTime <= 0)
            fadeTime = defaultFadeTime;

        StopCurrentFade();

        fadeCoroutine = StartCoroutine(
            FadeToMusicRoutine(newClip, fadeTime, loop, targetVolume)
        );
    }

    public void FadeOutMusic(float fadeTime = -1f)
    {
        if (fadeTime <= 0)
            fadeTime = defaultFadeTime;

        StopCurrentFade();

        fadeCoroutine = StartCoroutine(
            FadeOutRoutine(fadeTime)
        );
    }

    // =========================
    // COROUTINES
    // =========================

    private IEnumerator FadeToMusicRoutine(AudioClip newClip, float fadeTime, bool loop, float targetVolume)
    {
        float startVolume = musicSource.volume;

        // Fade out current track
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();

        // Switch track
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.volume = 0f;
        musicSource.Play();

        // Fade in new track
        while (musicSource.volume < targetVolume)
        {
            musicSource.volume += targetVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private IEnumerator FadeOutRoutine(float fadeTime)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    private void StopCurrentFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
}