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
        children.ForEach(c => c.state = State.Running);
    }

    protected override State OnUpdate()
    {
        BTNode child;
        if (tree.blackboard.FindVariable(intVariableName, out int value))
            child = children[value];
        else
            throw new Exception($"Can't find variable {intVariableName}");

        return child.Update() switch
        {
            State.Running => State.Running,
            State.Succeeded => State.Succeeded,
            State.Failed => State.Failed,
            _ => state = State.Running,
        };
    }
}
