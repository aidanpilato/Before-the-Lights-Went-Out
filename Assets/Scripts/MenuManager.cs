using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI")]
    public GameObject firstSelected;

    [Header("Menu Actions")]
    public string gameSceneName = "GameScene";

    [Header("Hover Settings")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float lerpSpeed = 10f;

    [Header("Menu Buttons")]
    public Transform[] allButtons;

    private Transform currentHoverTarget;
    private bool lastWasController;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        lastWasController = SimpleInputMode.UsingController;
        Select(firstSelected);
    }

    void Update()
    {
        HandleInputModeSwitch();
        UpdateButtonScales();
    }

    // ---------------- SELECTION ----------------

    public void Select(GameObject obj)
    {
        if (obj == null) return;
        EventSystem.current.SetSelectedGameObject(obj);
    }

    void HandleInputModeSwitch()
    {
        bool usingController = SimpleInputMode.UsingController;
        if (usingController == lastWasController) return;

        lastWasController = usingController;

        // Reset all buttons to normal scale immediately on any mode switch
        foreach (var b in allButtons)
        {
            if (b != null)
                b.localScale = normalScale;
            
            var pointer = b.GetComponent<MenuButtonPointer>();
            if (pointer != null)
                pointer.enabled = !usingController;
        }

        currentHoverTarget = null;
        
        if (!usingController)
        {
            // Switched TO MOUSE: clear controller selection
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            // Switched TO CONTROLLER: restore selection
            if (EventSystem.current.currentSelectedGameObject == null && firstSelected != null)
                Select(firstSelected);
        }
    }

    // ---------------- HOVER ----------------

    public void SetHover(Transform t)
    {
        if (SimpleInputMode.UsingController) return;
        currentHoverTarget = t;
    }

    public void ClearHover(Transform t)
    {
        // Only clear if this button is actually the current hover target,
        // preventing a late-exiting button from wiping another button's hover
        if (t == currentHoverTarget)
            currentHoverTarget = null;
    }

    void UpdateButtonScales()
    {
        foreach (var b in allButtons)
        {
            if (b == null) continue;

            bool isActive = SimpleInputMode.UsingController
                ? b.gameObject == EventSystem.current.currentSelectedGameObject
                : b == currentHoverTarget;

            Vector3 target = isActive ? hoverScale : normalScale;
            b.localScale = Vector3.Lerp(b.localScale, target, Time.deltaTime * lerpSpeed);
        }
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