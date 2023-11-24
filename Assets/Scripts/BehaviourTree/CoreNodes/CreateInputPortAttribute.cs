using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Danmaku.BehaviourTree
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CreateInputPortAttribute : Attribute
    {
        public string guid;

        public CreateInputPortAttribute()
        {
            guid = GUID.Generate().ToString();
        }
    }
}