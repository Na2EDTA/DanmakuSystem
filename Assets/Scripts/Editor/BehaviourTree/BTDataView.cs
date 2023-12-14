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
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;

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
    }

    private void CreateDataPorts(BTData data)
    {
        data.inputFieldCaches.Clear();
        data.outputFieldCaches.Clear();

        FieldInfo[] fields = data.GetType().GetFields();
        int inputCount = 0, outputCount = 0;
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].IsDefined(typeof(CreateInputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);

                inputContainer.Add(dataPort);
                dataInputs.Add(dataPort);

                data.AddInputCache(inputCount, fields[i].Name);
                inputCount++;
            }
            if (fields[i].IsDefined(typeof(CreateOutputPortAttribute)))
            {
                Port dataPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, fields[i].FieldType);
                dataPort.style.flexDirection = FlexDirection.Row;
                dataPort.portName = " ";
                dataPort.portColor = new(0.5f, 0.75f, 0.5f, 1);

                outputContainer.Add(dataPort);
                dataOutputs.Add(dataPort);
                
                data.AddOutputCache(outputCount, fields[i].Name);

                outputCount++;

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
