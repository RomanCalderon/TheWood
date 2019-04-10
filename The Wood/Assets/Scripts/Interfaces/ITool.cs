using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ToolActionHandler();

public interface ITool
{
    event ToolActionHandler OnPerformAction;

    void PerformAction();
}
