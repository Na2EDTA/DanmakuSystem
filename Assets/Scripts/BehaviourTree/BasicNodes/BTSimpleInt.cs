using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSimpleInt : BTData
{
    [Danmaku.BehaviourTree.CreateOutputPort]
    public int value;

    [Danmaku.BehaviourTree.CreateOutputPort]
    public int valve;

    [Danmaku.BehaviourTree.CreateOutputPort]
    public int glove;
}
