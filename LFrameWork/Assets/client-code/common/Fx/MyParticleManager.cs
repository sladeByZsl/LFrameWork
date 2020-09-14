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
        if(mTemplete==null)
        {
            mTemplete = new GameObject("ParticleInstance");
            mTemplete.AddComponent<MyParticleInstance>();
            GameObject.DontDestroyOnLoad(mTemplete);
        }
        if(root==null)
        {
            root = new GameObject("ParticleRoot");
            rootTrans = root.transform;
            GameObject.DontDestroyOnLoad(root);
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
        
        particleInstance.Clear();
        GameObjectPoolManager.GetInstance().RecycleGameObject(particleInstance.gameObject);
    }

    public void Clear()
    {
        Util.Destroy(mTemplete);
    }
}
