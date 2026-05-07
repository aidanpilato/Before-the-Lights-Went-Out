using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLaunchTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Transition Settings")]
    public float transitionDuration = 3f;

    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip uiSelect;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition());
        }
    }

    public void StartTransition()
    {
        if (triggered) return;
        triggered = true;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // Play UI click once (Play button feedback)
            if (uiAudioSource != null && uiSelect != null)
            {
                uiAudioSource.PlayOneShot(uiSelect);
            }
        }
        
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        TransitionController.Instance.StartFadeOut(transitionDuration);

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(nextSceneName);
    }
}