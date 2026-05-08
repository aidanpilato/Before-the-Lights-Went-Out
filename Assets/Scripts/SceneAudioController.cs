using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    public AudioClip ambience;

    [Range(0f, 1f)]
    public float ambienceVolume = 1f;
    public float fadeTime = 2f;

    void Start()
    {
        AudioManager.Instance.FadeToAmbience(ambience, fadeTime, ambienceVolume);
    }
}
