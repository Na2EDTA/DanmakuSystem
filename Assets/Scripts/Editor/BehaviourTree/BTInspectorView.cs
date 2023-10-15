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
        Clear();//�����ڵ㣬��UIElementsԪ��������
        UnityEngine.Object.DestroyImmediate(editor);//����ԭ����inspector

        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new(() => { //container��Ԫ��
            if(editor.target)
                editor.OnInspectorGUI(); 
        });
        Add(container);
    }
}
