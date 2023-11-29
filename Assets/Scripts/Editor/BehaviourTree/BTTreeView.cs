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
        searchMenu.Init(this);
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
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, a.eventInfo.mousePosition));
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
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateData(type));
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
        tree.MergeDatasAndNodes();
        tree.elements.ForEach(e =>//逐个节点操作
        {
            BTElementView ev = FindElementView(e);
            List<Port> outputPorts = ev.dataOutputs;
            for (int i = 0; i < outputPorts.Count; i++)//逐个端口操作
            {
                var outputLinks = tree.FindOutputLinks(e, i);
                outputLinks.ForEach(ol => //逐条线连接
                {
                    BTElementView target = FindElementView(ol.end);
                    var edge = outputPorts[i].ConnectTo(target.dataInputs[ol.endIndex]);
                    AddElement(edge);
                });
            }
        });
    }

    BTNodeView FindNodeView(BTNode node)
    {
        return GetNodeByGuid(node.guid) as BTNodeView;
    }

    BTElementView FindElementView(BTElement element)
    {
        return GetNodeByGuid(element.guid) as BTElementView;
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
                        BTElementView startView = edge.output.node as BTElementView;
                        BTElementView endView = edge.input.node as BTElementView;
                        int inIndex = endView.dataInputs.IndexOf(edge.input);
                        int outIndex = startView.dataOutputs.IndexOf(edge.output);
                        tree.UnlinkDatas(startView.Element, outIndex, endView.Element, inIndex);
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
                    BTElementView startView = edge.output.node as BTElementView;
                    BTElementView endView = edge.input.node as BTElementView;
                    int inIndex = endView.dataInputs.IndexOf(edge.input);
                    int outIndex = startView.dataOutputs.IndexOf(edge.output);
                    Type outType = edge.output.portType;
                    Type inType = edge.input.portType;
                    tree.LinkDatas(startView.Element, outIndex, outType, endView.Element, inIndex, inType);
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

/*    public void CreateNode(Type type)
    {
        BTNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }*/

    public void CreateNode(Type type, Vector2 pos)
    {
        BTNode node = tree.CreateNode(type);
        node.position = pos;
        CreateNodeView(node);
    }

/*    public void CreateData(Type type)
    {
        BTData data = tree.CreateData(type);
        CreateDataView(data);
    }*/

    public void CreateData(Type type, Vector2 pos)
    {
        BTData data = tree.CreateData(type);
        data.position = pos;
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
            if (n.GetType() == typeof(BTNodeView))
            {
                BTNodeView view = n as BTNodeView;

                view.UpdateState();
            }
        });
    }


}
