using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWaitFrameNode: BTActionNode
{
    UniTask t;
    public int duriation = 60;

    protected override void OnStart()
    {
        t = UniTask.Create(() => UniTask.DelayFrame(duriation));
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (t.Status == UniTaskStatus.Succeeded)
            return State.Succeeded;
        else if (t.Status == UniTaskStatus.Faulted)
            return State.Failed;
        else
            return State.Running;
    }
}