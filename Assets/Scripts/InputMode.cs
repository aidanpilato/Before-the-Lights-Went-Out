using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleInputMode : MonoBehaviour
{
    public static bool UsingController;

    void Update()
    {
        // Check last used device
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            UsingController = true;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame || 
            Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            UsingController = false;
        }
    }
}