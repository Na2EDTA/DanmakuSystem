using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SimpleBulletsNode: BTActionNode
{   
    UniTask t;
    public BulletStyle style;
    public Vector2 bulletPosition;
    public int num, interval;
    public float angle, angleSpread, 
        speedStart, speedEnd, aim, maxSpeed, acceleration, rotation;
    public RelativeTo relativeTo;

    protected override void OnStart()
    {
        t = UniTask.Create(async () => await DanmakuEmission.CreateSimpleBulletsAsync(num,
            interval, angle, angleSpread, speedStart, speedEnd, style,
            bulletPosition, aim, maxSpeed, acceleration, rotation));
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

    [Serializable]
    public enum RelativeTo:byte
    {
        Local, World
    }
}