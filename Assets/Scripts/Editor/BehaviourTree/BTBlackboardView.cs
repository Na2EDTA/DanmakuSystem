using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class BTBlackboardView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BTBlackboardView, UxmlTraits> { }

    Editor editor;

    public BTBlackboardView()
    {

    }

    public void UpdateSelection(BTTree tree)
    {
        Clear();
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(tree.blackboard);
        IMGUIContainer container = new(() => { 
            if (editor.target)
                editor.OnInspectorGUI();
        });
        Add(container);
    }
}
