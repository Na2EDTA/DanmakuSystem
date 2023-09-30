using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BTInvokeNode: BTActionNode
{
    public UnityEvent unityEvent;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        unityEvent.Invoke();
        return State.Succeeded;
    }
}