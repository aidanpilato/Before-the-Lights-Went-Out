using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class AudioLogInteraction : MonoBehaviour
{
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
                isPlaying = false;
                targetAlpha = 1f;
            }
            return;
        }

        if (secondaryInteractAction.action.WasPressedThisFrame())
        {
            PlayAudioLog();
        }
    }

    void PlayAudioLog()
    {
        isPlaying = true;
        targetAlpha = 0f;

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