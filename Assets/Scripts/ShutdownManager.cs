using UnityEngine;
using Cinemachine;
using System.Collections;
using StarterAssets;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class ShutdownManager : MonoBehaviour, IShutdownHandler
{
    [Header("References")]
    public GameObject memoryRoot;
    public GameObject industrialRoot;
    public Volume postProcess;
    public CinemachineVirtualCamera gameplayCam;
    public ThirdPersonController playerController; // drag your movement script here
    public Animator playerAnimator;
    public GameObject[] consoleLights;
    public Material consoleLightNoPowerMaterial;
    public AudioClip industrialAmbience;
    public AudioClip shutdownSound;
    public AudioSource sfxSource;
    public Canvas promptCanvas;

    [Header("Shutdown Settings")]
    public float shutdownDuration = 5f;
    public float fovReduction = 20f;
    public float dutchAmount = 25f;

    private float originalFOV;
    private float currentProgress = 0f;
    private bool shuttingDown = false;
    private bool isCharging = false;
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        originalFOV = gameplayCam.m_Lens.FieldOfView;
        postProcess.profile.TryGet(out LensDistortion lensDistortion);
        postProcess.profile.TryGet(out ColorAdjustments colorAdjustments);
        this.lensDistortion = lensDistortion;
        this.colorAdjustments = colorAdjustments;
    }

    public void BeginShutdown()
    {
        if (shuttingDown) return;

        shuttingDown = true;
        playerController.movementLocked = true;

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

            lensDistortion.intensity.value =
                Mathf.Lerp(0f, -1f, EvaluateDistortion(t));

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
        lensDistortion.intensity.value = 0f;

        // Kill memory world instantly
        memoryRoot.SetActive(false);

        shuttingDown = false;
    }

    public void BeginShutdownCharge()
    {
        if (isCharging) return;

        isCharging = true;
        playerController.movementLocked = true;
    }

    public void UpdateShutdownProgress(float progress)
    {
        currentProgress = Mathf.Clamp01(progress);

        float curve = Mathf.Pow(currentProgress, 2f);

        gameplayCam.m_Lens.FieldOfView =
            Mathf.Lerp(originalFOV, originalFOV - fovReduction, curve);

        gameplayCam.m_Lens.Dutch =
            Mathf.Lerp(0f, dutchAmount, curve);
        
        float t = currentProgress;
        
        lensDistortion.intensity.value =
            Mathf.Lerp(0f, -1f, EvaluateDistortion(t));
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
        float startLensDistortion = lensDistortion.intensity.value;

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
            
            lensDistortion.intensity.value = 
                Mathf.Lerp(startLensDistortion, 0f, t);

            yield return null;
        }

        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;
        lensDistortion.intensity.value = 0f;

        playerController.movementLocked = false;
    }

    public void CompleteShutdown()
    {
        StopAllCoroutines();

        // Activate industrial world
        industrialRoot.SetActive(true);

        // Kill memory world
        memoryRoot.SetActive(false);

        // HARD SNAP CAMERA RESET
        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;
        lensDistortion.intensity.value = 0f;

        // Adjust post-processing for industrial look
        colorAdjustments.postExposure.value = -1.72f;
        Debug.Log("Post Exposure set to: " + colorAdjustments.postExposure.value);

        // Disable prompt canvas
        if (promptCanvas != null)
        {
            promptCanvas.enabled = false;
        }
        
        // Change material of console lights
        for (int i = 0; i < consoleLights.Length; i++)
        {
            MeshRenderer rend = consoleLights[i].GetComponent<MeshRenderer>();
            if (rend != null)
            {
                rend.material = consoleLightNoPowerMaterial; // assign this in inspector
            }
        }

        // Play shutdown sound
        if (sfxSource != null && shutdownSound != null)
        {
            sfxSource.PlayOneShot(shutdownSound);
        }
        else
        {
            Debug.LogWarning("SFX Source or Shutdown Sound not set!");
        }

        //DynamicGI.UpdateEnvironment();

        Ch3LightingManager ch3Lighting = GetComponent<Ch3LightingManager>();
        if (ch3Lighting != null)
        {
            ch3Lighting.ApplyIndustrialLighting();
            Debug.Log("Applied ch3 industrial lighting");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CutToAmbience(industrialAmbience);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }

        // If current scene not Chapter3, enable sprinting
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Chapter3")
        {
            playerController.sprintEnabled = true;
            Debug.Log("Sprinting enabled: " + playerController.sprintEnabled);
        }
        
        // Restore player control
        playerController.movementLocked = false;

        // Reset state
        isCharging = false;
        currentProgress = 0f;
    }

    float EvaluateDistortion(float t)
    {
        float distortionStart = 0.9f;
        float distortionT = Mathf.InverseLerp(distortionStart, 1f, t);
        distortionT = Mathf.Clamp01(distortionT);
        return Mathf.Pow(distortionT, 3f);
    }
}