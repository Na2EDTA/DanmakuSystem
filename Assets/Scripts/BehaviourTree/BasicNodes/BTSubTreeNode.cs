using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSubTreeNode : BTActionNode
{
    public BTTree subTree;
    private BTTree subTreeClone;
    protected override void OnStart()
    {
        subTreeClone=subTree.Clone();
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        State SubTreeState=subTreeClone.state;
        return SubTreeState;
    }
}
