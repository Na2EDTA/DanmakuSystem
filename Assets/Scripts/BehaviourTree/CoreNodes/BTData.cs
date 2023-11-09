using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTData : BTElement
{
    public float value;
    public BTData source;
    public List<BTData> destinations;
}
