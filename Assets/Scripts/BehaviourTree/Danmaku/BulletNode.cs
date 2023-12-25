using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNode: BTActionNode
{
    public string bulletName; 
    public RelativeTo relativeTo;
    public Vector2 bulletPosition;
    public List<string> arguments;
    List<float> args;
    [SerializeField] GameObject bullet;
    string type;

    protected override void OnStart()
    {
        if (relativeTo == RelativeTo.Local) bulletPosition += 
                (Vector2) tree.runtime.gameObject.transform.position;

        BTBlackboard b = tree.blackboard;

        for (int i = 0; i < arguments.Count; i++)
        {
            float temp;
            b.Parse(arguments[i], out temp);
            args[i] = temp;
        }

        type = bullet.GetComponent<DanmakuObject>().GetType().Name;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        bullet = Pool.instance.Create(bulletPosition, type, default, default, args.ToArray());
        return State.Succeeded;
    }

    [Serializable]
    public enum RelativeTo
    {
        Local, World
    }
}