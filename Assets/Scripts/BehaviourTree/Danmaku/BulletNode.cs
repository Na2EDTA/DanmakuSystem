using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNode: BTActionNode
{
    public string bulletName; 
    public RelativeTo relativeTo;
    public Vector2 bulletPosition;
    public List<float> arguments;
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        Pool.instance.Create(bulletName, bulletPosition, arguments.ToArray());
        return State.Succeeded;
    }

    [Serializable]
    public enum RelativeTo
    {
        Local, World
    }

}