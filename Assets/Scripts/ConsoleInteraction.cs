using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleInteraction : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference primaryInteractAction;

    [Header("Settings")]
    public float requiredHoldTime = 5f;

    public bool isUsed = false;

    private bool playerInRange = false;
    private bool isHolding = false;
    private float holdTimer = 0f;

    [SerializeField] private MonoBehaviour shutdownHandlerObject;
    private IShutdownHandler shutdownHandler;

    void Awake()
    {
        shutdownHandler = shutdownHandlerObject as IShutdownHandler;
    }

    void OnEnable()
    {
        primaryInteractAction.action.Enable();
    }

    void OnDisable()
    {
        primaryInteractAction.action.Disable();
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

        if (primaryInteractAction.action.IsPressed())
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTimer = 0f;
                shutdownHandler.BeginShutdownCharge();
            }

            holdTimer += Time.deltaTime;

            float progress = holdTimer / requiredHoldTime;
            shutdownHandler.UpdateShutdownProgress(progress);

            if (holdTimer >= requiredHoldTime)
            {
                isHolding = false;
                shutdownHandler.CompleteShutdown();
                isUsed = true;
            }
        }
        else
        {
            if (isHolding)
            {
                isHolding = false;
                holdTimer = 0f;
                shutdownHandler.CancelShutdownCharge();
            }
        }
    }

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