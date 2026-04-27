using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class TransitionController : MonoBehaviour
{
    public static TransitionController Instance;

    private Volume postProcessVolume;

    private Bloom bloom;
    private ColorAdjustments colorAdjustments;

    [Header("Transition Settings")]
    public float maxBloom = 20f;
    public float maxExposure = 3f;

    private float defaultBloom;
    private float defaultExposure;

    [Header("UI Fade")]
    public Image fadePanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPostProcessing();
    }

    void FindPostProcessing()
    {
        postProcessVolume = FindAnyObjectByType<Volume>();

        if (postProcessVolume == null) return;

        var profile = postProcessVolume.profile;

        if (profile.TryGet(out bloom))
        {
            defaultBloom = bloom.intensity.value;
        }

        if (profile.TryGet(out colorAdjustments))
        {
            defaultExposure = colorAdjustments.postExposure.value;
        }
    }

    public void StartFadeOut(float duration)
    {
        StartCoroutine(FadeTransition(
            defaultBloom, maxBloom,
            defaultExposure, maxExposure,
            0f, 1f, // panel: transparent → white
            duration));
    }

    public void StartFadeIn(float duration)
    {
        StartCoroutine(FadeTransition(
            maxBloom, defaultBloom,
            maxExposure, defaultExposure,
            1f, 0f, // panel: white → transparent
            duration));
    }

    IEnumerator FadeTransition(
    float bloomStart, float bloomEnd,
    float exposureStart, float exposureEnd,
    float panelStart, float panelEnd,
    float duration)
    {
        if (bloom == null || colorAdjustments == null) yield break;

        float time = 0f;

        Color panelColor = fadePanel != null ? fadePanel.color : Color.white;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float easedT = Mathf.SmoothStep(0f, 1f, t);

            // Post-processing
            bloom.intensity.value = Mathf.Lerp(bloomStart, bloomEnd, easedT);
            colorAdjustments.postExposure.value = Mathf.Lerp(exposureStart, exposureEnd, easedT);

            // Panel (direct + predictable)
            if (fadePanel != null)
            {
                float panelT = Mathf.Clamp01((t - 0.15f) / 0.85f);
                float easedPanelT = Mathf.SmoothStep(0f, 1f, panelT);
                float alpha = Mathf.Lerp(panelStart, panelEnd, easedPanelT);

                fadePanel.color = new Color(
                    panelColor.r,
                    panelColor.g,
                    panelColor.b,
                    alpha
                );
            }

            yield return null;
        }

        // Final values
        bloom.intensity.value = bloomEnd;
        colorAdjustments.postExposure.value = exposureEnd;

        if (fadePanel != null)
        {
            fadePanel.color = new Color(
                panelColor.r,
                panelColor.g,
                panelColor.b,
                panelEnd
            );
        }
    }
}