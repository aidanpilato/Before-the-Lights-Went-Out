using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource ambienceSource;
    public AudioSource sfxSource;

    [Header("Settings")]
    public float defaultFadeTime = 2f;

    void Awake()
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
    // AMBIENCE
    // =========================

    public void PlayAmbience(AudioClip clip, float volume = 1f)
    {
        ambienceSource.clip = clip;
        ambienceSource.volume = volume;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    public void FadeToAmbience(AudioClip newClip, float fadeTime = -1f)
    {
        if (fadeTime <= 0) fadeTime = defaultFadeTime;
        StartCoroutine(FadeAmbienceRoutine(newClip, fadeTime));
    }

    private IEnumerator FadeAmbienceRoutine(AudioClip newClip, float fadeTime)
    {
        // Fade out
        float startVolume = ambienceSource.volume;

        while (ambienceSource.volume > 0)
        {
            ambienceSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        ambienceSource.Stop();
        ambienceSource.clip = newClip;
        ambienceSource.Play();

        // Fade in
        while (ambienceSource.volume < startVolume)
        {
            ambienceSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        ambienceSource.volume = startVolume;
    }

    // =========================
    // 2D SFX
    // =========================

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    // =========================
    // 3D SFX
    // =========================

    public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1f)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 1f;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}