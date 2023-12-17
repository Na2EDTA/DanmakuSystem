using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using System.Linq;

public class BTEditorWindow : EditorWindow
{
    BTTreeView treeView;
    BTInspectorView inspectorView;
    BTBlackboardView blackboardView;
    ToolbarMenu menu;

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
        menu = root.Q<ToolbarMenu>();

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        treeView.OnDataSelected = OnDataSelectionChanged;

        menu.menu.AppendAction("Debug Links", LinkDebug);
        menu.menu.AppendAction("Debug Element", ElementDebug);

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

    void LinkDebug(DropdownMenuAction dropdownMenuAction)
    {
        var tree = treeView.tree;
        Debug.Log("LinkCache: Input:" + tree.linkCache.inCache.Count + ", Output:" + tree.linkCache.outCache.Count);

        tree.dataLinks.ForEach(l => Debug.Log(l.guid));
        
    }

    void ElementDebug(DropdownMenuAction dropdownMenuAction)
    {
        var elements = treeView.selection;
        elements.ForEach(ev => {
            BTElement element = null;
            if ((ev as BTElementView) != null)
                element = (ev as BTElementView).Element;
            if (element != null)
            {
                Debug.Log(element.guid + "/ " + element.GetHashCode() + ", " + element.name + "\n" +
                    "OutputFieldCaches:" + element.outputFieldCaches.Count + "\t" +
                    "InputFieldCaches:" + element.inputFieldCaches.Count);
                element.tree.FindInputLinks(element).ForEach(l => Debug.Log("getter:" + l.getter + ", setter:"+ l.setter));
            }
        });
        
    }

    void MenuDebug(DropdownMenuAction dropdownMenuAction)
    {
        
    }

    void Debug01()
    {
        var tree = treeView.tree;
        tree.elements.FindAll(e=>e.inputFieldCaches.Count!=0).ForEach(e=>Debug.Log(e.inputFieldCaches[0]));
    }

    void Debug02()
    {
        Debug.Log(treeView.tree.linkCache.outCache.Count);
        treeView.tree.linkCache.outCache.ToList().ForEach(ic => Debug.Log(ic.Key.GetHashCode()));
    }
}