using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleInteraction : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference interactAction;

    [Header("Settings")]
    public float requiredHoldTime = 5f;

    public bool isUsed = false;

    private bool playerInRange = false;
    private bool isHolding = false;
    private float holdTimer = 0f;

    private ShutdownManager shutdownManager;

    void Awake()
    {
        shutdownManager = FindAnyObjectByType<ShutdownManager>();
    }

    void OnEnable()
    {
        interactAction.action.Enable();
    }

    void OnDisable()
    {
        interactAction.action.Disable();
    }

    void Update()
    {
        // If already used, do nothing
        if (isUsed)
        {
            return;
        }
        
        // Everything below only runs if console has not been used yet

        if (!playerInRange)
            return;

        if (interactAction.action.IsPressed())
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTimer = 0f;
                shutdownManager.BeginShutdownCharge();
            }

            holdTimer += Time.deltaTime;

            float progress = holdTimer / requiredHoldTime;
            shutdownManager.UpdateShutdownProgress(progress);

            if (holdTimer >= requiredHoldTime)
            {
                isHolding = false;
                shutdownManager.CompleteShutdown();
                isUsed = true;
            }
        }
        else
        {
            if (isHolding)
            {
                isHolding = false;
                holdTimer = 0f;
                shutdownManager.CancelShutdownCharge();
            }
        }
    }

    /*void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;

        isHolding = true;
        holdTimer = 0f;

        shutdownManager.BeginShutdownCharge();
    }

    void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (!isHolding) return;

        isHolding = false;
        holdTimer = 0f;

        shutdownManager.CancelShutdownCharge();
    }
    */

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}