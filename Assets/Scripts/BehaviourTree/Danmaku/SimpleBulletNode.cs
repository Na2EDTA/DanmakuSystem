using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleBulletNode: BTActionNode
{
    public BulletStyle style;
    public RelativeTo relativeTo;
    public Vector2 bulletPosition;
    public string angle, speed, aim, maxSpeed, acceleration, rotation;
    float _angle, _speed, _aim, _maxSpeed, _acceleration, _rotation;

    protected override void OnStart()
    {
        if (relativeTo == RelativeTo.Local) bulletPosition +=
        (Vector2) tree.runtime.gameObject.transform.position;

        BTBlackboard b = tree.blackboard;
        b.Parse(angle, out _angle);
        b.Parse(speed, out _speed);
        b.Parse(aim, out _aim);
        b.Parse(maxSpeed, out _maxSpeed);
        b.Parse(acceleration, out _acceleration);
        b.Parse(rotation, out _rotation);
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        DanmakuEmission.CreateSimpleBullet(style, bulletPosition,
               _angle, _speed, _aim, _maxSpeed, _acceleration, _rotation);
        return State.Succeeded;
    }

    [Serializable]
    public enum RelativeTo
    {
        Local, World
    }
}