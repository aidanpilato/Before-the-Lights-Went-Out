using UnityEngine;
using UnityEngine.UI;

public class ConsoleUIPrompt : MonoBehaviour
{
    [Header("Distance Fade")]
    public Transform player;
    public float maxDistance = 5f;
    public float minDistance = 2f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // Distance fade
        float dist = Vector3.Distance(player.position, transform.position);
        float alpha = Mathf.InverseLerp(maxDistance, minDistance, dist);
        canvasGroup.alpha = alpha;
    }
}