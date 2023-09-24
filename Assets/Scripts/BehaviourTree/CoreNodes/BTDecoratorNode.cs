using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTDecoratorNode : BTNode
{
    [HideInInspector] public BTNode child;

    public override BTNode Clone()
    {
        BTDecoratorNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
