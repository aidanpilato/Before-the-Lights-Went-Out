using UnityEngine;
using UnityEngine.UI;

public class InputSpriteSwitcher : MonoBehaviour
{
    public Image targetImage;

    public Sprite keyboardSprite;
    public Sprite controllerSprite;

    void OnEnable()
    {
        SimpleInputMode.OnInputModeChanged += UpdateSprite;

        // Set correct sprite immediately
        UpdateSprite(SimpleInputMode.UsingController);
    }

    void OnDisable()
    {
        SimpleInputMode.OnInputModeChanged -= UpdateSprite;
    }

    void UpdateSprite(bool usingController)
    {
        targetImage.sprite = usingController ? controllerSprite : keyboardSprite;
    }
}