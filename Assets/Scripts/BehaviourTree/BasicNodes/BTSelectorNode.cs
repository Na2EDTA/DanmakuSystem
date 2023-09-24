using System;
using System.Collections.Generic;
using UnityEngine;

public class BTSelectorNode : BTCompositeNode
{
    public string intVariableName;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        BTNode child = children[tree.blackboard.FindIntegerVariable(intVariableName)];
        return child.Update() switch
        {
            State.Running => State.Running,
            State.Succeeded => State.Succeeded,
            State.Failed => State.Failed,
            _ => state = State.Running,
        };
    }
}
