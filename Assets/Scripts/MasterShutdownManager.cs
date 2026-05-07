using UnityEngine;
using Cinemachine;
using System.Collections;
using StarterAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class MasterShutdownManager : MonoBehaviour, IShutdownHandler
{
    [Header("References")]
    public CinemachineVirtualCamera gameplayCam;
    public ThirdPersonController playerController;
    public GameObject[] consoleLights;
    public Material consoleLightNoPowerMaterial;
    public GameObject endPanel;
    public Volume postProcess;
    public PauseMenu pauseManager;
    public EndingScreenController endingScreenController;

    [Header("Shutdown Settings")]
    public float fovReduction = 20f;
    public float dutchAmount = 25f;
    public float maxVignette = 0.6f;

    private float originalFOV;
    private float originalVignette;
    private float currentProgress = 0f;
    private bool isCharging = false;

    private Vignette vignette;

    void Start()
    {
        originalFOV = gameplayCam.m_Lens.FieldOfView;
        postProcess.profile.TryGet(out Vignette vignette);
        this.vignette = vignette;
        originalVignette = vignette.intensity.value;
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
        
        vignette.intensity.value = 
            Mathf.Lerp(originalVignette, maxVignette, curve);
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
            
            vignette.intensity.value = 
                Mathf.Lerp(vignette.intensity.value, originalVignette, t);

            yield return null;
        }

        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;
        vignette.intensity.value = originalVignette;

        playerController.movementLocked = false;
    }

    public void CompleteShutdown()
    {
        pauseManager.canPause = false;
        
        StopAllCoroutines();

        // HARD SNAP (important for your design language)
        gameplayCam.m_Lens.FieldOfView = originalFOV;
        gameplayCam.m_Lens.Dutch = 0f;

        // Kill console lights
        for (int i = 0; i < consoleLights.Length; i++)
        {
            MeshRenderer rend = consoleLights[i].GetComponent<MeshRenderer>();
            if (rend != null)
            {
                rend.material = consoleLightNoPowerMaterial;
            }
        }

        playerController.movementLocked = true;

        isCharging = false;
        currentProgress = 0f;

        // Show end panel
        endPanel.SetActive(true);
        endingScreenController.StartEndingSequence();
    }
}