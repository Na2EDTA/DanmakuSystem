using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTData : BTElement
{
    public BTData source;
    public List<BTData> destinations;

    public abstract object Value { get; set; }
}
