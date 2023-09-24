using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWaitNode : BTActionNode
{
    public float duriation = 60;
    float startTime;
    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        //暂停时的解决方法......

        if (Time.time - startTime > duriation / GameManager.instance.fps && GameManager.instance.fps!=0) 
            return State.Succeeded;
        return State.Running;
    }
}
