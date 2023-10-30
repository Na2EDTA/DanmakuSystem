using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTData : ScriptableObject
{
    public string comment;
    public string guid;
    public BTData source;
    public List<BTData> destinations;
    [HideInInspector] public BTTree tree;
    [HideInInspector] public Vector2 position;
}

public class BTData<T> : BTData
{
    public T value;
}