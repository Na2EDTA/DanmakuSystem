using UnityEditor;
using UnityEngine;

namespace Danmaku.SerializeExtension
{
    [CustomPropertyDrawer(typeof(Dictionary), true)]
    public class DictionaryDrawer : PropertyDrawer
    {
        private SerializedProperty listProperty;

        private SerializedProperty GetListProperty(SerializedProperty property)
        {
            if (listProperty == null)
                listProperty = property.FindPropertyRelative("list");

            return listProperty;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, GetListProperty(property), label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(GetListProperty(property), true);
        }
    }
}