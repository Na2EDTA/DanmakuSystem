using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;

public class Pool : MonoBehaviour
{
    public static Pool instance;
    [HideInInspector] public static int order = 0; 
    Dictionary<string, Queue<GameObject>> mainPool = new();
    Dictionary<string, GameObject> dict = new();
    [SerializeField] GameObject[] prefabList;
    [SerializeField] int defaultFillCount;
    [SerializeField] int defaultLeastCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        for (int i = 0; i < prefabList.Length; i++)
        {
            GameObject obj = prefabList[i];
            string name = obj.name;
            dict.Add(name, obj);
            
            mainPool.Add(name, new Queue<GameObject>());
        }
    }

    void Fill(string objType)
    {
        for (int i = 0; i < defaultFillCount; i++)
        {
            GameObject obj = Instantiate(dict[objType], transform);
            Dispose(obj);
        }
    }
    void Fill(string objType, int fillCount)
    {
        for (int i = 0; i < fillCount; i++)
        {
            GameObject obj = Instantiate(dict[objType], transform);
            Dispose(obj);
        }
    }

    public GameObject Create(string objType, Vector2 pos, params float[] ps)
    {
        if (mainPool[objType].Count < defaultLeastCount) Fill(objType);
        GameObject obj = mainPool[objType].Dequeue();
        if (obj)
        {
            obj.transform.position = pos;
            obj.GetComponent<DanmakuObject>().InitParams(ps);
            obj.SetActive(true);
            obj.GetComponent<SpriteRenderer>().sortingOrder = order;
            order++;
        }
        return obj;
    }

    public GameObject Create(string objType, Vector2 pos, int fillCount, int leastCount, params float[] ps)
    {
        if (mainPool[objType].Count < leastCount) Fill(objType, fillCount);
        GameObject obj = mainPool[objType].Dequeue();
        obj.transform.position = pos;
        obj.GetComponent<DanmakuObject>().InitParams(ps);
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().sortingOrder = order;
        order++;
        return obj;
    }

    public void Dispose(GameObject obj)
    {
        obj.SetActive(false);
        string cname = obj.name;
        mainPool[cname.Remove(cname.Length - 7)].Enqueue(obj);
        return;
    }

    /*-------------------------------------------------------------------------------*/

    [SerializeField] public Danmaku.SerializeExtension.Dictionary<System.Type, Queue<GameObject>> objectPool = new();
    Dictionary<System.Type, GameObject> objectList = new();

#if UNITY_EDITOR
    public void Init()
    {
        List<GameObject> prefabs = new();
        FindPrefabsInDirectroy("Assets/Prefabs/", out prefabs);
        
        if (prefabs.Any())
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                TryGetComponent<DanmakuObject>(out var d);
                System.Type type = d.GetType();
                if (objectPool.Keys.Contains(type))
                    continue;
                else
                {
                    objectPool.Add(type, new());
                    objectList.Add(type, prefabs[i]);
                }
            }
        }
    }

    static void FindPrefabsInDirectroy(string path, out List<GameObject> output)
    {
        output = new();

        if (string.IsNullOrEmpty(path)) throw new System.ArgumentException("Path is null or empty");
        string[] subfolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        string[] guids = null;
        string[] assetPaths = null;
        for (int i = 0; i < subfolders.Length; i++)
        {
            guids = AssetDatabase.FindAssets("t:Prefab");
            assetPaths = new string[guids.Length];
            //sth
        }

        output = null;
    }
#endif

    void Fill<T>(int count = 50)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(objectList[typeof(T)], transform);
            Dispose<T>(obj);
        }
    }

    public GameObject Create<T>(Vector2 pos, params object[] ps) where T: DanmakuObject
    {
        Queue<GameObject> queue = objectPool[typeof(T)];
        if (queue.Count < 10) Fill<T>();
        var obj = queue.Dequeue();
        obj.transform.position = pos;
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().sortingOrder = order;
        order++;
        return obj;
    }

    public void Dispose<T>(GameObject obj)
    {
        obj.SetActive(false);
        objectPool[typeof(T)].Enqueue(obj);
    }
}
