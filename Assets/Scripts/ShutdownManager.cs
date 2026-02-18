using UnityEngine;
using Cinemachine;
using System.Collections;
using StarterAssets;

public class ShutdownManager : MonoBehaviour
{
    [Header("References")]
    public GameObject memoryRoot;
    public CinemachineVirtualCamera gameplayCam;
    public ThirdPersonController playerMovement; // drag your movement script here
    public Animator playerAnimator; // optional, for shutdown animation

    [Header("Shutdown Settings")]
    public float shutdownDuration = 5f;
    public float fovReduction = 20f;
    public float dutchAmount = 25f;

    private float originalFOV;
    private float currentProgress = 0f;
    private bool shuttingDown = false;
    private bool isCharging = false;

    void Start()
    {
        originalFOV = gameplayCam.m_Lens.FieldOfView;
    }

    public void BeginShutdown()
    {
        if (shuttingDown) return;

        shuttingDown = true;
        playerMovement.movementLocked = true;

        StartCoroutine(ShutdownSequence());
    }

    IEnumerator ShutdownSequence()
    {
        float time = 0f;

        while (time < shutdownDuration)
        {
            time += Time.deltaTime;
            float t = time / shutdownDuration;

            // Slight curve for tension build
            float curve = Mathf.Pow(t, 2f);

            gameplayCam.m_Lens.FieldOfView =
                Mathf.Lerp(originalFOV, originalFOV - fovReduction, curve);

            gameplayCam.m_Lens.Dutch =
                Mathf.Lerp(0f, dutchAmount, curve);

            yield return null;
        }

        HardPowerCut();
    }

    void HardPowerCut()
    {
        StopAllCoroutines();

        // Instant reset
        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;

        // Kill memory world instantly
        memoryRoot.SetActive(false);

        shuttingDown = false;
    }

    public void BeginShutdownCharge()
    {
        if (isCharging) return;

        isCharging = true;
        playerMovement.movementLocked = true;

        // Force idle animation
        //playerAnimator.SetFloat("Speed", 0f);
        //playerAnimator.SetBool("IsMoving", false);
    }

    public void UpdateShutdownProgress(float progress)
    {
        currentProgress = Mathf.Clamp01(progress);

        float curve = Mathf.Pow(currentProgress, 2f);

        gameplayCam.m_Lens.FieldOfView =
            Mathf.Lerp(originalFOV, originalFOV - fovReduction, curve);

        gameplayCam.m_Lens.Dutch =
            Mathf.Lerp(0f, dutchAmount, curve);
    }

    public void CancelShutdownCharge()
    {
        if (!isCharging) return;

        isCharging = false;
        currentProgress = 0f;

        StartCoroutine(RestoreCamera());
    }

    IEnumerator RestoreCamera()
    {
        float startFOV = gameplayCam.m_Lens.FieldOfView;
        float startDutch = gameplayCam.m_Lens.Dutch;

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            gameplayCam.m_Lens.FieldOfView =
                Mathf.Lerp(startFOV, originalFOV, t);

            gameplayCam.m_Lens.Dutch =
                Mathf.Lerp(startDutch, 0f, t);

            yield return null;
        }

        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;

        playerMovement.movementLocked = false;
    }

    public void CompleteShutdown()
    {
        StopAllCoroutines();

        // HARD SNAP CAMERA RESET
        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;

        // Kill memory world
        memoryRoot.SetActive(false);

        // Restore player control
        playerMovement.movementLocked = false;

        // Reset state
        isCharging = false;
        currentProgress = 0f;
    }
}