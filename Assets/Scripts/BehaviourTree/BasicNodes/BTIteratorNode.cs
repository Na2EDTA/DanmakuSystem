using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTIteratorNode : BTDecoratorNode
{
    [CreateInputPort]public string count = "inf";
    int _count = -1;
    int i = 0;

    protected override void OnStart()
    {
        BTBlackboard b = tree.blackboard;

        if (count == "inf") return;

        b.Parse(count, out _count);
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (count == "inf")//infinite loop
        {
            state = child.Update();
            if (state == State.Failed)
            {
                Debug.LogError($"{child.name} Failed!");
                return state;
            }
            else
                state = child.state = State.Running;
        }
        else if (i < _count)//finite loop
        {
            state = child.Update();
            if (state == State.Failed)
            {
                Debug.LogError($"{child.name} failed in running!");
                return state;
            }
            else if (state == State.Succeeded)
            {
                i++;
                state = child.state = State.Running;
            }
            return state;
        }
        else//finished
        {
            state = State.Succeeded;
            i = 0;
        }


        return state;
    }
}
