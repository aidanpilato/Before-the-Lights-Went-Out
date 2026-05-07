using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioLogInteraction : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip clip1;
    [SerializeField] private AudioClip clip2;

    [Header("Input")]
    public InputActionReference secondaryInteractAction;

    [Header("UI")]
    public GameObject promptUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 10f;

    private bool playerInRange = false;
    private bool isPlaying = false;
    private float targetAlpha = 0f;

    private AudioSource audioSource;
    private Coroutine playbackRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        secondaryInteractAction.action.Enable();
    }

    void OnDisable()
    {
        secondaryInteractAction.action.Disable();

        if (audioSource != null)
            audioSource.Stop();

        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);

        isPlaying = false;
    }

    void Update()
    {
        // Smooth fade
        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.deltaTime * fadeSpeed
        );

        // Snap when very close
        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            canvasGroup.alpha = targetAlpha;

        if (!playerInRange || isPlaying)
            return;

        // Interaction
        if (secondaryInteractAction.action.WasPressedThisFrame())
        {
            PlayAudioLog();
        }
    }

    void PlayAudioLog()
    {
        if (clip1 == null || clip2 == null)
        {
            Debug.LogWarning("Missing audio clips on " + gameObject.name);
            return;
        }

        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);

        playbackRoutine = StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;
        targetAlpha = 0f;

        // Play first clip
        audioSource.clip = clip1;
        audioSource.Play();

        yield return WaitForClipToFinish();

        // Play second clip
        audioSource.clip = clip2;
        audioSource.Play();

        yield return WaitForClipToFinish();

        // Finished sequence
        isPlaying = false;

        if (playerInRange)
            targetAlpha = 1f;
    }

    IEnumerator WaitForClipToFinish()
    {
        while (audioSource.isPlaying || AudioListener.pause)
        {
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!isPlaying)
                targetAlpha = 1f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            targetAlpha = 0f;
        }
    }
}