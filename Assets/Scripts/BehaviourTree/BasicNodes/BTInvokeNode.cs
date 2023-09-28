using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BTInvokeNode: BTActionNode
{
    public UnityEvent unityEvent;
    protected override void OnStart()
    {
        //do something...
    }

    protected override void OnStop()
    {
        //do something...
    }

    protected override State OnUpdate()
    {
        return State.Succeeded;
        //do something...
    }
}