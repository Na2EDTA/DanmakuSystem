using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Linq;

public class BTSearchMenu : ScriptableObject, ISearchWindowProvider
{
    BTTreeView treeView;
    BTEditorWindow editorWindow;
    Texture2D identationIcon;

    public void Init(BTTreeView treeView, BTEditorWindow editorWindow)
    {
        this.treeView = treeView;
        this.editorWindow = editorWindow;
        identationIcon = new(1, 1);
        identationIcon.SetPixel(0, 0, Color.clear);
        identationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new();


        tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Element"), 0));
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Data"), 1));
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Node"), 1));

        foreach (var type in TypeCache.GetTypesDerivedFrom<BTNode>())
        {
            if(type == typeof(BTActionNode) || type == typeof(BTCompositeNode) || type == typeof(BTDecoratorNode))
            {
                Debug.Log(type);
                tree.Add(new SearchTreeGroupEntry(new GUIContent($"Create {type.Name}"), 2));

                foreach (var t in TypeCache.GetTypesDerivedFrom(type))
                {
                    Debug.Log(t.Name);
                    tree.Add(new SearchTreeEntry(new GUIContent($"Create {t.Name}"))
                    {
                        userData = treeView.tree.CreateNode(t),
                        level = 3
                    });
                }
            }
        }

        /*foreach (var type in TypeCache.GetTypesDerivedFrom<BTData>())
        {
            tree.Add(new SearchTreeEntry(new GUIContent($"Create {type.Name}"))
            {
                userData = treeView.tree.CreateData(type),
                level = 2
            }) ;
        }*/

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent,
            context.screenMousePosition - editorWindow.position.position);

        Vector2 localMousePosition = treeView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case BTNode:
                var node = SearchTreeEntry.userData as BTNode;
                treeView.CreateNodeView(node);
                node.position = localMousePosition;
                return true;

            case BTData:
                var data = SearchTreeEntry.userData as BTData;
                treeView.CreateDataView(data);
                data.position = localMousePosition;
                return true;

            default:
                return false;
        }
    }
}
