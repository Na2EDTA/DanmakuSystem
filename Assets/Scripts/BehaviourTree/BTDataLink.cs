using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BTDataLink : ScriptableObject
{
    public BTElement start, end;
    public string guid;
    public Type startType, endType;
    public int startIndex, endIndex;

    public BTDataLink(BTElement start, int startPortIndex, BTElement end, int endPortIndex)
    {
        this.start = start;
        this.startIndex = startPortIndex;
        this.end = end;
        this.endIndex = endPortIndex;
    }

    public void Init(BTElement start, int startPortIndex, Type startType, BTElement end, int endPortIndex, Type endType)
    {
        this.start = start;
        this.startIndex = startPortIndex;
        this.startType = startType;
        this.end = end;
        this.endIndex = endPortIndex;
        this.endType = endType;
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
}
