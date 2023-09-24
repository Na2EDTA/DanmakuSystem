using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Danmaku.Deprecated
{
    [Serializable]
    public class BTGraph : ScriptableObject
    {
        public List<BTNodeLinkData> links = new();
        public List<BTNodeData> nodeData = new();
        public List<ExternalProperty> properties = new();
    }
}