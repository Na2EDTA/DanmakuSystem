using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Danmaku.BehaviourTree
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CreateOutputPortAttribute : Attribute
    {
        public string guid;

        public CreateOutputPortAttribute()
        {
            guid = GUID.Generate().ToString();
        }
    }
}
