using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;

public class Pool : MonoBehaviour
{
    public static Pool instance;
    [HideInInspector] public static int order = 0;//��ߵ��Ŵ���ÿ���ӵ�Ψһ
    [HideInInspector] public static int objectCountOnStage = 0;//���ϻ�Ծ����������

    private void Awake()
    {
        //��������������
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //����ϵ�ĳ�ʼ�����̣���Asset/Prefabs/����Զ�װ�ط���Ҫ���ԭ�ͣ�ʡ��ʡ��
        Init();
    }

    public Dictionary<System.Type, SubPool> objectPool = new();//����
    [SerializeField] List<GameObject> prefabs;//���ԭ�ͱ����Զ���������

    [ContextMenu("Init")]//�ڱ༭������һ��װԭ�ͱ�����װҲû��ϵ���ڱ༭�������ʱҲ��װ
    public void Init()
    {
#if UNITY_EDITOR
        objectPool.Clear();

        prefabs = new();

        FindPrefabsInDirectroy("Assets/Prefabs/", out prefabs);//Ѱ������ԭ��
#endif

        if (prefabs.Any())
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].TryGetComponent<DanmakuObject>(out var d))
                    //ֻ��������DanmakuObject�����ԭ��
                {
                    System.Type type = d.GetType();

                    if (objectPool.Keys.Contains(type))//����ظ��˾�����
                        continue;
                    else//��Ȼ�ͽ�DanmakuObject����ľ������͵�������ԭ����ֳص���ϵ���ֵ��������Ϊ���ص��ֵ䡣
                        objectPool.Add(type, new() { prefab = prefabs[i], queue = new() });
                }
            }
        }
    }
#if UNITY_EDITOR
    static void FindPrefabsInDirectroy(string path, out List<GameObject> output)
    {
        output = new();

        if (string.IsNullOrEmpty(path)) throw new System.ArgumentException("Path is null or empty");
        
        //�ҵ�����Ƕ�ײ㼶�����ļ���
        string[] subfolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        string[] guids = null;
        int index = 0, max = 0;
        {
            guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            //���ļ��������ļ�
            for (index = 0, max = guids.Length; index < max; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[index]);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                output.Add(go);
            }
        }
        for (int i = 0; i < subfolders.Length; i++)
        {
            guids = AssetDatabase.FindAssets("t:Prefab", new string[] { subfolders[i]});
            //���ļ��������ļ�
            for (index = 0, max=guids.Length; index < max; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[index]);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                output.Add(go);
            }
        }
    }
#endif

    void Fill<T>(int count = 50, int leastCount = 15) where T : DanmakuObject
    {
        SubPool pool = objectPool[typeof(T)];

        if (pool.queue.Count < leastCount)
        {
            for (int i = 0; i < count; i++)
            {
                objectCountOnStage++;
                var obj = Instantiate(pool.prefab, transform);
                Dispose<T>(obj);
            }
        }
    }

    void Fill(string type, int count = 50, int leastCount = 15)
    {
        var t = System.Type.GetType(type);
        SubPool pool = objectPool[t];
        if (pool.queue.Count < leastCount)
        {
            for (int i = 0; i < count; i++)
            {
                objectCountOnStage++;
                var obj = Instantiate(pool.prefab, transform);
                Dispose(type, obj);
            }
        }
    }

    public GameObject Create<T>(Vector2 pos, int count = 50, int leastCount = 15, params float[] ps) where T: DanmakuObject
    {

        Queue<GameObject> queue = objectPool[typeof(T)].queue;

        Fill<T>(count, leastCount);
        var obj = queue.Dequeue();
        obj.GetComponent<T>().OnInit(ps);
        obj.transform.position = pos;
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().sortingOrder = order;
        order++;
        objectCountOnStage++;
        return obj;
        
    }

    public GameObject Create(Vector2 pos, string type, int count = 50, int leastCount = 15, params float[] ps)
    {
        Queue<GameObject> queue = objectPool[System.Type.GetType(type)].queue;
        Fill(type, count, leastCount);
        var obj = queue.Dequeue();
        (obj.GetComponent(type) as DanmakuObject).OnInit(ps);
        obj.transform.position = pos;
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().sortingOrder = order;
        order++;
        objectCountOnStage++;
        return obj;
    }

    public void Dispose<T>(GameObject obj) where T : DanmakuObject
    {
        obj.SetActive(false);
        objectPool[typeof(T)].queue.Enqueue(obj);
        objectCountOnStage--;
    }

    public void Dispose(string type, GameObject obj)
    {
        obj.SetActive(false);
        objectPool[System.Type.GetType(type)].queue.Enqueue(obj);
        objectCountOnStage--;
    }

    public void Dispose(GameObject obj, params float[] useless)
    {
        obj.SetActive(false);
        objectPool[obj.GetComponent<DanmakuObject>().GetType()].queue.Enqueue(obj);
        objectCountOnStage--;
    }

    [System.Serializable]
    public class SubPool
    {
        public GameObject prefab;
        public Queue<GameObject> queue;
    }
}
