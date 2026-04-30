using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLaunchTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Transition Settings")]
    public float transitionDuration = 3f;

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

        StartCoroutine(Transition());
        triggered = true;
    }

    IEnumerator Transition()
    {
        TransitionController.Instance.StartFadeOut(transitionDuration);

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(nextSceneName);
    }
}