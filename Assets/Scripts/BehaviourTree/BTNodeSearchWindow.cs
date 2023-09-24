/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Linq;

public class BTNodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private BTGraphView _graphView;
    private EditorWindow _editorWindow;
    Texture2D _identationIcon;

    public void Init(BTGraphView graphView, EditorWindow editorWindow)
    {
        _graphView = graphView;
        _editorWindow = editorWindow;
        _identationIcon = new(1, 1);
        _identationIcon.SetPixel(0, 0, Color.clear);
        _identationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 1),
            new SearchTreeEntry(new GUIContent(" Create Plain Node", _identationIcon))
            {
                userData = new BTNode(),
                level = 2
            },
            new SearchTreeEntry(new GUIContent())
        };

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(
            _editorWindow.rootVisualElement.parent, 
            context.screenMousePosition - _editorWindow.position.position);

        Vector2 localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case BTNode:
                _graphView.CreateNodeOnWindow("Plain Node", localMousePosition);
                return true;

            default:
                return false;
        }
    }
}
*/