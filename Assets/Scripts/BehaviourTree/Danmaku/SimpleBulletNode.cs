using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletNode: BTActionNode
{
    public BulletStyle style;
    public Vector2 bulletPosition;
    public float angle, speed, aim, maxSpeed, acceleration, rotation;
    
    protected override void OnStart()
    {
        DanmakuEmission.CreateSimpleBullet(style, bulletPosition, 
            angle, speed, aim, maxSpeed, acceleration, rotation);
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return State.Succeeded;
    }
}