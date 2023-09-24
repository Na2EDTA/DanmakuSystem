using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTParallelNode : BTCompositeNode
{
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
        children.ForEach(c => c.state = State.Running);
    }

    protected override State OnUpdate()
    {
        State tempState = State.Succeeded;
        for (int i = 0; i < children.Count; i++)
        {
            var c = children[i];

            switch (c.state)
            {
                case State.Running:
                    c.Update();
                    tempState = State.Running;
                    break;

                case State.Succeeded:
                    break;

                case State.Failed:
                    return State.Failed;

                default:
                    break;
            }
        }
        return tempState;
    }
}
