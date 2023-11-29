using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BTElementView : Node
{
    public List<Port> dataInputs = new(), dataOutputs = new();

    public BTElementView(string ussPath): base(ussPath)
    {
        
    }

    public abstract BTElement Element { get; set; }

    
}
