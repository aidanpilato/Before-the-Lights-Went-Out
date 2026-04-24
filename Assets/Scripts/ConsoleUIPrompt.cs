using Cinemachine;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUIPrompt : MonoBehaviour
{
    public TMPro.TextMeshProUGUI actionText;
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite keyboardSprite;   // E
    public Sprite controllerSprite; // RT

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

        if (SimpleInputMode.UsingController)
        {
            buttonImage.sprite = controllerSprite;
        }
        else
        {
            buttonImage.sprite = keyboardSprite;
        }
    }
}