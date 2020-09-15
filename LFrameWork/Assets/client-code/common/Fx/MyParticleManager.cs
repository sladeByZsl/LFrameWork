using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticleManager : SingletonBase<MyParticleManager>, IManager
{
    public GameObject mTemplete;
    private Dictionary<string, MyParticleInstance> partilceDic=new Dictionary<string, MyParticleInstance>();

    public GameObject root;
    public Transform rootTrans;

    public void Init()
    {
        if(root==null)
        {
            root = new GameObject("ParticleRoot");
            rootTrans = root.transform;
            GameObject.DontDestroyOnLoad(root);
        }
        if (mTemplete == null)
        {
            mTemplete = new GameObject("ParticleTemplete");
            mTemplete.AddComponent<MyParticleInstance>();
            mTemplete.transform.parent = rootTrans;
            mTemplete.transform.localPosition = Vector3.zero;
            mTemplete.transform.localScale = Vector3.one;
            mTemplete.SetActive(false);
            GameObject.DontDestroyOnLoad(mTemplete);
        }
    }

    public MyParticleInstance GetParticleInstance(string particleName,Action callback)
    {
        GameObject gameObject = GameObjectPoolManager.GetInstance().GetGameObject(mTemplete, rootTrans);
        MyParticleInstance pi = gameObject.GetComponent<MyParticleInstance>();
        pi.load(particleName, callback);
        pi.gameObject.transform.parent = rootTrans;
        pi.gameObject.transform.localPosition = Vector3.zero;
        pi.gameObject.SetActive(true);
        pi.gameObject.name = particleName;
        return pi;
    }

    public void AddParticleToCache(MyParticleInstance particleInstance)
    {
        GameObjectPoolManager.GetInstance().RecycleGameObject(particleInstance.gameObject);
        particleInstance.Clear();
    }

    public void Clear()
    {
        Util.Destroy(mTemplete);
    }
}
