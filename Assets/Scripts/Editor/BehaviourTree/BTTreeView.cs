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
    public BTSearchMenu searchMenu;
    public BTEditorWindow editorWindow;
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
        AddSearchMenu();

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTree/BTEditorWindow.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void AddSearchMenu()
    {
        searchMenu = ScriptableObject.CreateInstance<BTSearchMenu>();
        searchMenu.Init(this, editorWindow);
        nodeCreationRequest = context => SearchWindow.Open(new(context.screenMousePosition), searchMenu);
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    //创建节点菜单
    /*public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
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
    }*/

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

        //create datas in view
        tree.datas.ForEach(n => CreateDataView(n));
        //create edges in view
        //tree.datas.ForEach(n =>
        //{
        //    BTDataView inputView = FindDataView(n.next);
        //    BTDataView outputView = FindDataView(n);
        //    Edge edge = outputView.output.ConnectTo(inputView?.input);
        //    AddElement(edge);
        //});
    }

    BTNodeView FindNodeView(BTNode node)
    {
        return GetNodeByGuid(node.guid) as BTNodeView;
    }

    BTDataView FindDataView(BTData data)
    {
        return GetNodeByGuid(data.guid) as BTDataView;
    }

    //获取可连接的节点
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node && endPort.portName == startPort.portName 
        && endPort.portType == startPort.portType).ToList();
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

                BTDataView dataView = elem as BTDataView;
                if (dataView != null)
                {
                    tree.RemoveData(dataView.data);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    if (edge.input.portName == "" && edge.input.portName == edge.output.portName) 
                    {
                        BTNodeView parentView = edge.output.node as BTNodeView;
                        BTNodeView childView = edge.input.node as BTNodeView;
                        tree.RemoveChild(parentView.node, childView.node); 
                    }
                    else if(edge.input.portType == edge.output.portType && edge.input.portName == " ")
                    {
                        //数据节点链接行为
                    }
                }
            });
        }

        var edgesToCreate = graphViewChange.edgesToCreate;

        if (edgesToCreate != null)
        {
            edgesToCreate.ForEach(edge => 
            {
                if (edge.input.portName == edge.output.portName && edge.input.portName == "") 
                {
                    BTNodeView parentView = edge.output.node as BTNodeView;
                    BTNodeView childView = edge.input.node as BTNodeView;
                    tree.AddChild(parentView.node, childView.node); 
                }
                else if (edge.input.portType == edge.output.portType && edge.input.portName  == " ")
                {
                    //数据节点链接行为
                }
            });
        }

        //重排子节点顺序
        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                if (n.GetType() == typeof(BTNodeView))
                {
                    BTNodeView view = n as BTNodeView;
                    view.SortChildren();
                }
            });
        }

        return graphViewChange;
    }
/*
    public void CreateNode(Type type)
    {
        BTNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    public void CreateData(Type type)
    {
        BTData data = tree.CreateData(type);
        CreateDataView(data);
    }
*/
    public void CreateNodeView(BTNode node)
    {
        BTNodeView nodeView = new(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    public void CreateDataView(BTData data)
    {
        BTDataView dataView = new(data);
        dataView.OnDataSelected = OnDataSelected;

        AddElement(dataView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            if (n.GetType() == typeof(BTNodeView))
            {
                BTNodeView view = n as BTNodeView;

                view.UpdateState();
            }
        });
    }
}
