using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletsNode: BTActionNode
{
    public BulletStyle style;
    public RelativeTo relativeTo;
    public Vector2 bulletPosition;
    public int num, interval;
    public float angle, angleSpread, 
        speedStart, speedEnd, aim, maxSpeed, acceleration, rotation;

    protected override void OnStart()
    {
        DanmakuEmission.CreateSimpleBulletsAsync(num, interval, angle, angleSpread, speedStart, speedEnd, style, bulletPosition, aim, maxSpeed, acceleration, rotation);
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

    [Serializable]
    public enum RelativeTo
    {
        Local, World
    }
}