using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;

namespace LFrameWork.AssetManagement
{
    public class WebLoader
    {
        private class WebAssetInfo
        {
            public Object asset;
            public int unusedFrame;
        }

        private AssetRefManager refMgr = new AssetRefManager("WebLoader");
        private Dictionary<string, WebAssetInfo> loaded = new Dictionary<string, WebAssetInfo>();
        private LRUCache lruCache;
        private Dictionary<string, List<AssetLoadRequest>> loadingReqs = new Dictionary<string, List<AssetLoadRequest>>();

        public void Init(int cacheSize)
        {
            lruCache = new LRUCache(cacheSize);
            loaded.Clear();
        }

        public void Load(AssetLoadRequest req)
        {
            string url = req.url;

            if (loaded.TryGetValue(url, out var info))
            {
                //已经加载完了
                info.unusedFrame = 0;
                req.assetHandle = new AssetHandle(info.asset, refMgr.GetOrCreateRef(info.asset));
                req.Complete();
                lruCache.Put(url);
                return;
            }

            if (loadingReqs.TryGetValue(url, out var reqs))
            {
                reqs.Add(req);
            }
            else
            {
                reqs = new List<AssetLoadRequest>();
                reqs.Add(req);


                var webReq = req.GetData<UnityWebRequest>("__webRequest");
                if (webReq == null)
                {
                    webReq = UnityWebRequest.Get(url);
                }

                loadingReqs[webReq.url] = reqs;
                var opt = webReq.SendWebRequest();
                opt.completed += OnRequestCompelted;
            }
        }

        private void OnRequestCompelted(AsyncOperation opt)
        {
            var webOpt = opt as UnityWebRequestAsyncOperation;
            if (webOpt == null)
            {
                return;
            }
            var webReq = webOpt.webRequest;
            if (webReq == null)
            {
                return;
            }
            if (!loadingReqs.TryGetValue(webReq.url, out var reqs))
            {
                return;
            }
            loadingReqs.Remove(webReq.url);
            if (reqs.Count == 0)
            {
                return;
            }

            Object asset = null;
            byte[] rawDatas = null;
            if (webReq.isHttpError || webReq.isNetworkError)
            {
                Debug.LogError(webReq.error);
            }
            else
            {
                var req = reqs[0];
                if (HandleAsset(webReq, req.type, out asset))
                {
                    //unity内置格式
                }
                else
                {
                    //原始byte
                    rawDatas = webOpt.webRequest.downloadHandler.data;
                }
            }

            foreach (var req in reqs)
            {
                if (asset != null)
                {
                    OnLoadCompleted(asset, req);
                }
                else
                {
                    req.AttachData("__webBytes", rawDatas);
                }
                req.Complete();
            }
            webReq.Dispose();
        }

        private bool HandleAsset(UnityWebRequest webReq, Type type, out Object asset)
        {
            if (type == typeof(Texture2D))
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(webReq.downloadHandler.data, true);
                asset = tex;
                return true;
            }
            else if (type == typeof(TextAsset))
            {
                string content = webReq.downloadHandler.text;
                asset = new TextAsset(content);
                return true;
            }
            else
            {
                asset = null;
                return false;
            }
        }

        private void OnLoadCompleted(Object asset, AssetLoadRequest req)
        {
            loaded[req.path] = new WebAssetInfo()
            {
                asset = asset,
                unusedFrame = 0,
            };
            req.assetHandle = new AssetHandle(asset, refMgr.GetOrCreateRef(asset));
        }

        private void Unload(string url)
        {
            if (!loaded.TryGetValue(url, out var info))
            {
                return;
            }

            var asset = info.asset;
            refMgr.DestroyRef(asset);
            loaded.Remove(url);
            if (asset != null)
            {
                Object.Destroy(asset);
            }
        }

        private List<string> tmps = new List<string>();
        public void UnloadUnused(bool clearCache = false)
        {
            tmps.Clear();
            int currFrameCount = Time.frameCount;
            foreach (var pair in loaded)
            {
                var info = pair.Value;

                if (refMgr.HasRef(info.asset))
                {
                    continue;
                }

                if (info.unusedFrame == 0)
                {
                    info.unusedFrame = currFrameCount;
                }

                if (!clearCache && currFrameCount - info.unusedFrame < 30)
                {
                    continue;
                }
                if (!clearCache && lruCache.Contains(pair.Key))
                {
                    continue;
                }

                tmps.Add(pair.Key);
            }

            foreach (var key in tmps)
            {
                Unload(key);
            }
            tmps.Clear();
        }

        public IEnumerator AutoUnloadUnused()
        {
            while (true)
            {
                UnloadUnused();
                yield return new WaitForSeconds(30f);
            }
        }
    }
}