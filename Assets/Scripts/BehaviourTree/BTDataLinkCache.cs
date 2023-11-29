using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class BTDataLinkCache
{
    Dictionary<BTElement, Dictionary<int, List<BTDataLink>>> outCache = new();
    Dictionary<BTElement, Dictionary<int, List<BTDataLink>>> inCache = new();

    public Dictionary<int, List<BTDataLink>> this[BTElement element, Direction direction]
    {
        get
        {
            if (direction == Direction.Output)
                return outCache[element];
            else
                return inCache[element];
        }
    }

    public List<BTDataLink> this[BTElement element, Direction direction, int index]
    {
        get
        {
            if (direction == Direction.Output)
                return outCache[element][index];
            else
                return inCache[element][index];
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
            outCache.Add(ins.Key, target);
        }
    }
}
