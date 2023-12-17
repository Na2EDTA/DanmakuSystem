using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu]
public class BTTree : ScriptableObject
{
    public BTRootNode rootNode;
    public BTNode.State state = BTNode.State.Running;
    public List<BTNode> nodes = new();
    public List<BTData> datas = new();
    public List<BTElement> elements = new();
    public List<BTDataLink> dataLinks = new();
    public BTBlackboard blackboard;
    public BTRuntime runtime;
    public BTDataLinkCache linkCache;

    public BTNode.State Update()
    {
        if (rootNode.state == BTNode.State.Running) state = rootNode.Update();
        return state;
    }

    public void MergeDatasAndNodes()
    {
        elements.Clear();
        elements.AddRange(nodes);
        elements.AddRange(datas);
    }

    public void UpdateLinkCache()
    {
        linkCache = new(dataLinks);
    }

    #region 树的编辑
#if UNITY_EDITOR
    public BTBlackboard CreateBlackboard()
    {
        BTBlackboard blackboard = CreateInstance<BTBlackboard>();
        blackboard.name = "Blackboard";
        AssetDatabase.AddObjectToAsset(blackboard, this);
        AssetDatabase.SaveAssets();
        return blackboard;
    }

    //创建控制流节点
    public BTNode CreateNode(System.Type type)
    {
        BTNode node = CreateInstance(type) as BTNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        node.tree = this;

        Undo.RecordObject(this, "Behaviour Tree (Create Node)");
        nodes.Add(node);
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
        AssetDatabase.SaveAssets();
        MergeDatasAndNodes();
        return node;
    }

    //创建数据节点
    public BTData CreateData(System.Type type)
    {
        BTData data = ScriptableObject.CreateInstance(type) as BTData;
        
        data.guid = GUID.Generate().ToString();
        data.tree = this;
        data.name = type.Name;

        Undo.RecordObject(this, "Behaviour Tree (Create Data)");
        datas.Add(data);
        if (!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(data, this);
        Undo.RegisterCreatedObjectUndo(data, "Behaviour Tree (Create Data)");
        AssetDatabase.SaveAssets();
        MergeDatasAndNodes();
        return data;
    }

    public void RemoveNode(BTNode node)
    {
        Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
        nodes.Remove(node);
        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
        MergeDatasAndNodes();
        UpdateLinkCache();
    }

    public void RemoveData(BTData data)
    {
        Undo.RecordObject(this, "Behaviour Tree (Delete Data)");
        datas.Remove(data);
        //AssetDatabase.RemoveObjectFromAsset(data);
        Undo.DestroyObjectImmediate(data);
        AssetDatabase.SaveAssets();
        MergeDatasAndNodes();
        UpdateLinkCache();
    }

    public void LinkDatas(BTElement start, int startPortIndex, string outName, BTElement end, int endPortIndex, string inName)
    {
        Undo.RecordObject(this, "Behaviour Tree (Link Data)");

        var tarLink = ScriptableObject.CreateInstance<BTDataLink>();
        tarLink.Init(start, startPortIndex, outName, end, endPortIndex, inName);
        dataLinks.Add(tarLink);
        UpdateLinkCache();
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(tarLink, this);
        }
        Undo.RegisterCreatedObjectUndo(tarLink, "Behaviour Tree (Link Data)");
        AssetDatabase.SaveAssets();
    }

    public void UnlinkDatas(BTElement start, int startPortIndex, BTElement end, int endPortIndex)
    {
        Undo.RecordObject(this, "Behaviour Tree (Unlink Data)");
        
        var linksToRemove = dataLinks.Where(dl => 
        { 
            return dl.start == start && 
            dl.startIndex == startPortIndex &&
            dl.end == end &&
            dl.endIndex == endPortIndex; 
        }).ToList();
        
        for (int i = 0; i < linksToRemove.Count; i++)
        {
            dataLinks.Remove(linksToRemove[i]);
            Undo.DestroyObjectImmediate(linksToRemove[i]);
        }
        linksToRemove.Clear();
        UpdateLinkCache();
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BTNode parent, BTNode child)
    {
        BTDecoratorNode decorator = parent as BTDecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (Add Child)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }

        BTCompositeNode composite = parent as BTCompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (Add Child)");
            composite.children?.Add(child);
            EditorUtility.SetDirty(composite);
        }

        BTRootNode root = parent as BTRootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behaviour Tree (Add Child)");
            root.child = child;
            EditorUtility.SetDirty(root);
        }
        /*switch (parent)
        {
            case BTDecoratorNode:
                (parent as BTDecoratorNode).child = child;
                break;

            case BTCompositeNode:
                (parent as BTCompositeNode).children.Add(child);
                break;

            case BTRootNode:
                (parent as BTRootNode).child = child;
                break;
        }*/
    }

    public void RemoveChild(BTNode parent, BTNode child)
    {
        BTDecoratorNode decorator = parent as BTDecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (Remove Child)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }

        BTCompositeNode composite = parent as BTCompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (Remove Child)");
            composite.children?.Remove(child);
            EditorUtility.SetDirty(composite);
        }

        BTRootNode root = parent as BTRootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behaviour Tree (Remove Child)");
            root.child = null;
            EditorUtility.SetDirty(root);
        }

        UpdateLinkCache();
        /*switch (parent)
        {
            case BTDecoratorNode:
                (parent as BTDecoratorNode).child = null;
                break;

            case BTCompositeNode:
                (parent as BTCompositeNode).children.Remove(child);
                break;

            case BTRootNode:
                (parent as BTRootNode).child = null;
                break;
        }*/
    }
#endif
    #endregion

    public List<BTNode> GetChildren(BTNode parent)
    {
        List<BTNode> list = new();
        switch (parent)
        {
            case BTDecoratorNode:
                if ((parent as BTDecoratorNode).child != null)
                    list.Add((parent as BTDecoratorNode).child);
                break;

            case BTRootNode:
                if ((parent as BTRootNode).child != null)
                    list.Add((parent as BTRootNode).child);
                break;

            case BTCompositeNode:
                return (parent as BTCompositeNode).children;
        }
        return list;
    }

    public void Traverse(BTNode node, System.Action<BTNode> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public List<BTDataLink> FindOutputLinks(BTElement element)
    {
        return dataLinks.FindAll(l => l.start.guid == element.guid);
    }

    public List<BTDataLink> FindInputLinks(BTElement element)
    {
        return dataLinks.FindAll(l => l.end.guid == element.guid);
    }

    public List<BTDataLink> FindOutputLinks(BTElement element, int outputIndex)
    {
        if (linkCache == null)
            return dataLinks.FindAll(l => l.start.guid == element.guid && l.startIndex == outputIndex);
        else
        {
            //return linkCache[element, Direction.Output, outputIndex];
            var res = new List<BTDataLink>();
            linkCache.outCache.TryGetValue(element, out var elementLinks);
            elementLinks?.TryGetValue(outputIndex, out res);
            return res;
        }
    }

    public List<BTDataLink> FindInputLinks(BTElement element, int inputIndex)
    {
        if (linkCache == null)
            return dataLinks.FindAll(l => l.end.guid == element.guid && l.endIndex == inputIndex);
        else
        {
            //return linkCache[element, Direction.Input, inputIndex];
            var res = new List<BTDataLink>();
            linkCache.inCache.TryGetValue(element, out var elementLinks);
            elementLinks?.TryGetValue(inputIndex, out res);
            return res;
        }
    }

    public BTTree Clone()
    {
        BTTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone() as BTRootNode;
        tree.nodes = new List<BTNode>();
        tree.datas = new List<BTData>();
        tree.blackboard = blackboard.Clone();
        Traverse(tree.rootNode, (n) => { tree.nodes.Add(n); n.tree = tree; });
        for (int i = 0; i < datas.Count; i++)
        {
            tree.datas.Add(datas[i].Clone());
        }
        tree.MergeDatasAndNodes();
        //links是有问题的，没有完全生成独立的实例，仍然和prefab有粘连。
        for (int i = 0; i < tree.dataLinks.Count; i++)
        {
            tree.dataLinks[i] = tree.dataLinks[i].Clone(tree);
        }
        tree.linkCache = new(tree.dataLinks);
        return tree;
    }
}
