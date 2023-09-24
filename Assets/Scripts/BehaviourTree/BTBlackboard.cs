using System;
using System.Collections.Generic;
using UnityEngine;
using Danmaku.OdinSerializer;

[Serializable]
public class BTBlackboard: SerializedScriptableObject
{
    public Danmaku.SerializeExtension.Dictionary<string, int> intVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, float> floatVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, bool> booleanVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, bool> triggerVariables = new();

    public bool FindBooleanVariable(string name)
    {
        if (booleanVariables.ContainsKey(name))
        {
            return booleanVariables[name];
        }
        else throw new Exception("Can't find the boolean variable.");
    }

    public int FindIntegerVariable(string name)
    {
        if (intVariables.ContainsKey(name))
        {
            return intVariables[name];
        }
        else throw new Exception("Can't find the integer variable.");
    }

    public float FindFloatVariable(string name)
    {
        if (floatVariables.ContainsKey(name))
        {
            return floatVariables[name];
        }
        else throw new Exception("Can't find the float variable.");
    }
}
