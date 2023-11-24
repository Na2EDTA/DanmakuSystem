using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSimpleFloat : BTData
{
    [Danmaku.BehaviourTree.CreateInputPort]
    [Danmaku.BehaviourTree.CreateOutputPort]
    public float value;
}