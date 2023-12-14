using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Danmaku.BehaviourTree;

public abstract class BTNode: BTElement
{
    public List<ReflexiveAccessor> accessors = new();
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
                InputDatas();
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                OutputDatas();
                started = false;
            }
        }
        catch (System.Exception)
        {
            OnStop();
            OutputDatas();
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

    void InputDatas()
    {
        Debug.Log(tree.FindInputLinks(this).Count);//0
        tree.FindInputLinks(this).ForEach(il => il.Transmit());
    }

    void OutputDatas()
    {
        tree.FindOutputLinks(this).FindAll(l => l.end is BTData).ForEach(ol => ol.Transmit());
    }
}
