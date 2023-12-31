using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BTBlackboard: ScriptableObject
{
    public Danmaku.SerializeExtension.Dictionary<string, int> intVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, float> floatVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, bool> booleanVariables = new();
    public Danmaku.SerializeExtension.Dictionary<string, bool> triggerVariables = new();

    public bool FindVariable(string name, out bool result)
    {
        if (booleanVariables.ContainsKey(name))
        {
            result = booleanVariables[name];
            return true;
        }
        else
        {
            result = false;
            return false;
        }
    }

    public bool FindVariable(string name, out int result)
    {
        if (intVariables.ContainsKey(name))
        {
            result = intVariables[name];
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }

    public bool FindVariable(string name, out float result)
    {
        if (floatVariables.ContainsKey(name))
        {
            result = floatVariables[name];
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }

    public bool SetVariable(string name, IComparable ori)
    {
        float f = 0;
        int i = 0;
        bool b = false;

        switch(ori)
        {
            case int:
                i = (int)ori;
                break;

            case float:
                f = (float)ori;
                break;

            case bool:
                b = (bool)ori;
                break;

            default:
                return false;
        }

        if (floatVariables.ContainsKey(name))
        {
            floatVariables[name] = f;
            return true;
        }
        else if (intVariables.ContainsKey(name))
        {
            intVariables[name] = i;
            return true;
        }
        else if (booleanVariables.ContainsKey(name))
        {
            booleanVariables[name] = b;
            return true;
        }
        else
        {
            Debug.LogError($"Can't find variable {name}");
            return false;
        }
    }

    public bool Parse(string s, out bool result)
    {
        if (bool.TryParse(s, out bool temp))
        {
            result = temp;
            return true;
        }
        else if (FindVariable(s, out temp))
        {
            result = temp;
            return true;
        }
        else
        {
            Debug.LogWarning($"Can't find variable {s} in {this}.");
            result = false;
            return false;
        }
    }

    public bool Parse(string s, out int result)
    {
        if (int.TryParse(s, out int temp))
        {
            result = temp;
            return true;
        }
        else if (FindVariable(s, out temp))
        {
            result = temp;
            return true;
        }
        else
        {
            Debug.LogWarning($"Can't find variable {s} in {this}.");
            result = 0;
            return false;
        }
    }

    public bool Parse(string s, out float result)
    {
        if (float.TryParse(s, out float temp))
        {
            result = temp;
            return true;
        }
        else if (FindVariable(s, out temp))
        {
            result = temp;
            return true;
        }
        else
        {
            result = 0;
            Debug.LogWarning($"Can't find variable {s} in {this}.");
            return false;
        }
    }

    public BTBlackboard Clone()
    {
        return Instantiate(this);
    }
}
