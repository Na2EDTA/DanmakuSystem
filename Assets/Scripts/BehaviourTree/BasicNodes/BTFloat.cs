using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFloat : BTData
{
    float value;

    public override object Value { get => value; set => this.value = (float)value; }
}
