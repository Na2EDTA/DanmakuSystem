using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNode: BTActionNode
{
    public string bulletName;
    public Vector2 bulletPosition;
    public List<float> arguments;
    protected override void OnStart()
    {
        Pool.instance.Create(bulletName, bulletPosition, arguments.ToArray());
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return State.Succeeded;
    }
}