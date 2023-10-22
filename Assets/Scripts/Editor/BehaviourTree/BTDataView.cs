using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;


public class BTDataView : Node
{
    public BTData data;
    public Port input, output;
    public Action<BTDataView> OnNodeSelected;
}
