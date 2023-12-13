using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public abstract class BTElement : ScriptableObject
{
    public string comment;
    public string guid;
    [HideInInspector] public BTTree tree;
    public Dictionary<int, object> inputFieldCaches = new();
    public Dictionary<int, object> outputFieldCaches = new();

#if UNITY_EDITOR
    public Vector2 position;
#endif
}
