using UnityEngine;

public class Ch3LightingManager : MonoBehaviour
{
    private ShutdownManager shutdownManager;

    [Header("Directional Lights")]
    public Light memoryDirectionalLight;
    public Light industrialDirectionalLight;

    [Header("Skybox")]
    public Material industrialSkybox;

    [Header("Fog Settings")]
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;

    void Start()
    {
        shutdownManager = GetComponent<ShutdownManager>();
    }

    public void ApplyIndustrialLighting()
    {
        Debug.Log("Applying industrial lighting settings...");

        // Swap directional lights
        if (memoryDirectionalLight != null)
            memoryDirectionalLight.enabled = false;

        if (industrialDirectionalLight != null)
            industrialDirectionalLight.enabled = true;

        // Swap skybox
        if (industrialSkybox != null)
            RenderSettings.skybox = industrialSkybox;
        
        // Apply fog settings
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}