using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.Reflection;
using Danmaku.BehaviourTree;

public class BTNodeView : Node
{
    public BTNode node;
    public Port input, output;
    public List<Port> dataInputs, dataOutputs;
    public Action<BTNodeView> OnNodeSelected;

    public BTNodeView(BTNode node): base("Assets/Scripts/Editor/BehaviourTree/BTNodeView.uxml")
    {
        if (node is BTSubTreeNode) SetUpSubtreeNode(node);

        this.node = node;
        title = node.name;
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateOutputPorts();
        CreateInputPorts();
        CreateDataPorts(node);//根据attribute的情况，添加数据端口
        SetupClasses();
    }

    private void CreateDataPorts(BTNode node)
    {
        var fields = node.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].IsDefined(typeof(CreateInputPortAttribute)))
            {
                Port data = InstantiatePort(Orientation.Horizontal, 
                Direction.Input, Port.Capacity.Single, fields[i].FieldType);
                data.style.flexDirection = FlexDirection.Row;
                data.portName = " ";
                data.portColor = new(0.5f, 0.75f, 0.5f, 1);
                inputContainer.Add(data);
                dataInputs.Add(data);
            }
            else if (fields[i].FieldType.IsDefined(typeof(CreateOutputPortAttribute)))
            {
                Port data = InstantiatePort(Orientation.Horizontal,
                    Direction.Output, Port.Capacity.Multi, fields[i].FieldType);
                data.style.flexDirection = FlexDirection.Row;
                data.portName = " ";
                data.portColor = new(0.5f, 0.75f, 0.5f, 1);
                outputContainer.Add(data);
                dataOutputs.Add(data);
            }

        }
    }

    void SetUpSubtreeNode(BTNode node)
    {
        var subtreeNode = node as BTSubTreeNode;

        //双击BTSubTreeNode事件监听
        this.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.clickCount == 2 && title == "BTSubTreeNode") // 检查是否是双击事件并且节点的名称是"BTSubTreeNode"
            {
                Selection.activeObject = subtreeNode.subTree;
            }
        });
    }

    private void SetupClasses()
    {
        switch (node)
        {
            case BTActionNode:
                AddToClassList("action");                
                break;

            case BTCompositeNode:
                AddToClassList("composite");
                break;

            case BTDecoratorNode:
                AddToClassList("decorator");
                break;

            case BTRootNode:
                AddToClassList("root");
                break;
        }
    }

    private void CreateInputPorts()
    {
        switch (node)
        {
            case BTActionNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;

            case BTCompositeNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;

            case BTDecoratorNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;

            case BTRootNode:
                
                break;
        }

        if (input != null)
        {
            input.style.flexDirection = FlexDirection.Row;
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        switch (node)
        {
            case BTActionNode:
                
                break;

            case BTCompositeNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;

            case BTDecoratorNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;

            case BTRootNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;

            default:
                break;
        }

        if (output != null)
        {
            output.style.flexDirection = FlexDirection.RowReverse;
            output.portName = "";
            outputContainer.Add(output);
        }
    }

    //支持鼠标拖动位置，并且可以撤回
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    //重排子节点顺序
    public void SortChildren()
    {
        BTCompositeNode compositeNode = node as BTCompositeNode;
        if (compositeNode)
        {
            compositeNode.children?.Sort(SortByVerticalPosition);
        }
    }

    private int SortByVerticalPosition(BTNode up, BTNode down)
    {
        return up.position.y < down.position.y ? -1 : 1;
    }

    //外观更新状态
    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("succeeded");
        RemoveFromClassList("failed");
        RemoveFromClassList("started");
        RemoveFromClassList("unstarted");

        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case BTNode.State.Running:
                    if(node.started)
                        AddToClassList("running");
                    break;
                case BTNode.State.Succeeded:
                    AddToClassList("succeeded");
                    break;
                case BTNode.State.Failed:
                    AddToClassList("failed");
                    break;
                default:
                    break;
            }
            if (node.started)
            {
                AddToClassList("started");
            }
            else
                AddToClassList("unstarted");

        }
    }
}
