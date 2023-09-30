using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BTWaitSecondNode : BTActionNode
{
    UniTask t;
    public float duriation = 1f;
    public bool ignoreTimeScale;

    protected override void OnStart()
    {
        t = UniTask.Create(() => UniTask.Delay((int)(duriation * 1000), ignoreTimeScale));
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
