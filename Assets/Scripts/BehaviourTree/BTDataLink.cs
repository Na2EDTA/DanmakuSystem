using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Danmaku.BehaviourTree;

public class BTDataLink : ScriptableObject
{
    public BTElement start, end;
    public string guid;
    public string startName, endName;
    public int startIndex, endIndex;
    Func<object, object> getter;
    Action<object, object> setter;

    public BTDataLink(BTElement start, int startPortIndex, BTElement end, int endPortIndex)
    {
        this.start = start;
        this.startIndex = startPortIndex;
        this.end = end;
        this.endIndex = endPortIndex;
    }

    public void Init(BTElement start, int startPortIndex, string startName, BTElement end, int endPortIndex, string endName)
    {
        this.start = start;
        this.startIndex = startPortIndex;
        this.startName = startName;
        this.end = end;
        this.endIndex = endPortIndex;
        this.endName = endName;
        this.getter = ExpressionUtility.CreateGetter(startName, start.GetType());
        this.setter = ExpressionUtility.CreateSetter(endName, end.GetType());
        this.guid = GUID.Generate().ToString();
    }

    public override bool Equals(object obj)
    {
        return obj is BTDataLink link &&
               base.Equals(obj) &&
               name == link.name &&
               hideFlags == link.hideFlags &&
               EqualityComparer<BTElement>.Default.Equals(start, link.start) &&
               EqualityComparer<BTElement>.Default.Equals(end, link.end) &&
               startIndex == link.startIndex &&
               endIndex == link.endIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), name, hideFlags, start, end, startIndex, endIndex);
    }

    public static bool operator == (BTDataLink link1, BTDataLink link2)
    {
        return link1.start == link2.start &&
            link1.end == link2.end &&
            link1.startIndex == link2.startIndex &&
            link1.endIndex == link2.endIndex;
    }

    public static bool operator != (BTDataLink link1, BTDataLink link2)
    {
        return !(link1 == link2);
    }

    public void Transmit()
    {
        setter.Invoke(end, getter.Invoke(start));
    }
}
