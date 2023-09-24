/*///
/// 全几把是前端性质的活儿
///————行为树编辑窗口
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

namespace Danmaku.Deprecated
{
    public class BTGraphEditorWindow : EditorWindow
    {
        private BTGraphView _graphView;
        private string _fileName = "New Behaviour Tree";

        [MenuItem("Graph/Behaviour Tree Graph")]
        public static void BehaviourTreeGraph()
        {
            var window = GetWindow<BTGraphEditorWindow>();
            window.titleContent = new GUIContent("BT Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMinimap();
            GenerateBlackboard();
        }

        private void GenerateBlackboard()
        {
            Blackboard blackboard = new(_graphView);
            blackboard.Add(new BlackboardSection { title = "External Properties" });
            blackboard.addItemRequested = _blackboard1 =>
            {
                _graphView.AddPropertyToBlackboard(new ExternalProperty());
            };
            blackboard.editTextRequested = (b, el, newValue) =>
            {
                int repeated = 0;
                string tempName = newValue;

                var oldName = (el as BlackboardField).text;
                while (_graphView.properties.Any(x => x.name == newValue))
                {
                    repeated++;
                    newValue = $"{tempName} ({repeated})";
                }

                int propertyIndex = _graphView.properties.FindIndex(x => x.name == oldName);
                _graphView.properties[propertyIndex].name = newValue;
                (el as BlackboardField).text = newValue;
         *//*   };

            blackboard.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(blackboard);
            _graphView.blackboard = blackboard;
        }

        private void GenerateMinimap()
        {
            MiniMap miniMap = new()
            {
                anchored = true
            };
            Vector2 cords = _graphView.contentContainer.WorldToLocal(new Vector2(maxSize.x - 10, maxSize.y - 10));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        //整个工具栏，上面有些按钮
        void GenerateToolbar()
        {
            Toolbar toolbar = new();

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

            //读取文件并保存
            TextField fileNameTextField = new("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool isSaving)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid File Name", "Please enter a valid file name.", "OK");
                return;
            }

            BTGraphSaveLoadUtility utility = BTGraphSaveLoadUtility.GetInstance(_graphView);
            if (isSaving) utility.SaveGraph(_fileName);
            else utility.LoadGraph(_fileName);
        }

        //显示BT图操作界面
        void ConstructGraphView()
        {
            _graphView = new BTGraphView(this)
            {
                name = "Behaviour Tree Graph"
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
    }*//*
//}*/