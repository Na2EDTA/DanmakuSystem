using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode: BTElement
{
    /*[HideInInspector]*/ public bool started = false;
    /*[HideInInspector]*/ public State state = State.Running;

    public float this[string index]
    {
        get
        {
            return tree.blackboard.floatVariables[index];
        }
        set
        {
            tree.blackboard.floatVariables[index] = value;
        }
    }

    public enum State
    {
        Running, Succeeded, Failed
    }

    public State Update()
    {
        try
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                started = false;
            }
        }
        catch (System.Exception)
        {
            OnStop();
            state = State.Failed;
            started = false;
        }

        return state;
    }

    public virtual BTNode Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract State OnUpdate();
    protected abstract void OnStop();
}
