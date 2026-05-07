using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class EndingScreenController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup textGroup;   // assign your text CanvasGroup
    public float fadeDuration = 1f;

    [Header("Timing")]
    public float holdTime = 10f;
    public float finalDelay = 1.5f;

    public void StartEndingSequence()
    {
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // Ensure starting state
        textGroup.alpha = 0f;
        textGroup.gameObject.SetActive(true);

        // SHORT DELAY
        yield return new WaitForSeconds(finalDelay);

        // FADE IN
        yield return StartCoroutine(Fade(textGroup, 0f, 1f, fadeDuration));

        // HOLD
        yield return new WaitForSeconds(holdTime);

        // FADE OUT
        yield return StartCoroutine(Fade(textGroup, 1f, 0f, fadeDuration));

        // SHORT DELAY
        yield return new WaitForSeconds(finalDelay);

        // LOAD MAIN MENU
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator Fade(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0f;
        cg.alpha = start;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }

        cg.alpha = end;
    }
}