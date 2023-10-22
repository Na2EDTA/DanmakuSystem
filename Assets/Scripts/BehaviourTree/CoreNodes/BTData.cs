using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTData : ScriptableObject { }

public class BTDataNode<T> : BTData
{
    public string comment;
    public string guid;
    public T value;
    [HideInInspector] public BTTree tree;
    [HideInInspector] public Vector2 position;
    
}