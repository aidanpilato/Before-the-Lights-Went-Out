using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    public AudioClip ambience;
    public float fadeTime = 2f;

    void Start()
    {
        AudioManager.Instance.FadeToAmbience(ambience, fadeTime);
    }
}
