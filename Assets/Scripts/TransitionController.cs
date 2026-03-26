using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;

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
            duration));
    }

    public void StartFadeIn(float duration)
    {
        StartCoroutine(FadeTransition(
            maxBloom, defaultBloom,
            maxExposure, defaultExposure,
            duration));
    }

    IEnumerator FadeTransition(
        float bloomStart, float bloomEnd,
        float exposureStart, float exposureEnd,
        float duration)
    {
        if (bloom == null || colorAdjustments == null) yield break;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            bloom.intensity.value = Mathf.Lerp(bloomStart, bloomEnd, t);
            colorAdjustments.postExposure.value = Mathf.Lerp(exposureStart, exposureEnd, t);

            yield return null;
        }

        bloom.intensity.value = bloomEnd;
        colorAdjustments.postExposure.value = exposureEnd;
    }
}