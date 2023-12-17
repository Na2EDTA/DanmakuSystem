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

public class BTNodeView : BTElementView
{
    public BTNode node;
    public Port input, output;
    public Action<BTNodeView> OnNodeSelected;

    public override BTElement Element { get => node; set => node = value as BTNode; }

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
        node.inputFieldCaches.Clear();
        node.outputFieldCaches.Clear();

        FieldInfo[] fields = node.GetType().GetFields();
        int inputCount = 0, outputCount = 0;

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].IsDefined(typeof(CreateInputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal, 
                Direction.Input, Port.Capacity.Multi, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);

                inputContainer.Add(dataPort);
                dataInputs.Add(dataPort);
                
                node.inputFieldCaches.Add(inputCount, fields[i].Name);
                
                inputCount++;
            }
            if (fields[i].IsDefined(typeof(CreateOutputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal,
                    Direction.Output, Port.Capacity.Multi, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);
                //dataPort.valueOffset = UnsafeUtility.GetFieldOffset(fields[i]);

                outputContainer.Add(dataPort);
                dataOutputs.Add(dataPort);

                node.outputFieldCaches.Add(outputCount, fields[i].Name);
                outputCount++;
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
