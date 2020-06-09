
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using UnityEngine.Networking;

namespace LFrameWork.AssetManagement
{
    using Resources = UnityEngine.Resources;


    public class AbLoader
    {
        public static bool OUTPUT_LOG =
#if UNITY_EDITOR
        true;
#else
        false;
#endif //UNITY_EDITOR


        private class AbInfo
        {
            //assetBundle
            public AssetBundle ab;
            //加载时，依赖的内容
            public AssetRefList refList;
            //最近使用的帧数，超过一定帧后才触发删除
            public int usedFrame;
            //没用引用后，超过一定帧后才触发删除
            public int unusedFrame;
            //io时用的加密流
            public Stream stream;
            public Dictionary<string, Object> loadedAssets;
        }

        public string rootPath { get; private set; }

        public string LoaderName { get; private set; }

        private AssetBundleManifest manifest;
        private Stream manifestStream = null;
        private Dictionary<string, AbInfo> loadedAbs = new Dictionary<string, AbInfo>();

        // <asetPath, bundleName>
        private AssetBundleMap bundleMap = new AssetBundleMap();

        //异步加载时使用的ab，这时刚刚加载的ab，保证不能释放掉
        private HashSet<string> usingAbs = new HashSet<string>();

        //缓存依赖查询，减少gc
        private Dictionary<string, HashSet<string>> cachedDeps = new Dictionary<string, HashSet<string>>();

        //缓存所有的ab名，用来判断ab包是否存在
        //private HashSet<string> allAbNames = new HashSet<string>();

        private AssetRefManager assetRefMgr;
        private AssetRefManager bundleRefMgr;
#if UNITY_EDITOR
        public AssetRefManager RefManager => assetRefMgr;
        public AssetRefManager BundleRefMgr => bundleRefMgr;
#endif

        //无引用时的缓存
        private LRUCache lruCache;
        private int cacheSize;

        //自动卸载一帧一次最多卸载的数量
        public int autoUnloadMaxCount = 5;
        //自动卸载一帧消耗最久用时（毫秒）
        public float autoUnloadMaxMilliSecUse = 3.0f;
        //自动卸载，完整卸载完成后，下次卸载间隔
        public float autoUnloadFullUnloadInterval = 5.0f;
        public int fastAsyncLoadAbCountPerFrame = 0;
        public int maxAsyncLoadAbCountPerFrame = -1;

        //检查某个ab是否需要缓存
        public CheckCache checkCache;
        private BundleStreamCreator createEncyptStream;
        private bool released = false;

        private class InternalAsyncAbLoadRequest
        {
            public string abPath;
            public event Action<AbInfo> completed;
            public AbInfo loadingInfo;
            public int priority;
            public AssetBundleCreateRequest abReq;
            public int depLoadingCount;
            public AbLoader loader;
            private bool selfLoaded = false;

            public void OnDepLoaded(AbInfo loaded)
            {
                depLoadingCount--;
                if (loadingInfo.refList == null)
                {
                    loadingInfo.refList = new AssetRefList();
                }
                //引用这个ab
                loadingInfo.refList.AddRef(loader.bundleRefMgr.GetOrCreateRef(loaded.ab));
                TryComplete();
            }

            public void OnSelfLoaded()
            {
                selfLoaded = true;
                TryComplete();
            }

            private void TryComplete()
            {
                if (depLoadingCount == 0 && selfLoaded)
                {
                    loader.loadedAbs.Add(abPath, loadingInfo);
                    if (completed != null)
                    {
                        completed(loadingInfo);
                    }
                }
            }
        }

        public class AsyncAbLoadRequest : IEnumerator
        {
            public AssetHandle assetHandle;
            internal bool loaded;

            public object Current { get; private set; }

            public bool MoveNext()
            {
                return !loaded;
            }

            public void Reset()
            {
                loaded = false;
                assetHandle = AssetHandle.invalid;
            }
        }

        private PriorityQueue<InternalAsyncAbLoadRequest> asyncAbQueue = new PriorityQueue<InternalAsyncAbLoadRequest>();
        private Dictionary<string, InternalAsyncAbLoadRequest> loadingAsyncAbs = new Dictionary<string, InternalAsyncAbLoadRequest>();

        public bool Init(string path, int lruSize, BundleStreamCreator encypt_)
        {

            _Init(path, lruSize, encypt_);
            LoaderName = Path.GetFileName(rootPath);
            IsLoadFromWeb = false;
            //加载ab映射关系
            AssetBundleMap.LoadTxt(rootPath, bundleMap);

            //加载manifest
            AssetBundle ab = AssetBundle.LoadFromFile(GetFullPath(Path.GetFileName(rootPath)));
            if (ab == null)
            {
                Debug.LogError("Load Manifest failed." + rootPath);
                return false;
            }
            manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null)
            {
                ab.Unload(true);
                Debug.LogError("Load Manifest failed.");
                return false;
            }

            return true;
        }

        //TODO,Web加载的ab，增加加密
        public IEnumerator InitFromWeb(string path, int lruSize, BundleStreamCreator bundleStreamCtor)
        {
            if (bundleStreamCtor != null)
            {
                throw new Exception("load ab from web not support encrypt now.");
            }
            _Init(path, lruSize, bundleStreamCtor);
            IsLoadFromWeb = true;
            LoaderName = Path.GetFileName(rootPath);
            yield return AssetBundleMap.LoadFromWeb(path, bundleMap);
            using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                yield return uwr.SendWebRequest();
                AssetBundle ab = DownloadHandlerAssetBundle.GetContent(uwr);
                if (ab == null)
                {
                    Debug.LogError("Load Manifest failed." + rootPath);
                    yield break;
                }
                manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                {
                    ab.Unload(true);
                    Debug.LogError("Load Manifest failed.");
                    yield break;
                }
            }
        }

        private void _Init(string path, int lruSize, BundleStreamCreator bundleStreamCtor)
        {
            released = false;
            createEncyptStream = bundleStreamCtor;
            manifest = null;
            loadedAbs.Clear();
            usingAbs.Clear();
            this.cacheSize = lruSize;
            lruCache = new LRUCache(lruSize);
            //allAbNames.Clear();
            assetRefMgr = new AssetRefManager("AssetRefManager");
            bundleRefMgr = new AssetRefManager("BundleRefManager");
            this.rootPath = path;
        }

        public void Release()
        {
            released = true;
            usingAbs.Clear();
            assetRefMgr.Clear();
            bundleRefMgr.Clear();
            UnloadUnusedTotal();
            if (manifest != null)
            {
                Resources.UnloadAsset(manifest);
            }
            manifest = null;
            loadedAbs.Clear();

            if (manifestStream != null)
            {
                manifestStream.Close();
                manifestStream = null;
            }
        }


        public List<AssetHandle> LoadAllShaders()
        {
            List<AssetHandle> handleList = new List<AssetHandle>();

            AssetHandle assetHandle = LoadAssetBundle(AssetConst.ShaderBundleName);
            AssetBundle shaderBundle = assetHandle.asset as AssetBundle;
            if (shaderBundle != null)
            {
                string[] names = shaderBundle.GetAllAssetNames();
                if (names != null)
                {
                    for (int i = 0; i < names.Length; ++i)
                    {
                        if (names[i].EndsWith(AssetConst.ShaderExtensions))
                        {
                            string shaderName = names[i];
                            Shader shader = shaderBundle.LoadAsset<Shader>(shaderName);
                            var assetRef = CreateAssetRef(shaderName, shader, shaderBundle);
                            handleList.Add(new AssetHandle(shader, assetRef));
                        }
                    }
                }
            }

            return handleList;
        }

        public AssetHandle LoadAssetBundle(string abPath)
        {
            var ab = LoadAb(abPath);
            if (ab == null)
            {
                return AssetHandle.invalid;
            }
            var abRef = bundleRefMgr.GetOrCreateRef(ab);
            return new AssetHandle(ab, abRef);
        }

        public AsyncAbLoadRequest LoadAssetBundleAsync(string abPath, Action<AsyncAbLoadRequest> completed = null, int priority = AssetLoadRequest.Priority_Common)
        {
            AsyncAbLoadRequest req = new AsyncAbLoadRequest();
            req.loaded = false;
            LoadAbAsync(abPath, (info) => {
                req.loaded = true;
                var ab = info.ab;
                if (ab == null)
                {
                    req.assetHandle = AssetHandle.invalid;
                }
                req.assetHandle = new AssetHandle(ab, bundleRefMgr.GetOrCreateRef(ab));
                if (completed != null)
                {
                    completed(req);
                }
            }, priority);
            return req;
        }

        public bool TryGetLoaded(string abPath, string assetName, out AssetHandle assetHandle)
        {
            assetHandle = AssetHandle.invalid;
            if (!loadedAbs.TryGetValue(abPath, out var info))
            {
                return false;
            }
            if (string.IsNullOrEmpty(assetName))
            {
                assetHandle = new AssetHandle(info.ab, bundleRefMgr.GetOrCreateRef(info.ab));
                return true;
            }
            if (info.loadedAssets == null)
            {
                return false;
            }
            if (info.loadedAssets.TryGetValue(assetName, out var asset))
            {
                if (checkCache(abPath))
                {
                    lruCache.Put(abPath);
                }
                assetHandle = new AssetHandle(asset, CreateAssetRef(assetName, asset, info.ab));
                return true;
            }
            return false;
        }

        public AssetHandle LoadFromAb(string abPath, string assetName, Type type)
        {
            //从ab包重加载资源，返回资源，和一个引用
            AssetBundle ab = LoadAb(abPath);
            if (ab == null)
            {
                return AssetHandle.invalid;
            }
            if (string.IsNullOrEmpty(assetName))
            {
                //加载的是ab
                return new AssetHandle(ab, bundleRefMgr.GetOrCreateRef(ab));
            }
            Object asset = type == null ? ab.LoadAsset(assetName) : ab.LoadAsset(assetName, type);
            if (asset != null)
            {
                //加载成功
                var assetRef = CreateAssetRef(assetName, asset, ab);
                return new AssetHandle(asset, assetRef);
            }
            return AssetHandle.invalid;
        }

        public AssetHandle LoadFromAb<T>(string abPath, string assetName) where T : Object
        {
            AssetBundle ab = LoadAb(abPath);
            if (ab == null)
            {
                return AssetHandle.invalid;
            }
            T asset = ab.LoadAsset<T>(assetName);
            if (asset != null)
            {
                //加载成功
                var assetRef = CreateAssetRef(assetName, asset, ab);
                return new AssetHandle(asset, assetRef);
            }
            return AssetHandle.invalid;
        }

        private int syncLoadAbCount = 0;
        public bool LoadFromAbAsync(string abPath, string assetName, AssetLoadRequest req)
        {
            if (lastFrame < Time.frameCount)
            {
                lastFrame = Time.frameCount;
                syncLoadAbCount = 0;
            }
            if (syncLoadAbCount >= fastAsyncLoadAbCountPerFrame || req.priority >= AssetLoadRequest.Priority_Slow)
            {
                LoadAbAsync(abPath, (info) => {
                    _LoadFromAbAsync(abPath, info.ab, assetName, req);
                }, req.priority);
                return true;
            }
            AssetBundle ab = LoadAb(abPath);
            if (ab == null)
            {
                return false;
            }
            _LoadFromAbAsync(abPath, ab, assetName, req);
            return true;
        }

        private void _LoadFromAbAsync(string abPath, AssetBundle ab, string assetName, AssetLoadRequest req)
        {
            if (ab == null)
            {
                //Debug.LogError("load ab failed." + abPath);
                req.Complete();
                return;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                req.assetHandle = new AssetHandle(ab, bundleRefMgr.GetOrCreateRef(ab));
                req.Complete();
                return;
            }

            var loadReq = req.type == null ? ab.LoadAssetAsync(assetName)
                : ab.LoadAssetAsync(assetName, req.type);
            if (loadReq == null)
            {
                Debug.LogError("load from ab failed." + abPath + "," + assetName);
                req.Complete();
                return;
            }

            usingAbs.Add(abPath);
            //Debug.Log("start load from." + abPath + "," + assetName);
            //assetbundle优先级是越大越先加载
            loadReq.priority = AssetLoadRequest.Priority_Max - req.priority;
            loadReq.completed += (opt) =>
            {
                var asset = loadReq.asset;
                if (asset != null)
                {
                    var handle = new AssetHandle(asset, CreateAssetRef(assetName, asset, ab));
                    req.assetHandle = handle;
                }

                if (asset == null)
                {
                    if (OUTPUT_LOG)
                    {
                        UnityEngine.Debug.LogWarning("load failed:" + abPath + "/" + assetName);
                        if (!ab.Contains(assetName))
                        {
                            UnityEngine.Debug.LogError("ab not contains asset." + assetName);
                        }
                    }
                }

                req.Complete();
                usingAbs.Remove(abPath);
            };
        }

        private AssetRef CreateAssetRef(string assetName, Object asset, AssetBundle ab)
        {
            var assetRef = assetRefMgr.GetOrCreateRef(asset);
            assetRef.linkRef = bundleRefMgr.GetOrCreateRef(ab);
            if (loadedAbs.TryGetValue(ab.name, out var info))
            {
                if (info.loadedAssets == null)
                {
                    info.loadedAssets = new Dictionary<string, Object>();
                }
                info.loadedAssets[assetName] = asset;
            }

            return assetRef;
            // return abRefMgr.GetOrCreateRef(ab);
        }

        private HashSet<string> GetDirectDependencies(string abPath)
        {
            HashSet<string> depSet;
            if (cachedDeps.TryGetValue(abPath, out depSet))
            {
                return depSet;
            }
            string[] deps = manifest.GetDirectDependencies(abPath);
            depSet = new HashSet<string>(deps);
            depSet.Remove(abPath);      //防止自己引用自己
            cachedDeps[abPath] = depSet;
            return depSet;
        }

        //加载一个ab包，并且加载他的依赖
        private AssetBundle LoadAb(string abPath)
        {
            if (IsLoadFromWeb)
            {
                throw new Exception("can not load sync from web.");
            }
            AbInfo info = LoadAbStep(abPath);
            if (info != null)
            {
                return info.ab;
            }
            return null;
        }

        private AbInfo LoadAbStep(string abPath, int level = 0)
        {
            bool isLoaded = false;
            AbInfo inf = _LoadAb(abPath, level, out isLoaded);
            if (inf == null)
            {
                return null;
            }
            if (isLoaded)
            {
                return inf;
            }

            var deps = GetDirectDependencies(abPath);

            foreach (string dep in deps)
            {
                AbInfo depAb = LoadAbStep(dep, level + 1);
                if (depAb == null)
                {
                    continue;
                }
                if (inf.refList == null)
                {
                    inf.refList = new AssetRefList();
                }
                //引用这个ab
                inf.refList.AddRef(bundleRefMgr.GetOrCreateRef(depAb.ab));
            }

            return inf;
        }

        private void LoadAbAsync(string abPath, Action<AbInfo> completed, int priority, int level = 0)
        {
            if (loadedAbs.TryGetValue(abPath, out var info))
            {
                completed(info);
                return;
            }


            if (loadingAsyncAbs.TryGetValue(abPath, out var loadingReq))
            {
                loadingReq.completed += completed;
                return;
            }

            var deps = GetDirectDependencies(abPath);
            int loadingDepCount = deps.Count;

            if (level == 0)
            {
                if (OUTPUT_LOG)
                {
                    Debug.LogFormat("<color='green'>Load Ab <color='red'>Async</color>:{0}.{1}</color>", abPath, Time.frameCount);
                }
            }
            else
            {
                if (OUTPUT_LOG)
                {
                    string levelStr = new string('-', level * 4);
                    Debug.LogFormat("<color='blue'>-{0}>Load dep Ab <color='red'>Async</color>:{1}.</color>", levelStr, abPath);
                }
            }

            if (checkCache(abPath))
            {
                lruCache.Put(abPath);
            }

            AbInfo inf = new AbInfo();
            inf.usedFrame = Time.frameCount;

            InternalAsyncAbLoadRequest req = new InternalAsyncAbLoadRequest();
            req.completed += completed;
            req.loader = this;
            req.abPath = abPath;
            req.loadingInfo = inf;
            req.priority = priority;
            req.depLoadingCount = loadingDepCount;

            loadingAsyncAbs.Add(abPath, req);
            if (maxAsyncLoadAbCountPerFrame <= 0)
            {
                DoLoadAbAsync(req);
            }
            else
            {
                //压入队列
                asyncAbQueue.Enqueue(priority, req);
            }

            foreach (string dep in deps)
            {
                LoadAbAsync(dep, req.OnDepLoaded, priority, level + 1);
            }
        }

        private bool _UnloadAb(string abPath)
        {
            AbInfo info;

            if (!loadedAbs.TryGetValue(abPath, out info))
            {
                return false;
            }

            //正在异步加载的，不可以被卸载
            if (usingAbs.Contains(abPath))
            {
                return false;
            }

            //销毁资源引用
            if (info.loadedAssets != null)
            {
                foreach (var pair in info.loadedAssets)
                {
                    assetRefMgr.DestroyRef(pair.Value);
                }
                info.loadedAssets.Clear();
                info.loadedAssets = null;
            }
            //销毁ab引用
            bundleRefMgr.DestroyRef(info.ab);
            //释放所有对依赖的引用
            if (info.refList != null)
            {
                info.refList.ClearRef();
            }
            info.refList = null;
            loadedAbs.Remove(abPath);
            info.ab.Unload(true);
            info.ab = null;
            if (info.stream != null)
            {
                info.stream.Close();
                info.stream = null;
            }

            if (!released)
            {
                if (OUTPUT_LOG)
                {
                    Debug.Log("<color='red'>[unload ab:" + abPath + "," + Time.frameCount + "</color>");
                }
            }

            //这里不做不立即释放依赖，依赖等下次自动释放

            return true;
        }

        public bool HasBundleRef(string abPath)
        {
            AbInfo ab;
            if (!loadedAbs.TryGetValue(abPath, out ab))
            {
                return false;
            }
            return HasBundleRef(ab);
        }

        private bool HasBundleRef(AbInfo info)
        {
            //这个ab还有依赖引用，不能释放
            if (bundleRefMgr.HasRef(info.ab))
            {
                return true;
            }

            return false;
        }

        private int lastFrame = 0;
        //加载一个Ab，上层处理引用
        private AbInfo _LoadAb(string abPath, int level, out bool isLoaded)
        {
            isLoaded = false;

#if UNITY_EDITOR
            OnStartLoad(abPath);
#endif

            AbInfo info;
            if (loadedAbs.TryGetValue(abPath, out info))
            {
                info.usedFrame = Time.frameCount;
                info.unusedFrame = 0;
                isLoaded = true;
#if UNITY_EDITOR
                TestIfHitCache(abPath);
#endif
                return info;
            }

            // if (!allAbNames.Contains(abPath))
            // {
            //     return null;
            // }

            syncLoadAbCount++;

            if (loadingAsyncAbs.TryGetValue(abPath, out var asyncReq))   //处于异步加载中
            {
                loadingAsyncAbs.Remove(abPath);
                var asyncInfo = asyncReq.loadingInfo;
                var loadingAb = asyncReq.abReq.assetBundle; //触发同步加载
                asyncInfo.ab = loadingAb;
                return asyncInfo;
            }

            string fullPath = GetFullPath(abPath);
            AssetBundle ab = null;
            Stream stream = null;

            try
            {
                if (createEncyptStream != null)
                {
                    stream = createEncyptStream(fullPath);
                    ab = AssetBundle.LoadFromStream(stream);
                }
                else
                {
                    ab = AssetBundle.LoadFromFile(fullPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ab = null;
            }

            if (level == 0)
            {
                if (OUTPUT_LOG)
                {
                    Debug.LogFormat("<color='green'>Load Ab:{0}. {1}</color>", abPath, Time.frameCount);
                }
            }
            else
            {
                if (OUTPUT_LOG)
                {
                    string levelStr = new string('-', level * 4);
                    Debug.LogFormat("<color='blue'>-{0}>Load dep Ab:{1}.</color>", levelStr, abPath);
                }
            }

            if (ab == null)
            {
                //Debug.LogError("LoadAssetBundle faild " + fullPath);
                return null;
            }

            AbInfo inf = new AbInfo();
            inf.ab = ab;
            inf.usedFrame = Time.frameCount;
            inf.stream = stream;

            loadedAbs.Add(abPath, inf);
            if (checkCache(abPath))
            {
                lruCache.Put(abPath);
            }

            return inf;
        }

        private bool DoLoadAbAsync(InternalAsyncAbLoadRequest req)
        {
            if (loadedAbs.TryGetValue(req.abPath, out var info))
            {
                loadingAsyncAbs.Remove(req.abPath);
                req.OnSelfLoaded();
                return false;
            }
            else
            {
                if (IsLoadFromWeb)
                {
                    DoAsyncLoadFromWeb(req);
                }
                else
                {
                    DoAsyncLoadAb(req);
                }
                return true;
            }
        }

        public IEnumerator ProcessAsyncLoadAb()
        {
            while (true)
            {
                int frameLoadAbCount = 0;
                while (!asyncAbQueue.IsEmpty())
                {
                    var req = asyncAbQueue.Dequeue();
                    if (DoLoadAbAsync(req))
                    {
                        frameLoadAbCount++;
                    }
                    if (frameLoadAbCount >= maxAsyncLoadAbCountPerFrame)
                    {
                        frameLoadAbCount = 0;
                        yield return null;
                    }
                }
                yield return null;
            }
        }

        private void DoAsyncLoadAb(InternalAsyncAbLoadRequest req)
        {
            Stream stream = null;
            AbInfo inf = req.loadingInfo;
            string abPath = req.abPath;
            string fullPath = GetFullPath(abPath);

            Action<AsyncOperation> onloaded = (opt) =>
            {
                AssetBundleCreateRequest cr = opt as AssetBundleCreateRequest;
                inf.ab = cr.assetBundle;
                loadingAsyncAbs.Remove(abPath);
                req.OnSelfLoaded();
            };

            try
            {
                if (createEncyptStream != null)
                {
                    stream = createEncyptStream(fullPath);
                    req.abReq = AssetBundle.LoadFromStreamAsync(stream);
                }
                else
                {
                    req.abReq = AssetBundle.LoadFromFileAsync(fullPath);
                }
                req.abReq.completed += onloaded;
                req.abReq.priority = AssetLoadRequest.Priority_Max - req.priority;
                inf.stream = stream;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                loadingAsyncAbs.Remove(abPath);
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                req.OnSelfLoaded();
            }
        }

        public bool IsLoadFromWeb { get; private set; } = false;
        private void DoAsyncLoadFromWeb(InternalAsyncAbLoadRequest asyncReq)
        {
            string abPath = asyncReq.abPath;
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(GetFullPath(abPath));
            var opt = uwr.SendWebRequest();
            opt.completed += (optReq) =>
            {
                loadingAsyncAbs.Remove(abPath);
                var inf = asyncReq.loadingInfo;
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    inf.ab = DownloadHandlerAssetBundle.GetContent(uwr);
                }
                asyncReq.OnSelfLoaded();
                uwr.Dispose();
            };
        }

        private string GetFullPath(string abPath)
        {
            //TODO,增加路径缓存
            if (IsLoadFromWeb)
            {
                //http://wwww.test.com/DataWeb/a.bundle
                return string.Format("{0}/{1}", rootPath, abPath);
            }
            else
            {
                return Path.Combine(rootPath, abPath);
            }
        }

        //释放一步
        //返回true表示释放完
        public bool UnloadUnusedStep()
        {
            tmps.Clear();
            foreach (var pair in loadedAbs)
            {
                var info = pair.Value;
                if (!HasBundleRef(info))
                {
                    tmps.Add(pair.Key);
                }
                if (usingAbs.Contains(pair.Key))
                {
                    continue;
                }
            }
            bool cleard = tmps.Count == 0;
            foreach (var key in tmps)
            {
                _UnloadAb(key);
            }
            return cleard;
        }

        //完全释放
        public void UnloadUnusedTotal()
        {
            int step = 0;
            while (!UnloadUnusedStep())
            {
                step++;
                if (step >= 100)
                {
                    Debug.LogError("too many unload unued step:" + step);
                    break;
                }
            }
        }

        private List<string> tmps = new List<string>();
        private Stopwatch stopwatch = new Stopwatch();
        //卸载无引用ab
        public bool UnloadUnused(int maxUnload = -1, float autoBreakTime = -1f)
        {
            tmps.Clear();
            int currFameCount = Time.frameCount;
            int shouldUnloadCount = 0;
            foreach (var pair in loadedAbs)
            {
                AbInfo info = pair.Value;
                if (currFameCount - info.usedFrame < 30) //30帧之内，不检查引用计数，不做清理
                {
                    continue;
                }
                if (!HasBundleRef(info))
                {
                    bool shouldCache = checkCache != null ? checkCache(pair.Key) : true;

                    if (shouldCache)
                    {
                        if (info.unusedFrame == 0)
                        {
                            info.unusedFrame = currFameCount;
                        }
                        if (currFameCount - info.unusedFrame < 30)   //无引用缓存至少30帧
                        {
                            continue;
                        }
                        if (lruCache.Contains(pair.Key))
                        {
                            continue;
                        }
                    }

                    tmps.Add(pair.Key);
                    //有最大卸载数，先收集
                    if (maxUnload > 0)
                    {
                        ++shouldUnloadCount;
                        if (shouldUnloadCount >= maxUnload * 2)
                        {
                            break;
                        }
                    }
                }
            }

            int count = 0;
            stopwatch.Reset();
            bool unloadAll = true;
            foreach (var tmp in tmps)
            {
                stopwatch.Start();
                if (!_UnloadAb(tmp))
                {
                    //没发生卸载
                    continue;
                }
                count++;
                stopwatch.Stop();
                if (maxUnload > 0 && count >= maxUnload)
                {
                    unloadAll = false;
                    break;
                }
                if (autoBreakTime > 0 && stopwatch.ElapsedMilliseconds > autoBreakTime)
                {
                    unloadAll = false;
                    break;
                }
            }
            tmps.Clear();
            return unloadAll;
        }

        public IEnumerator AutoClear()
        {
            while (true)
            {
                //一帧最多卸载5个，超过3毫秒就停止，等到下一帧继续卸载
                bool unloadAll = UnloadUnused(autoUnloadMaxCount, autoUnloadMaxMilliSecUse);
                if (unloadAll)
                {
                    //卸载完了，等5s后再触发
                    yield return new WaitForSeconds(autoUnloadFullUnloadInterval);
                }
                else
                {
                    //分帧了，下帧继续
                    yield return null;
                }
            }
        }

        public bool ContainAsset(string assetPath)
        {
            return !string.IsNullOrEmpty(bundleMap.GetBundle(assetPath));
        }

        public bool GetAbPath(string assetPath, out string abPath, out string assetName)
        {
            return bundleMap.GetAbPath(assetPath, out abPath, out assetName);
        }

        //编辑器接口
#if UNITY_EDITOR
        int hitCacheCount = 0;
        int loadCount = 0;
        private void TestIfHitCache(string abPath)
        {
            if (lruCache.Contains(abPath))
            {
                //Debug.Log("hit cache:" + abPath);
                hitCacheCount++;
            }
        }
        private void OnStartLoad(string abPath)
        {
            loadCount++;
        }

        public class AbEditorInfo
        {
            public UnityEngine.Object ab;
            public AssetRefList deps;
            public string abPath;
        }

        //加载的ab
        public List<AbEditorInfo> GetLoadedAbs()
        {
            List<AbEditorInfo> objs = new List<AbEditorInfo>();
            foreach (var pair in loadedAbs)
            {
                objs.Add(new AbEditorInfo()
                {
                    ab = pair.Value.ab,
                    deps = pair.Value.refList,
                    abPath = pair.Key,
                });
            }
            return objs;
        }

        //缓存内容
        public string[] GetLruInfo()
        {
            return lruCache.ToArray();
        }

        //缓存命中率
        public float cacheHitPercent { get { return hitCacheCount / (float)loadCount; } }
#endif
    }
}
