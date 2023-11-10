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
    EditorWindow editorWindow;
    Texture2D identationIcon;

    public void Init(BTTreeView treeView)
    {
        this.treeView = treeView;
        this.editorWindow = EditorWindow.focusedWindow;
        identationIcon = new(1, 1);
        identationIcon.SetPixel(0, 0, Color.clear);
        identationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new();

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Element"), 0));

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Data"), 1));

        foreach (var type in TypeCache.GetTypesDerivedFrom<BTData>())
        {
            tree.Add(new SearchTreeEntry(new GUIContent($" {type.Name}"))
            {
                userData = type,
                level = 2
            });
        }

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Node"), 1));

        foreach (var type in TypeCache.GetTypesDerivedFrom<BTNode>())
        {
            if(type == typeof(BTActionNode) || type == typeof(BTCompositeNode) || type == typeof(BTDecoratorNode))
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent($" {type.Name}"), 2));

                foreach (var t in TypeCache.GetTypesDerivedFrom(type))
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($" {t.Name}"))
                    {
                        userData = t,
                        level = 3
                    });
                }
            }
        }
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent,
            context.screenMousePosition - editorWindow.position.position);

        Vector2 localMousePosition = treeView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch ((SearchTreeEntry.userData as Type).BaseType.Name)
        {
            case "BTActionNode":
                {
                    var type = SearchTreeEntry.userData as Type;
                    treeView.CreateNode(type, localMousePosition);
                    return true;
                }

            case "BTCompositeNode":
                {
                    var type = SearchTreeEntry.userData as Type;
                    treeView.CreateNode(type, localMousePosition);
                    return true;
                }

            case "BTDecoratorNode":
                {
                    var type = SearchTreeEntry.userData as Type;
                    treeView.CreateNode(type, localMousePosition);
                    return true;
                }

            case "BTData":
                {
                    var type = SearchTreeEntry.userData as Type;
                    treeView.CreateData(type, localMousePosition);
                    return true;
                }

            default:
                return false;
        }
    }
}
