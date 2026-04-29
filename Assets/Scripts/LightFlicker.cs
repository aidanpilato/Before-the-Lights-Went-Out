using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    private Light lightSource;

    [Header("Flicker Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    [Tooltip("How fast the light flickers")]
    public float flickerSpeed = 0.05f;

    [Tooltip("How random the flicker feels (0 = smooth, higher = chaotic)")]
    public float randomness = 1f;

    private float targetIntensity;
    private float timer;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        targetIntensity = lightSource.intensity;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Pick a new random intensity
            targetIntensity = Random.Range(minIntensity, maxIntensity);

            // Reset timer with randomness influence
            timer = flickerSpeed * Random.Range(0.5f, 1.5f) / Mathf.Max(0.01f, randomness);
        }

        // Smoothly move toward target intensity
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, Time.deltaTime * 10f);
    }
}