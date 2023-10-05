using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
