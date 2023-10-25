using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class BTTreeView : GraphView
{
    public BTTree tree;
    public Action<BTNodeView> OnNodeSelected;
    public Action<BTDataView> OnDataSelected;

    public class UxmlFactroy: UxmlFactory<BTTreeView, UxmlTraits> { }
    public BTTreeView()
    {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTree/BTEditorWindow.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    //创建节点菜单
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            var types = TypeCache.GetTypesDerivedFrom<BTActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BTCompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BTDecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BTRootNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BTData>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.Name}]", (a) => CreateData(type));
            }
        }
    }

    //读取树并显示
    public void PopulateView(BTTree tree)
    {
        this.tree = tree;
        if (!tree) return;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(BTRootNode)) as BTRootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        if (tree.blackboard == null)
        {
            tree.blackboard = tree.CreateBlackboard();
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        //create nodes in view
        tree.nodes.ForEach(n => CreateNodeView(n)); 
        //create edges in view
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                BTNodeView parentView = FindNodeView(n);
                BTNodeView childView = FindNodeView(c);

                Edge edge = parentView?.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });   
    }

    BTNodeView FindNodeView(BTNode node)
    {
        return GetNodeByGuid(node.guid) as BTNodeView;
    }

    //获取可连接的节点
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    //图发生改变时的响应函数，改变runtime的树
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                BTNodeView nodeView = elem as BTNodeView;
                if (nodeView != null)
                {
                    tree.RemoveNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    BTNodeView parentView = edge.output.node as BTNodeView;
                    BTNodeView childView = edge.input.node as BTNodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge => 
            {
                BTNodeView parentView = edge.output.node as BTNodeView;
                BTNodeView childView = edge.input.node as BTNodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }

        //重排子节点顺序
        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                BTNodeView view = n as BTNodeView;
                view.SortChildren();
            });
        }

        return graphViewChange;
    }

    void CreateNode(Type type)
    {
        BTNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    void CreateData<T>()
    {
        BTData<T> data = tree.CreateData<T>();
        CreateDataView(data);
    }

    void CreateData(Type type)
    {
        BTData data = tree.CreateData(type);
        CreateDataView(data);
    }

    void CreateNodeView(BTNode node)
    {
        BTNodeView nodeView = new(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    void CreateDataView(BTData data)
    {
        BTDataView dataView = new(data);
        dataView.OnDataSelected = OnDataSelected;

        AddElement(dataView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            BTNodeView view = n as BTNodeView;
            view.UpdateState();
        });
    }
}
