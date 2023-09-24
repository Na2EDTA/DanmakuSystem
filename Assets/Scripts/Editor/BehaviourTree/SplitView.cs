using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory: UxmlFactory<SplitView, UxmlTraits> { }
}
