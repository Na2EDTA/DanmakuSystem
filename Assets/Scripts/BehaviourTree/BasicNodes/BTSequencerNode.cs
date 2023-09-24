using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequencerNode : BTCompositeNode
{
    int current;
    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        BTNode child = children[current];
        switch (child.Update())
        {
            case State.Running:
                return State.Running;

            case State.Succeeded:
                current++;
                break;

            case State.Failed:
                return State.Failed;
        }

        return current == children.Count ? State.Succeeded : State.Running;
    }
}
