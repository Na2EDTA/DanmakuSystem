using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTIteratorNode : BTDecoratorNode
{
    public int count = -1;
    int i = 0;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (count == -1)//infinite loop
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
        else if (i < count)//finite loop
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
        }
        else//finished
        {
            state = State.Succeeded;
        }
        return state;
    }
}
