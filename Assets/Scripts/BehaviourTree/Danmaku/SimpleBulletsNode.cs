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
    public string num, interval, angle, angleSpread, 
        speedStart, speedEnd, aim, maxSpeed, acceleration, rotation;
    float _angle, _angleSpread,
        _speedStart, _speedEnd, _aim, _maxSpeed, _acceleration, _rotation;
    int _num, _interval;
    public RelativeTo relativeTo;

    protected override void OnStart()
    {
        if (relativeTo == RelativeTo.Local) bulletPosition +=
        (Vector2)tree.runtime.gameObject.transform.position;

        BTBlackboard b = tree.blackboard;

        b.Parse(num, out _num);
        b.Parse(interval, out _interval);

        b.Parse(angle, out _angle);
        b.Parse(angleSpread, out _angleSpread);

        b.Parse(speedStart, out _speedStart);
        b.Parse(speedEnd, out _speedEnd);

        b.Parse(aim, out _aim);
        b.Parse(maxSpeed, out _maxSpeed);
        b.Parse(acceleration, out _acceleration);
        b.Parse(rotation, out _rotation);

        t = UniTask.Create(async () => await DanmakuEmission.CreateSimpleBulletsAsync(_num,
             _interval, _angle, _angleSpread, _speedStart, _speedEnd, style,
            bulletPosition, _aim, _maxSpeed, _acceleration, _rotation));
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