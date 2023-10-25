using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTData : ScriptableObject
{
    public string comment;
    public string guid;
    public BTData previous;
    public BTData next;
    [HideInInspector] public BTTree tree;
    [HideInInspector] public Vector2 position;
}

public class BTData<T> : BTData
{
    public T value;
}