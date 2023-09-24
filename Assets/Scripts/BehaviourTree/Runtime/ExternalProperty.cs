using System;
using System.Collections.Generic;
using UnityEngine;

namespace Danmaku.Deprecated
{
    [Serializable]
    public class ExternalProperty
    {
        public string name = "New Variable";
        public string value = "New String";

        public ExternalProperty()
        {

        }

        public ExternalProperty(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}