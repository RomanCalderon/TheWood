using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseController
{
    public static bool IsPaused { get; private set; }

    public delegate void PauseEventHandler(bool paused);
    public static event PauseEventHandler OnPauseEvent;
    
    #region Events

    public static void PauseGame()
    {
        OnPauseEvent?.Invoke(IsPaused = true);
    }

    public static void ResumeGame()
    {
        OnPauseEvent?.Invoke(IsPaused = false);
    }

    #endregion
}
