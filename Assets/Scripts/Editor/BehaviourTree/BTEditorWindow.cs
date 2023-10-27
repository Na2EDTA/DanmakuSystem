using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using System;

public class BTEditorWindow : EditorWindow
{
    BTTreeView treeView;
    BTInspectorView inspectorView;
    BTBlackboardView blackboardView;

    [MenuItem("Behaviour Tree/Editor")]
    public static void OpenWindow()
    {
        BTEditorWindow wnd = GetWindow<BTEditorWindow>();
        wnd.titleContent = new GUIContent("BTEditorWindow");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BTTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    //实例与原型的编辑切换
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/BehaviourTree/BTEditorWindow.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTree/BTEditorWindow.uss"); 
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BTTreeView>();
        inspectorView = root.Q<BTInspectorView>();
        blackboardView = root.Q<BTBlackboardView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;
        treeView.OnDataSelected = OnDataSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BTTree tree = Selection.activeObject as BTTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                BTRuntime runtime = Selection.activeGameObject.GetComponent<BTRuntime>();
                if (runtime)
                {
                    tree = runtime.tree;
                }
            }
        }
        if (Application.isPlaying)
        {
            if (tree)
            {
                treeView?.PopulateView(tree);
                blackboardView?.UpdateSelection(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView?.PopulateView(tree);
                blackboardView?.UpdateSelection(tree);
            }
        }
    }

    //更新inspector监视对象到所选的节点
    void OnNodeSelectionChanged(BTNodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
    }
    void OnDataSelectionChanged(BTDataView dataView)
    {
        inspectorView.UpdateSelection(dataView);
    }

    //根据inspector变化更新节点状态
    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}