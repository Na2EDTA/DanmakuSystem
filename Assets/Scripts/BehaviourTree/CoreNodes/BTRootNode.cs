using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRootNode : BTNode
{
    [HideInInspector] public BTNode child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    public override BTNode Clone()
    {
        BTRootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }
}
