using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace LFrameWork.AssetManagement
{
    public class AssetLoadRequest : IEnumerator, IGCPool
    {
        public const int Priority_Sync = 0;     //使用同步加载
        public const int Priority_Fast = 100;   //使用同步加载，不超过1帧加载数量时，下帧返回，大于它都将使用异步
        public const int Priority_Common = 1000; //异步加载
        public const int Priority_Slow = 2000;  //慢速异步
        public const int Priority_Max = 10000;   //最大优先级

        //加载路径
        public string path { get; set; }
        //加载成功的资源
        public UnityEngine.Object asset { get { return assetHandle.asset; } }

        public AssetHandle assetHandle { get; internal set; } = AssetHandle.invalid;

        public Type type { get; set; }

        public object Current { get; private set; }

        public bool isDone { get; private set; } = false;

        //加载优先级，越小越先加载
        public int priority = Priority_Common;
        //自动关联都gameObject,加载成功后，自动把资源引用关联到该gameObject
        private GameObject _autoRefGameObject;
        private bool hasAutoRefGameObject;
        public GameObject autoRefGameObject
        {
            get { return _autoRefGameObject; }
            set { _autoRefGameObject = value; hasAutoRefGameObject = value != null; }
        }
        //加载成功后执行回收，如果有回调会在回调结束后执行
        public bool autoFree { get; set; } = false;

        public bool isUrl { get; private set; }
        public string url
        {
            set
            {
                path = value;
                isUrl = true;
            }

            get
            {
                return isUrl ? path : null;
            }
        }

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
            path = null;
            isDone = false;
            onCompleted = null;
            priority = Priority_Common;
            autoRefGameObject = null;
            hasAutoRefGameObject = false;
            attachDatas = null;
            assetHandle = AssetHandle.invalid;
            type = null;
            autoFree = false;
            isUrl = false;
        }

        public delegate void OnCompleted(AssetLoadRequest req);
        public event OnCompleted onCompleted;

        private Dictionary<string, System.Object> attachDatas;

        public void AttachData(string key, System.Object data)
        {
            if (attachDatas == null)
            {
                attachDatas = new Dictionary<string, object>();
            }
            attachDatas[key] = data;
        }

        public System.Object GetData(string key)
        {
            if (attachDatas == null)
            {
                return null;
            }
            if (attachDatas.TryGetValue(key, out var ret))
            {
                return ret;
            }
            return null;
        }

        public T GetData<T>(string key) where T : class
        {
            if (attachDatas == null)
            {
                return null;
            }
            if (!attachDatas.TryGetValue(key, out var val))
            {
                return null;
            }
            return val as T;
        }

        public T GetDataValue<T>(string key, T defaultValue) where T : struct
        {
            if (attachDatas == null)
            {
                return defaultValue;
            }
            if (!attachDatas.TryGetValue(key, out var val))
            {
                return defaultValue;
            }
            return (T)val;
        }

        internal void Complete()
        {
            if (isDone)
            {
                return;
            }
            isDone = true;
            if (assetHandle.isValid)
            {
                AssetManager.GetInstance().OnLoadAssetSucc(assetHandle);
                if (hasAutoRefGameObject)
                {
                    if (autoRefGameObject != null)
                    {
                        AssetManager.GetInstance().AddAssetRefToGameObject(autoRefGameObject, assetHandle.assetRef);
                    }
                    else
                    {
                        //有自动关联的GameObject，但是GameObject已经被销毁了
                        //一般是加载过程中，加载发起者已经被销毁，例如ui关闭，场景切换
                        //这时不再调用毁掉，防止上层逻辑异常，直接返回
                        if (autoFree)
                        {
                            Free(this);
                        }
                        return;
                    }
                }
            }
            if (onCompleted != null)
            {
                try
                {
                    onCompleted(this);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.ToString());
                }
                if (autoFree)
                {
                    Free(this);

                }
            }
        }

        private static GCPool<AssetLoadRequest> gcPool = new GCPool<AssetLoadRequest>();


        //获取一个加载任务
        //默认是自动回收的（加载成功后执行回收，如果有回调会在回调结束后执行）
        //如果有手动管理的需求，需要new一个出来
        public static AssetLoadRequest Get(bool autoFree = true)
        {
            var req = gcPool.Get();
            req.autoFree = autoFree;
            return req;
        }

        //回收任务，减少gc
        public static void Free(AssetLoadRequest req)
        {
            if (AssetManager.GetInstance() != null)
            {
                AssetManager.GetInstance().PushAutoFreeRequest(req);
            }
        }

        internal static void DoFree(AssetLoadRequest req)
        {
            gcPool.Free(req);
        }
    }

    public class AssetsLoadBatch : IEnumerator
    {
        public delegate void OnBatchCompleted(AssetsLoadBatch batch);
        public OnBatchCompleted onBatchCompleted;

        protected List<AssetLoadRequest> reqs = new List<AssetLoadRequest>();
        private int loadingCount;

        private List<AssetLoadRequest> autoFreeReqs = new List<AssetLoadRequest>();

        public void AddRequest<T>(string path, GameObject autoRefGameObject = null) where T : UnityEngine.Object
        {
            AddRequest(path, typeof(T), autoRefGameObject);
        }

        public void AddRequest(string path, Type type = null, GameObject autoRefGameObject = null)
        {
            var req = AssetLoadRequest.Get();
            req.path = path;
            req.type = type;
            req.autoRefGameObject = autoRefGameObject;
            AddRequest(req);
        }

        //添加提前创建的任务
        //注意任务回调会被覆盖掉
        public void AddRequest(AssetLoadRequest req)
        {
            reqs.Add(req);
            req.onCompleted += OnRequestComplted;
            if (req.autoFree)
            {
                req.autoFree = false;
                autoFreeReqs.Add(req);
            }
        }

        public AssetLoadRequest GetRequest(string path)
        {
            foreach (var req in reqs)
            {
                if (req.path == path)
                {
                    return req;
                }
            }
            return null;
        }

        public AssetLoadRequest[] GetRequests()
        {
            return reqs.ToArray();
        }

        public void Start(OnBatchCompleted onCompleted)
        {
            onBatchCompleted = onCompleted;
            var assetMgr = AssetManager.GetInstance();
            loadingCount = reqs.Count;
            foreach (var req in reqs)
            {
                assetMgr.StartLoad(req);
            }
        }

        public bool isDone
        {
            get
            {
                return loadingCount <= 0;
            }
        }

        public object Current { get; private set; }

        private void OnRequestComplted(AssetLoadRequest req)
        {
            --loadingCount;
            if (!isDone)
            {
                return;
            }
            OnCompleted();
        }

        protected virtual void OnCompleted()
        {
            //全部加载结束
            if (onBatchCompleted != null)
            {
                onBatchCompleted(this);
            }
            //回收
            foreach (var autoFreeReq in autoFreeReqs)
            {
                AssetLoadRequest.Free(autoFreeReq);
            }
            autoFreeReqs.Clear();
        }

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
            autoFreeReqs.Clear();
            reqs.Clear();
            onBatchCompleted = null;
            loadingCount = 0;
        }
    }

    //预加载资源，会自动进行弱引用
    public class PreloadRequest : AssetsLoadBatch
    {
        protected override void OnCompleted()
        {
            var mgr = AssetManager.GetInstance();
            foreach (var req in reqs)
            {
                mgr.WeakCache(req.assetHandle);
            }
            base.OnCompleted();
        }
    }
}