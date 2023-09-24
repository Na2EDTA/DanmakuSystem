/*///
/// 全几把是前端性质的活儿
///————行为树显示区域与逻辑
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Danmaku.Deprecated;

public class BTGraphView : GraphView
{
    public readonly Vector2 defaultSize = new(150, 200);
    BTNodeSearchWindow _searchWindow;

    public Blackboard blackboard;
    public List<ExternalProperty> properties = new();

    public BTGraphView(EditorWindow window)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("BehaviourTree/BTGraphEditorWindow"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryNode());
        AddSearchWindow(window);
    }

    //添加连接端口
    private Port GeneratePort(BTNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }

    //清空属性、小黑板
    public void ClearBlackboardAndProperties()
    {
        properties.Clear();
        blackboard.Clear();
    }

    //添加外部属性
    public void AddPropertyToBlackboard(ExternalProperty externalProperty)
    {
        var localPropertyName = externalProperty.name;
        var localPropertyValue = externalProperty.value;
        int repeated = 0;
        string tempName = localPropertyName;
        while (properties.Any(x => x.name == localPropertyName))//保证新变量不重名
        {
            repeated++;
            localPropertyName = $"{tempName} ({repeated})";
        }

        ExternalProperty property = new(localPropertyName, localPropertyValue);
        
        
        properties.Add(property);

        var container = new VisualElement();
        var blackboardField = 
            new BlackboardField { text = property.name, typeText = "string" };
        container.Add(blackboardField);

        TextField valueTextField = new("Value ") { value = localPropertyValue};
        valueTextField.RegisterValueChangedCallback(evt => 
        {
            int changedPropertyIndex = properties.FindIndex(x => x.name == property.name);
            properties[changedPropertyIndex].value = evt.newValue;
        });
        container.Add(valueTextField);
        BlackboardRow blackboardRow = new(blackboardField, valueTextField);
        container.Add(blackboardRow);

        blackboard.Add(container);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new();
        ports.ForEach((port) =>
        {
            //端口不可以连自己，也不可以连同一个节点的其他端口造成循环
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    //生成根节点
    private BTNode GenerateEntryNode()
    {
        BTNode node = new()
        {
            title = "Entry",
            GUID = Guid.NewGuid().ToString(),
            comment = "EntryPoint",
            isRoot = true
        };

        Port generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Start at";
        generatedPort.portColor = Color.magenta * 0.5f + Color.white * 0.5f;
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    //将新增节点加入到页面元素
    public void CreateNodeOnWindow(string nodeName, Vector2 pos)
    {
        AddElement(CreateNode(nodeName, pos));
    }

    //新建节点
    public BTNode CreateNode(string nodeName, Vector2 pos)
    {
        BTNode node = new()
        {
            title = nodeName,
            comment = "",
            GUID = Guid.NewGuid().ToString()
        };

        Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        Button button = new(() => { AddOutputPort(node); });
        button.text = "Add Output";
        node.titleContainer.Add(button);

        TextField textField = new(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            node.comment = evt.newValue;
            node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(node.title);
        node.mainContainer.Add(textField);

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(pos, defaultSize));
        return node;
    }

    public void AddOutputPort(BTNode node, string overriddenPortName = "")
    {
        Port generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portColor = Color.magenta * 0.5f + Color.white * 0.5f;

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        string btPortName = string.IsNullOrEmpty(overriddenPortName) 
            ? "Output" 
            : overriddenPortName;

        TextField textField = new()
        {
            name = string.Empty,
            value = btPortName
        };
        textField.RegisterValueChangedCallback(e => generatedPort.portName = e.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        Button deleteButton = new(() => RemovePort(node, generatedPort))
        { 
            text = "Delete"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = btPortName;
        node.outputContainer.Add(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    void RemovePort(BTNode node, Port generatedPort)
    {
        IEnumerable<Edge> targetEdge = edges.ToList().Where(x 
            => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (!targetEdge.Any())
        {
            var e = targetEdge.First();
            e.input.Disconnect(e);
            RemoveElement(targetEdge.First());
        }
        Edge edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        node.outputContainer.Remove(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<BTNodeSearchWindow>();
        _searchWindow.Init(this, editorWindow);
        nodeCreationRequest = context => SearchWindow.Open(
            new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }
}
*/