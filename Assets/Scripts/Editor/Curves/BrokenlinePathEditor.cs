using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrokenlinePath))]
public class BrokenlinePathEditor : Editor
{
    private BrokenlinePath _target
    {
        get
        {
            return target as BrokenlinePath;
        }
    }

    private void OnSceneGUI()
    {
        _target.UpdateSegments();
        Render();
    }

    void Render()
    {
        Handles.color = _target.color;
        for (int i = 0; i < _target.segments.Count; i++)
        {
            Handles.DrawLine(_target.waypoints[i], _target.waypoints[i] + _target.segments[i]);
        }
        for (int i = 0; i < _target.waypoints.Count; i++)
        {
            _target.waypoints[i] = Handles.PositionHandle(_target.waypoints[i], Quaternion.identity);
            Undo.RecordObject(_target, "Undo a BrokenlinePath edit.");
        }
    }

}
