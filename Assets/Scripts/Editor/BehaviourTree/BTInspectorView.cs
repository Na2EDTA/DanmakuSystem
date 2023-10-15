using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
public class BTInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BTInspectorView, UxmlTraits> { }

    Editor editor;

    public BTInspectorView()
    {

    }

    internal void UpdateSelection(BTNodeView nodeView)
    {
        Clear();//更换节点，在UIElements元素意义上
        UnityEngine.Object.DestroyImmediate(editor);//销毁原来的inspector

        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new(() => { //container是元素
            if(editor.target)
                editor.OnInspectorGUI(); 
        });
        Add(container);
    }
}
