using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;


public class BTDataView : Node
{
    public BTData data;
    public Port input, output;
    public Action<BTDataView> OnDataSelected;

    public BTDataView(BTData data) : base("Assets/Scripts/Editor/BehaviourTree/BTDataView.uxml")
    {
        this.data = data;
        title = data.name;
        viewDataKey = data.guid;

        style.left = data.position.x;
        style.top = data.position.y;

        CreateOutputPorts();
        CreateInputPorts();
    }

    private void CreateInputPorts()
    {
        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
    }

    private void CreateOutputPorts()
    {
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
    }

    //支持鼠标拖动位置，并且可以撤回
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(data, "Behaviour Tree (Set position)");
        data.position.x = newPos.xMin;
        data.position.y = newPos.yMin;
        EditorUtility.SetDirty(data);
    }

    //选中时行为
    public override void OnSelected()
    {
        base.OnSelected();
        if (OnDataSelected != null)
        {
            OnDataSelected.Invoke(this);
        }
    }
}
