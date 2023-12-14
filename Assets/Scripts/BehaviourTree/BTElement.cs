using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public abstract class BTElement : ScriptableObject
{
    public string comment;
    public string guid;
    [HideInInspector] public BTTree tree;
    public Dictionary<int, string> inputFieldCaches = new();
    public Dictionary<int, string> outputFieldCaches = new();

#if UNITY_EDITOR
    public Vector2 position;
#endif

    public void AddInputCache(int index, string value)
    {
        inputFieldCaches.Add(index, value);
    }

    public void AddOutputCache(int index, string value)
    {
        outputFieldCaches.Add(index, value);
    }
}
