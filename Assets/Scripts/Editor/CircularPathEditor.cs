using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircularPath))]
public class CircularPathEditor : Editor
{
    List<Vector2> samples = new();
    SerializedProperty radius;
    SerializedProperty clockwise;
    SerializedProperty offset;
    SerializedProperty length;
    SerializedProperty color;
    SerializedProperty posOffset;
    int precision = 40;

    private CircularPath _target
    {
        get
        {
            return target as CircularPath;
        }
    }

    private void OnEnable()
    {
        length = serializedObject.FindProperty("length");
        color = serializedObject.FindProperty("color");
        radius = serializedObject.FindProperty("radius");
        clockwise = serializedObject.FindProperty("clockwise");
        offset = serializedObject.FindProperty("argOffset");
        posOffset = serializedObject.FindProperty("positionOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(length);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(clockwise);
        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.PropertyField(posOffset);
        EditorGUILayout.PropertyField(color);
        precision = EditorPrefs.GetInt("precision", 40);
        precision = (int)EditorGUILayout.Slider("precision", precision, 0, 200);
        EditorPrefs.SetInt("precision", precision);
        Undo.RecordObject(this, "Undo a CircularPath edit");
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        Render();
    }

    void Render()
    {
        Handles.color = _target.color;

        float l = _target.length;
        float dl = l / precision;
        samples.Clear();
        samples.Add(_target.positionOffset);
        for (int i = 1; i <= precision; i++)
        {
            float t = _target.EvaluateArgument(i * dl);
            samples.Add(_target.Evaluate(t));
            Handles.DrawLine(samples[i - 1], samples[i]);
        }

    }
}
