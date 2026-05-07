using UnityEngine;
using UnityEngine.InputSystem;

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
    private int currentClipIndex = 0;

    private AudioSource audioSource;

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
    }

    void Update()
    {
        // Smooth fade
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        // Optional: snap when very close
        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            canvasGroup.alpha = targetAlpha;

        if (!playerInRange)
            return;

        if (isPlaying)
        {
            if (!audioSource.isPlaying)
            {
                // First clip finished
                if (currentClipIndex == 0)
                {
                    currentClipIndex = 1;

                    audioSource.clip = clip2;
                    audioSource.Play();
                }
                // Second clip finished
                else
                {
                    isPlaying = false;
                    currentClipIndex = 0;

                    targetAlpha = 1f;
                }
            }

            return;
        }

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

        isPlaying = true;
        targetAlpha = 0f;

        currentClipIndex = 0;

        audioSource.clip = clip1;
        audioSource.Play();
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