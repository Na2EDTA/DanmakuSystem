using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Velocity))]
public class VelocityEditor : Editor
{
    SerializedProperty x;
    SerializedProperty y;
    SerializedProperty magnitude;
    SerializedProperty angle;
    SerializedProperty method;
    SerializedProperty isAiming;
    SerializedProperty tar;
    SerializedProperty followTangent;
    SerializedProperty followAngle;
    SerializedProperty path;
    SerializedProperty length;

    private Velocity _target
    {
        get
        {
            return target as Velocity;
        }
    }

    private void OnEnable()
    {
        x = serializedObject.FindProperty("x");
        y = serializedObject.FindProperty("y");
        magnitude = serializedObject.FindProperty("magnitude");
        angle = serializedObject.FindProperty("angle");
        method = serializedObject.FindProperty("method");
        isAiming = serializedObject.FindProperty("isAiming");
        tar = serializedObject.FindProperty("target");
        followTangent = serializedObject.FindProperty("isFollowingTangent");
        followAngle = serializedObject.FindProperty("followAngle");
        path = serializedObject.FindProperty("path");
        length = serializedObject.FindProperty("length");
    }

    public override void OnInspectorGUI()
    {
        Velocity v = target as Velocity;
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(method);

        switch (_target.method)
        {
            case GameEnum.MovingMethod.Cartesian:
                EditorGUILayout.PropertyField(x);
                EditorGUILayout.PropertyField(y);
                break;
            case GameEnum.MovingMethod.Polar:
                EditorGUILayout.PropertyField(magnitude);
                EditorGUILayout.PropertyField(angle);
                break;
            case GameEnum.MovingMethod.Natural:
                EditorGUILayout.PropertyField(magnitude);
                EditorGUILayout.PropertyField(path);
                EditorGUILayout.PropertyField(length);
                break;
        }
        EditorGUILayout.PropertyField(isAiming);
        if(_target.IsAiming)
            EditorGUILayout.PropertyField(tar);
        EditorGUILayout.PropertyField(followTangent);
        if(_target.isFollowingTangent)
            EditorGUILayout.PropertyField(followAngle);
        serializedObject.ApplyModifiedProperties();
    }
}
