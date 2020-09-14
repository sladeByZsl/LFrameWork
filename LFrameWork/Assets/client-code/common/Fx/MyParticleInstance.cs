using libx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticleInstance:MonoBehaviour,IManager
{
    public GameObject particleObject;

    string assetPath = "Assets/Arts/effect/prefab/{0}.prefab";

    public void Init()
    {

    }

    public void Clear()
    {
        Util.Destroy(particleObject);
    }

    internal void load(string particleName, Action callback)
    {
        Assets.LoadAssetAsync(string.Format(assetPath,particleName), typeof(UnityEngine.Object)).completed += delegate (AssetRequest request)
        {
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.error);
                return;
            }
         
            particleObject = (GameObject)Instantiate(request.asset);
            particleObject.name = request.asset.name;
            particleObject.transform.parent = this.transform;
            particleObject.transform.localPosition = Vector3.zero;
            particleObject.transform.localScale = Vector3.one;
            request.Require(particleObject);
            request.Release();
            callback?.Invoke();
        };
    }
}
