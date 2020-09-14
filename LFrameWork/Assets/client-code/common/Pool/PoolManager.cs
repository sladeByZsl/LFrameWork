using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : SingletonBase<GameObjectPoolManager>,IManager
{
    private Dictionary<string, Queue<GameObject>> mPoolDic;
    private Dictionary<GameObject, string> mGOTagDic = null;
    private GameObject mRoot;    private Transform mTrans;



    public void Init()
    {
        mPoolDic = new Dictionary<string, Queue<GameObject>>();
        mGOTagDic = new Dictionary<GameObject, string>();
        mRoot = GameObject.Find("PoolManager");        if (mRoot == null)        {            mRoot = new GameObject("PoolManager");        }        mTrans = mRoot.transform;
    }

    public void PreloadGameObject(GameObject prefab, int count = 1)
    {
        string tag = prefab.GetInstanceID().ToString();
        for (int i = 0; i < count; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            MarkAsOut(go, tag);
            RecycleGameObject(go);
        }
    }


    /// <summary>
    /// 从缓存池获取实例对象
    /// </summary>
    /// <param name="prefab">预制体</param>
    /// <param name="parent">关联的父节点</param>
    /// <returns></returns>
    public GameObject GetGameObject(GameObject prefab, Transform parent = null)
    {
        string tag = prefab.GetInstanceID().ToString();
        GameObject go = GetGameObject(tag);
        if (go == null)
        {
            go = GameObject.Instantiate<GameObject>(prefab, parent);
        }
        else
        {
            go.transform.SetParent(parent, false);
        }
        go.SetActive(true);
        MarkAsOut(go, tag);
        return go;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private GameObject GetGameObject(string tag)
    {
        if (mPoolDic.ContainsKey(tag) && mPoolDic[tag].Count > 0)
        {
            GameObject obj = mPoolDic[tag].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    /// <summary>
    /// 延迟回收
    /// </summary>
    /// <param name="go"></param>
    /// <param name="delay"></param>
    public void RecycleGameObject(GameObject go, float delay)
    {
        Scheduler.Delay(delay, () => {
            RecycleGameObject(go);
        });
    }

    /// <summary>
    /// 回收缓存池里的对象
    /// </summary>
    /// <param name="go"></param>
    public void RecycleGameObject(GameObject go)
    {
        if (go == null)
            return;

        go.SetActive(false);
        go.transform.SetParent(mTrans, false);

        string tag = null;
        mGOTagDic.TryGetValue(go, out tag);
        if (tag == null)
        {
            return;
        }
        RemoveOutMark(go);
        if (mPoolDic.ContainsKey(tag) == false)
        {
            mPoolDic[tag] = new Queue<GameObject>();
        }

        mPoolDic[tag].Enqueue(go);
    }

    void MarkAsOut(GameObject go, string tag)
    {
        mGOTagDic.Add(go, tag);
    }

    void RemoveOutMark(GameObject go)
    {
        if (mGOTagDic.ContainsKey(go))
        {
            mGOTagDic.Remove(go);
        }
        else
        {
            Debug.LogError("Remove out mark erro, gameObject has not been marked");
        }
    }

    public void CleanUp()
    {
        foreach (var pair in mPoolDic)
        {
            var queue = pair.Value;
            while (queue.Count > 0)
            {
                var inst = queue.Dequeue();
                if (inst != null)
                    GameObject.Destroy(inst);
            }
        }
        mPoolDic.Clear();
        mGOTagDic.Clear();
    }

    public void GC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void Clear()
    {
        CleanUp();
        GC();
    }
}
