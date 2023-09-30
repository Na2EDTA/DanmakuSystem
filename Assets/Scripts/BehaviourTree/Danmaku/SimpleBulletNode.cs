using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleBulletNode: BTActionNode
{
    public BulletStyle style;
    public RelativeTo relativeTo;
    public Vector2 bulletPosition;
    public float angle, speed, aim, maxSpeed, acceleration, rotation;
    
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        DanmakuEmission.CreateSimpleBullet(style, bulletPosition,
               angle, speed, aim, maxSpeed, acceleration, rotation);
        return State.Succeeded;
    }

    [Serializable]
    public enum RelativeTo
    {
        Local, World
    }
}