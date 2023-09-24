/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using Danmaku.Deprecated;

public class BTGraphSaveLoadUtility
{
    private BTGraphView _targetGraphView;
    private BTGraph _graphCache;

    List<Edge> Edges => _targetGraphView.edges.ToList();
    List<BTNode> Nodes => _targetGraphView.nodes.ToList().Cast<BTNode>().ToList();

    public static BTGraphSaveLoadUtility GetInstance(BTGraphView targetGraphView)
    {
        return new BTGraphSaveLoadUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        var graph = ScriptableObject.CreateInstance<BTGraph>();
        if (!SaveNodes(graph)) return;
        SaveProperties(graph);

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(graph, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveProperties(BTGraph graph)
    {
        graph.properties.AddRange(_targetGraphView.properties);
    }

    private bool SaveNodes(BTGraph graph)
    {
        if (!Edges.Any()) return false; //空则返回

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            BTNode outputNode = connectedPorts[i].output.node as BTNode;
            BTNode inputNode = connectedPorts[i].input.node as BTNode;

            graph.links.Add(new BTNodeLinkData
            {
                startNodeGuid = outputNode.GUID,
                targetNodeGuid = inputNode.GUID,
                portName = connectedPorts[i].output.portName
            });
        }

        foreach (var btNode in Nodes.Where(node => !node.isRoot))
        {
            graph.nodeData.Add(new()
            {
                Guid = btNode.GUID,
                comment = btNode.comment,
                pos = btNode.GetPosition().position
            });
        }
        return true;
    }

    public void LoadGraph(string fileName)
    {
        _graphCache = Resources.Load<BTGraph>(fileName);
        if(_graphCache==null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target graph file does not exists!", "OK");
            return;
        }
        ClearGraph();
        CreateNodes();
        ConnectNodes();
        CreateProperties();
    }

    private void CreateProperties()
    {
        _targetGraphView.ClearBlackboardAndProperties();

        foreach (var property in _graphCache.properties)
        {
            _targetGraphView.AddPropertyToBlackboard(property);
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            List<BTNodeLinkData> connections = 
                _graphCache.links.Where(x => x.startNodeGuid == Nodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].targetNodeGuid;
                Node target = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)target.inputContainer[0]);

                target.SetPosition(new Rect
                    (_graphCache.nodeData.First(x => x.Guid == targetNodeGuid).pos,
                    _targetGraphView.defaultSize));
            }
        }
    }

    private void LinkNodes(Port start, Port target)
    {
        Edge tempEdge = new()
        {
            output = start,
            input = target
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _graphCache.nodeData)
        {
            var tempNode = _targetGraphView.CreateNode(nodeData.comment, nodeData.pos);
            tempNode.GUID = nodeData.Guid;
            tempNode.comment = nodeData.comment;
            _targetGraphView.AddElement(tempNode);

            List<BTNodeLinkData> nodePorts =
                _graphCache.links.Where(x => x.startNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddOutputPort(tempNode));
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.isRoot).GUID = _graphCache.links[0].startNodeGuid;

        foreach (var node in Nodes)
        {
            if (node.isRoot) continue;
            //清理连线
            Edges.Where(x => x.input.node == node).ToList().
                ForEach(edge => _targetGraphView.RemoveElement(edge));
            //清理多余节点
            _targetGraphView.RemoveElement(node);
        }
    }
}
*/