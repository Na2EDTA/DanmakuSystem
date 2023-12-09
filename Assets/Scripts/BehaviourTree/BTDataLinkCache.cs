using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class BTDataLinkCache:ISerializationCallbackReceiver
{
    public Dictionary<BTElement, Dictionary<int, List<BTDataLink>>> outCache = new();
    public Dictionary<BTElement, Dictionary<int, List<BTDataLink>>> inCache = new();
    public List<BTLinkInfo> tempLinks = new();

    public Dictionary<int, List<BTDataLink>> this[BTElement element, Direction direction]
    {
        get
        {
            Dictionary<int, List<BTDataLink>> res;
            if (direction == Direction.Output)
            {
                outCache.TryGetValue(element, out res);
            }
            else
            {
                inCache.TryGetValue(element, out res);
            }
            return res = null;
        }
    }

    public List<BTDataLink> this[BTElement element, Direction direction, int index]
    {
        get
        {
            List<BTDataLink> res = new();
            if (direction == Direction.Output)
            {
                outCache.TryGetValue(element, out var elementLinks);
                elementLinks?.TryGetValue(index, out res);
            }
            else
            {
                inCache.TryGetValue(element, out var elementLinks);
                elementLinks?.TryGetValue(index, out res);
            }
            return res;
        }
    }

    public BTDataLinkCache(List<BTDataLink> dataLinks)
    {
        HashSet<BTElement> elementHashset = new();
        for (int i = 0; i < dataLinks.Count; i++)
        {
            elementHashset.Add(dataLinks[i].start);
            elementHashset.Add(dataLinks[i].end);
        }
        List<BTElement> elements = elementHashset.ToList();
        var outputs = dataLinks.GroupBy(element => element.start);  //按照节点分组
        var inputs = dataLinks.GroupBy(element => element.end);     //按照节点分组

        foreach (var outs in outputs)
        {
            Dictionary<int, List<BTDataLink>> target = new();
            var ts = outs.GroupBy(element => element.startIndex);//继续按照端口号分组
            foreach (var t in ts)
            {
                target.Add(t.Key, t.ToList());
            }
            outCache.Add(outs.Key, target);
        }

        foreach (var ins in inputs)
        {
            Dictionary<int, List<BTDataLink>> target = new();
            var ts = ins.GroupBy(element => element.endIndex);//继续按照端口号分组
            foreach (var t in ts)
            {
                target.Add(t.Key, t.ToList());
            }
            inCache.Add(ins.Key, target);
        }
    }

    public void SetUpBTDataLinkCache(List<BTDataLink> dataLinks)
    {
        HashSet<BTElement> elementHashset = new();
        for (int i = 0; i < dataLinks.Count; i++)
        {
            elementHashset.Add(dataLinks[i].start);
            elementHashset.Add(dataLinks[i].end);
        }
        List<BTElement> elements = elementHashset.ToList();
        var outputs = dataLinks.GroupBy(element => element.start);  //按照节点分组
        var inputs = dataLinks.GroupBy(element => element.end);     //按照节点分组

        foreach (var outs in outputs)
        {
            Dictionary<int, List<BTDataLink>> target = new();
            var ts = outs.GroupBy(element => element.startIndex);//继续按照端口号分组
            foreach (var t in ts)
            {
                target.Add(t.Key, t.ToList());
            }
            outCache?.Add(outs.Key, target);
        }

        foreach (var ins in inputs)
        {
            Dictionary<int, List<BTDataLink>> target = new();
            var ts = ins.GroupBy(element => element.endIndex);//继续按照端口号分组
            foreach (var t in ts)
            {
                target.Add(t.Key, t.ToList());
            }
            inCache.Add(ins.Key, target);
        }
    }

    public void OnBeforeSerialize()
    {
        tempLinks?.Clear();
        if (outCache == null && inCache == null) return;
        foreach (var item in outCache)
        {
            var element = item.Key;
            foreach (var it in item.Value)
            {
                int index = it.Key;
                foreach (var i in it.Value)
                {
                        tempLinks.Add(new()
                        {
                            element = element,
                            dir = Direction.Output,
                            index = index,
                            link = i
                        });
                }
            }
        }
        foreach (var item in inCache)
        {
            var element = item.Key;
            foreach (var it in item.Value)
            {
                int index = it.Key;
                foreach (var i in it.Value)
                {
                    tempLinks.Add(new()
                    {
                        element = element,
                        dir = Direction.Input,
                        index = index,
                        link = i
                    });
                }
            }
        }
    }

    public void OnAfterDeserialize()
    {
        /*outCache?.Clear();
        inCache?.Clear();
        List<BTDataLink> links = tempLinks.Select(li=>li.link).ToList();
        SetUpBTDataLinkCache(links);*/
    }
}

[System.Serializable]
public class BTLinkInfo
{
    public BTElement element;
    public int index;
    public Direction dir;
    public BTDataLink link;
}