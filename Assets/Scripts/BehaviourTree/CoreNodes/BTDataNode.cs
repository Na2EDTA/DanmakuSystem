using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDataNode : ScriptableObject { }

public class BTDataNode<T> : BTDataNode
{
    public string comment;
    public string Guid;
    public T value;
    [HideInInspector] public Vector2 position;
    
}