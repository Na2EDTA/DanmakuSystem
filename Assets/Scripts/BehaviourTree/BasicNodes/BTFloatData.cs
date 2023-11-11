using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFloatData : BTData
{
    public float value;

    public override object Value { get => value; set => this.value = (float)value; }
}
