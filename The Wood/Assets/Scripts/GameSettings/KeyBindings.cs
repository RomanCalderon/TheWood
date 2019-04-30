using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindings
{
    public static KeyCode Interact = KeyCode.E;
    public static KeyCode ToggleMenuSet = KeyCode.Tab;
    public static KeyCode ActionOne = KeyCode.Mouse0;
    public static KeyCode ActionTwo = KeyCode.Mouse1;
    public static KeyCode Sprint = KeyCode.LeftShift;

    public static string GetFormat(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Mouse0)
            return "LMB";
        else if (keyCode == KeyCode.Mouse1)
            return "RMB";
        else if (keyCode == KeyCode.Mouse2)
            return "MMB";

        return keyCode.ToString().ToUpper();
    }
}
