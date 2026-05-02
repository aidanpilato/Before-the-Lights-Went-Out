using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SimpleInputMode : MonoBehaviour
{
    public static bool UsingController;

    public static event Action<bool> OnInputModeChanged;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        bool previous = UsingController;

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            UsingController = true;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame || 
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f)
        {
            UsingController = false;
        }

        // Only fire event if it actually changed
        if (previous != UsingController)
        {
            OnInputModeChanged?.Invoke(UsingController);
        }
    }
}