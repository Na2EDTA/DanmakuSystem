using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTElement : ScriptableObject
{
    public string comment;
    public string guid;
    [HideInInspector] public BTTree tree;
    public List<BTInputDataPort> inputs = new();
    public List<BTOutputDataPort> outputs = new();

#if UNITY_EDITOR
    public Vector2 position;
#endif
}
