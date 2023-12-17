using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BTData : BTElement
{
    public virtual BTData Clone()
    {
        return Instantiate(this);
    }
}
