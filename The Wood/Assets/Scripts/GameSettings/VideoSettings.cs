using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSettings
{
    public Display[] GetDisplays()
    {
        return Display.displays;
    }

    public void SetDisplay(int displayIndex)
    {
        if (Display.displays.Length > displayIndex)
            Display.displays[displayIndex].Activate();
        else
            Debug.LogError("displayIndex [" + displayIndex + "] out of range: " + Display.displays.Length);

    }
}
