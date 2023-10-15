using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

public class BTNodeView : Node
{
    public BTNode node;
    public Port input, output;
    public Action<BTNodeView> OnNodeSelected;

    public BTNodeView(BTNode node): base("Assets/Scripts/Editor/BehaviourTree/BTNodeView.uxml")
    {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateOutputPorts();
        CreateInputPorts();
        SetupClasses();
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
