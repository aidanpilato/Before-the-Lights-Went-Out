using UnityEngine;

public class MusicFadeTrigger : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private float fadeTime = 3f;

    [Header("Trigger")]
    [SerializeField] private Transform player;

    private bool triggered = false;

    void Update()
    {
        if (triggered || player == null)
            return;

        if (player.position.x >= transform.position.x)
        {
            triggered = true;

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.FadeOutMusic(fadeTime);
            }
        }
    }
}