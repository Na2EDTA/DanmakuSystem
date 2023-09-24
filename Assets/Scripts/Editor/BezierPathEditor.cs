using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    private BezierPath _target
    {
        get
        {
            return target as BezierPath;
        }
    }

    private void OnSceneGUI()
    {
        _target.UpdateAllSegments(); //性能好随便开
        _target.Render();
    }
}
