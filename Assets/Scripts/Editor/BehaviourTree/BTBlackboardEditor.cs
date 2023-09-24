/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BTBlackboard))]
public class BTBlackboardEditor : Editor
{
    SerializedProperty intParams, floatParams, booleanParams, triggerParams;

    private BTBlackboard _target
    {
        get
        {
            return target as BTBlackboard;
        }
    }

    private void OnEnable()
    {
        intParams = serializedObject.FindProperty("intVariables");
        floatParams = serializedObject.FindProperty("floatVariables");
        booleanParams = serializedObject.FindProperty("booleanVariables");
        triggerParams = serializedObject.FindProperty("triggerVariables");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(intParams);
        EditorGUILayout.PropertyField(floatParams);
        EditorGUILayout.PropertyField(booleanParams);
        EditorGUILayout.PropertyField(triggerParams);
        serializedObject.ApplyModifiedProperties();
    }
}
*/