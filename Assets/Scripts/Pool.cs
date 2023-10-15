using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;

public class Pool : MonoBehaviour
{
    public static Pool instance;
    [HideInInspector] public static int order = 0;//叠放次序，每个子弹唯一
    [HideInInspector] public static int objectCountOnStage = 0;//场上活跃的物体总数

    /*------------------老体系，准备拆除-----------------*/
    Dictionary<string, Queue<GameObject>> mainPool = new();
    Dictionary<string, GameObject> dict = new();
    [SerializeField] GameObject[] prefabList;
    [SerializeField] int defaultFillCount;
    [SerializeField] int defaultLeastCount;
    /*-------------------------------------------------*/

    private void Awake()
    {
        //懒汉单例不解释
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //也是老体系的一部分，把原型往池子填，原型表是手拖的，效率低下
        for (int i = 0; i < prefabList.Length; i++)
        {
            GameObject obj = prefabList[i];
            string name = obj.name;
            dict.Add(name, obj);
            
            mainPool.Add(name, new Queue<GameObject>());
        }

        //新体系的初始化过程，从Asset/Prefabs/里边自动装载符合要求的原型，省心省力
        Init();
    }

    /*-----------------------老体系的对象池基本操作部分-------------------------*/
    void Fill(string objType)//装填
    {
        for (int i = 0; i < defaultFillCount; i++)
        {
            objectCountOnStage++;
            GameObject obj = Instantiate(dict[objType], transform);
            Dispose(obj);
        }
    }
    void Fill(string objType, int fillCount)//指定了装填量的装填
    {
        for (int i = 0; i < fillCount; i++)
        {
            objectCountOnStage++;
            GameObject obj = Instantiate(dict[objType], transform);
            Dispose(obj);
        }
    }

    public GameObject Create(string objType, Vector2 pos, params float[] ps)//从池子里摇出来物体
    {
        if (mainPool[objType].Count < defaultLeastCount) Fill(objType);
        GameObject obj = mainPool[objType].Dequeue();
        if (obj)
        {
            obj.transform.position = pos;
            obj.GetComponent<DanmakuObject>().OnInit(ps);
            obj.SetActive(true);
            obj.GetComponent<SpriteRenderer>().sortingOrder = order;
            order++;
            objectCountOnStage++;
        }
        return obj;
    }

    public GameObject Create(string objType, Vector2 pos, int fillCount, int leastCount, params float[] ps)
    //也是从池子里摇出来物体，指定了存货紧缺的阈值与装填量
    {
        if (mainPool[objType].Count < leastCount) Fill(objType, fillCount);
        GameObject obj = mainPool[objType].Dequeue();
        obj.transform.position = pos;
        obj.GetComponent<DanmakuObject>().OnInit(ps);
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().sortingOrder = order;
        order++;
        objectCountOnStage++;
        return obj;
    }

    public void Dispose(GameObject obj)//从外边回收物体到池子里
    {
        obj.SetActive(false);
        string cname = obj.name;
        mainPool[cname.Remove(cname.Length - 7)].Enqueue(obj);
        objectCountOnStage--;
        return;
    }

    /*-----------------------------------新体系--------------------------------------------*/

    public Dictionary<System.Type, SubPool> objectPool = new();//主池
    [SerializeField] List<GameObject> prefabs;//这个原型表是自动完成填入的

    [ContextMenu("Init")]//在编辑器界面一键装原型表，忘了装也没关系，在编辑器里测试时也能装
    public void Init()
    {
#if UNITY_EDITOR
        objectPool.Clear();

        prefabs = new();

        FindPrefabsInDirectroy("Assets/Prefabs/", out prefabs);//寻找所有原型
#endif

        if (prefabs.Any())
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].TryGetComponent<DanmakuObject>(out var d))
                    //只保留挂载DanmakuObject组件的原型
                {
                    System.Type type = d.GetType();

                    if (objectPool.Keys.Contains(type))//如果重复了就跳过
                        continue;
                    else//不然就将DanmakuObject组件的具体类型当做键，原型与分池的组合当做值，填入作为主池的字典。
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
        
        //找到所有嵌套层级的子文件夹
        string[] subfolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        string[] guids = null;
        int index = 0, max = 0;
        {
            guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            //主文件夹中找文件
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
            //子文件夹中找文件
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
