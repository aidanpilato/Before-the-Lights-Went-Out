using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI")]
    public GameObject firstSelected;
    public Transform[] allButtons;

    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip hoverTick;

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Visuals")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float lerpSpeed = 10f;

    public Color normalColor = new Color(0.7f, 0.7f, 0.7f);
    public Color hoverColor = Color.white;

    private Transform currentHoverTarget;
    private bool lastWasController;
    private GameObject lastSelected;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HandleInputModeSwitch(SimpleInputMode.UsingController);
    }

    void Update()
    {
        UpdateVisuals();
    }

    // ---------------- INPUT MODE SWITCH ----------------

    void OnEnable()
    {
        SimpleInputMode.OnInputModeChanged += HandleInputModeSwitch;
        StartCoroutine(ResetMenuState());
    }

    void OnDisable()
    {
        SimpleInputMode.OnInputModeChanged -= HandleInputModeSwitch;
    }

    void HandleInputModeSwitch(bool usingController)
    {
        // Reset visuals instantly
        foreach (var b in allButtons)
        {
            if (b == null) continue;
            b.localScale = normalScale;

            var text = b.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.color = normalColor;
        }

        currentHoverTarget = null;
        lastSelected = null;

        if (usingController)
        {
            // Switching TO controller
            if (EventSystem.current.currentSelectedGameObject == null && firstSelected != null)
                Select(firstSelected);
        }
        else
        {
            // Switching TO mouse
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    IEnumerator ResetMenuState()
    {
        yield return null; // wait 1 frame for scene + EventSystem

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentHoverTarget = null;
        lastSelected = null;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        HandleInputModeSwitch(SimpleInputMode.UsingController);
    }

    // ---------------- HOVER (MOUSE) ----------------

    public void SetHover(Transform t)
    {
        if (SimpleInputMode.UsingController) return;

        if (currentHoverTarget == t) return;

        currentHoverTarget = t;
    }

    // ---------------- VISUALS ----------------

    void UpdateVisuals()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        foreach (var b in allButtons)
        {
            if (b == null) continue;

            bool isActive = SimpleInputMode.UsingController
                ? b.gameObject == selected
                : b == currentHoverTarget;

            // SCALE
            Vector3 targetScale = isActive ? hoverScale : normalScale;
            b.localScale = Vector3.Lerp(b.localScale, targetScale, Time.deltaTime * lerpSpeed);

            // COLOR
            var text = b.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                Color targetColor = isActive ? hoverColor : normalColor;
                text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * lerpSpeed);
            }

            // SOUND
            if (isActive && b.gameObject != lastSelected)
            {
                PlaySFX(hoverTick);
                lastSelected = b.gameObject;
            }
        }
    }

    // ---------------- SELECTION ----------------

    public void Select(GameObject obj)
    {
        if (obj == null) return;

        if (EventSystem.current.currentSelectedGameObject == obj) return;

        EventSystem.current.SetSelectedGameObject(obj);
    }

    // ---------------- ACTIONS ----------------

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    // ---------------- AUDIO ----------------

    void PlaySFX(AudioClip clip)
    {
        if (clip == null || uiAudioSource == null) return;
        uiAudioSource.PlayOneShot(clip);
    }
}