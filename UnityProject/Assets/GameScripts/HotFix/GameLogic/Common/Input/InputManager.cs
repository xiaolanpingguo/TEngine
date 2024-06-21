using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class InputManager
{
    public static InputActions InputActions { get; private set; }

    public static void Initialize()
    {
        if (InputActions == null)
        {
            InputActions = new InputActions();
            InputActions.Enable();
        }
    }

    public static void UnInitialize()
    {
        if (InputActions != null)
        {
            InputActions.Disable();
            InputActions.Dispose();
            InputActions = null;
        }
    }
}