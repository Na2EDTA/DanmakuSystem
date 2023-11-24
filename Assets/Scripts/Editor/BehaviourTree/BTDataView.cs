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

public class BTDataView : BTElementView
{
    public BTData data;
    public Action<BTDataView> OnDataSelected;

    public override BTElement Element { get => data; set => data = value as BTData; }

    public BTDataView(BTData data) : base("Assets/Scripts/Editor/BehaviourTree/BTDataView.uxml")
    {
        this.data = data;
        title = data.name;
        viewDataKey = data.guid;

        style.left = data.position.x;
        style.top = data.position.y;

        CreateDataPorts(data);
        //CreateOutputPorts();
        //CreateInputPorts();
    }

    private void CreateDataPorts(BTData data)
    {
        data.inputs.Clear();
        data.outputs.Clear();

        FieldInfo[] fields = data.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].IsDefined(typeof(CreateInputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal,
                Direction.Input, Port.Capacity.Single, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);
                inputContainer.Add(dataPort);
                dataInputs.Add(dataPort);

                var inputData = new BTInputDataPort()
                {
                    fieldName = fields[i].Name,
                    valueType = fields[i].FieldType,
                    element = data
                };
                data.inputs.Add(inputData);
            }
            if (fields[i].IsDefined(typeof(CreateOutputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal,
                    Direction.Output, Port.Capacity.Multi, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);
                outputContainer.Add(dataPort);
                dataOutputs.Add(dataPort);

                var outputData = new BTOutputDataPort()
                {
                    fieldName = fields[i].Name,
                    valueType = fields[i].FieldType,
                    element = data
                };
            }
        }
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
