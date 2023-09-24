using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTLogNode: BTActionNode
{
    [SerializeField] string startLog;
    [SerializeField] string stopLog;
    [SerializeField] string updateLog;
    protected override void OnStart()
    {
        if (startLog != "")
            Debug.Log(startLog);
    }

    protected override void OnStop()
    {
        if (stopLog != "")
            Debug.Log(stopLog);
    }

    protected override State OnUpdate()
    {
        if (updateLog != "")
            Debug.Log(updateLog);
        return State.Succeeded;
    }
}