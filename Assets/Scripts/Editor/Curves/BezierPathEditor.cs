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
        _target.UpdateAllSegments(); 
        _target.Render();
    }
}
