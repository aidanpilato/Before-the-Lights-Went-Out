using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI")]
    public GameObject firstSelected;
    public Transform[] allButtons;

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

    // ---------------- HOVER (MOUSE) ----------------

    public void SetHover(Transform t)
    {
        if (SimpleInputMode.UsingController) return;
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
        }
    }

    // ---------------- SELECTION ----------------

    public void Select(GameObject obj)
    {
        if (obj == null) return;
        EventSystem.current.SetSelectedGameObject(obj);
    }

    // ---------------- ACTIONS ----------------

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}