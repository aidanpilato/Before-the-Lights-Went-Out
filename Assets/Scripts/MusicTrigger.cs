using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool loop = true;
    [SerializeField] private float volume = 1f;

    [Header("Trigger")]
    [SerializeField] private Transform player;

    private bool triggered = false;

    void Update()
    {
        if (triggered || player == null || musicClip == null)
            return;

        if (player.position.x >= transform.position.x)
        {
            triggered = true;

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayMusic(musicClip, loop, volume);
            }
        }
    }
}