using UnityEngine;
using Cinemachine;
using System.Collections;
using StarterAssets;
using UnityEngine.Rendering.Universal;

public class MasterShutdownManager : MonoBehaviour, IShutdownHandler
{
    [Header("References")]
    public CinemachineVirtualCamera gameplayCam;
    public ThirdPersonController playerMovement;
    public GameObject[] consoleLights;
    public Material consoleLightNoPowerMaterial;

    [Header("Shutdown Settings")]
    public float fovReduction = 20f;
    public float dutchAmount = 25f;

    private float originalFOV;
    private float currentProgress = 0f;
    private bool isCharging = false;

    private LensDistortion lensDistortion;

    void Start()
    {
        originalFOV = gameplayCam.m_Lens.FieldOfView;
    }

    public void BeginShutdownCharge()
    {
        if (isCharging) return;

        isCharging = true;
        playerMovement.movementLocked = true;
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

        playerMovement.movementLocked = false;

        isCharging = false;
        currentProgress = 0f;

        // 👉 This is where your fade-to-black system will hook in later
    }
}