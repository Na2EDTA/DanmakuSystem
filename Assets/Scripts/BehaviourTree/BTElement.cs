using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTElement : ScriptableObject
{
    public string comment;
    public string guid;
    [HideInInspector] public BTTree tree;

#if UNITY_EDITOR
     public Vector2 position;
#endif
}
