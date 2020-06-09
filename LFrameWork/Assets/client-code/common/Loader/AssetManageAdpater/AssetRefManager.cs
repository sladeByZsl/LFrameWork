using System;
using System.Collections.Generic;
using UnityEngine;

namespace LFrameWork.AssetManagement
{
    using Object = UnityEngine.Object;


    //简版引用管理
    [Serializable]
    public class AssetRef
    {
        public int refCount { get; private set; }

        public bool valid { get { return manager != null; } }

        public AssetRefManager manager { get; internal set; }

        public AssetRef(AssetRefManager manager_)
        {
            manager = manager_;
#if UNITY_EDITOR && DEBUG_REF
            ctorStack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
        }

#if UNITY_EDITOR && DEBUG_REF
        ~AssetRef()
        {
            if ( refCount != 0 /*|| valid*/ )
            {
                string msg = string.Format("AssetRef.~AssetRef ref = {0}, valid = {1} ", refCount, valid);
                msg += (" assetName = " + assetName);
                msg += (" ctorStack = " + ctorStack);
                Debug.LogError(msg);
            }
        }
#endif

        internal void Ref()
        {
            refCount++;
            if (refCount == 1 && linkRef != null)
            {
                linkRef.Ref();
            }
#if UNITY_EDITOR && DEBUG_REF
            refCalledStacks.Add(GetCallInfo());
#endif
        }

        internal void UnRef()
        {
            refCount--;
            if (refCount < 0)
            {
                Debug.LogWarning("AssetRef::UnRef refCount < 0");
                refCount = 0;
            }
            if (refCount == 0 && linkRef != null)
            {
                linkRef.UnRef();
            }
#if UNITY_EDITOR && DEBUG_REF
            unRefCalledStacks.Add(GetCallInfo());
#endif
        }

        public bool HasRef()
        {
            return valid && refCount > 0;
        }

        internal AssetRef linkRef { get; set; }

#if UNITY_EDITOR
        public Object asset;
        public int refId;
        public string assetName;
        public string ctorStack;
        public List<string> refCalledStacks = new List<string>();
        public List<string> unRefCalledStacks = new List<string>();
        public void OnCreateAtEditor(Object asset)
        {
            this.asset = asset;
            this.refId = manager.GetRefId(asset);
            this.assetName = asset.name;
            //TODO
            //增加模拟模式
            //再未打包时，可以获得虚拟ab的assetName，instanceId
        }
        public string GetCallInfo()
        {
            string msg = UnityEngine.StackTraceUtility.ExtractStackTrace();
            string[] lines = msg.Split('\n');
            string trimMsg = "";
            for (int i = 2; i < lines.Length; i++)
            {
                trimMsg += lines[i] + '\n';
            }
            return trimMsg;
        }
        //编辑器退出时，清空引用
        internal void ReleaseAtEditor()
        {
            refCount = 0;
            manager = null;
        }
#endif


        public override string ToString()
        {
#if UNITY_EDITOR
            if (asset == null)
            {
                return "assetRef error.";
            }
            return string.Format("{0},refCount({1})", assetName, refCount);
#else
            return string.Format("refCount:{0}",refCount);
#endif
        }


    }

    public class AssetRefList
    {
        private List<AssetRef> refList = new List<AssetRef>();
#if UNITY_EDITOR
        public AssetRef[] ToArray()
        {
            return refList.ToArray();
        }

        public AssetRef GetAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                return null;
            }
            else
            {
                return refList[index];
            }
        }
#endif

        public int Count => refList.Count;

        public void AddRef(AssetRef assetRef)
        {
            if (assetRef == null)
            {
                return;
            }
            if (!assetRef.valid)
            {
                return;
            }
            if (!refList.Contains(assetRef))
            {
                assetRef.Ref();
                refList.Add(assetRef);
            }
        }

        public void AddRange(List<AssetRef> others)
        {
            foreach (var assetRef in others)
            {
                AddRef(assetRef);
            }
        }

        public bool RemoveRef(AssetRef assetRef)
        {
            bool ret = refList.Remove(assetRef);
            if (ret && assetRef.valid)
            {
                assetRef.UnRef();
            }
            return ret;
        }

        public void ClearRef()
        {
            foreach (var assetRef in refList)
            {
                if (assetRef.valid)
                {
                    assetRef.UnRef();
                }
            }
            refList.Clear();
        }

        public bool Contains(AssetRef assetRef)
        {
            return refList.Contains(assetRef);
        }
    }

    public class AssetRefManager
    {
        public readonly string name;

        private Dictionary<int, AssetRef> assetRefs = new Dictionary<int, AssetRef>();

#if UNITY_EDITOR
        public Dictionary<int, AssetRef> AssetRefMap => assetRefs;
#endif

        public AssetRefManager(string name_)
        {
            name = name_;
        }

        internal void Clear()
        {
#if UNITY_EDITOR
            foreach (var pair in assetRefs)
            {
                pair.Value.ReleaseAtEditor();
            }
#endif
            assetRefs.Clear();
        }

        public AssetRef GetRef(Object asset)
        {
            if (asset == null)
            {
                return null;
            }
            return GetRef(GetRefId(asset));
        }

        public AssetRef GetRef(int key)
        {
            AssetRef assetRef;
            if (assetRefs.TryGetValue(key, out assetRef))
            {
                return assetRef;
            }
            return null;
        }

        //销毁一个资源的引用管理
        internal void DestroyRef(Object asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("AssetRefManager.DestroyRef asset is null");
                return;
            }
            var key = GetRefId(asset);
            var assetRef = GetRef(key);
            if (assetRef != null)
            {
                assetRef.manager = null;
                assetRefs.Remove(key);
            }
            //资源可能从来没有被引用过或者引用已经被释放了，这时是查找不到引用的
#if UNITY_EDITOR
            // else
            // {
            //     Debug.LogWarning(string.Format("AssetRefManager.DestroyRef GetRef faild " + asset.name), asset);
            // }
#endif
        }

        internal AssetRef GetOrCreateRef(Object asset)
        {
            if (asset == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("AssetRefManager.GetOrCreateRef asset is null");
#endif
                return null;
            }
            int key = GetRefId(asset);
            AssetRef assetRef;
            if (assetRefs.TryGetValue(key, out assetRef))
            {
                return assetRef;
            }
            else
            {
                assetRef = new AssetRef(this);
                assetRefs.Add(key, assetRef);
#if UNITY_EDITOR
                assetRef.OnCreateAtEditor(asset);
#endif
                return assetRef;
            }
        }

        public bool HasRef(Object asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("AssetRefManager.HasRef asset is null");
                return false;
            }

            AssetRef assetRef;
            if (!assetRefs.TryGetValue(GetRefId(asset), out assetRef))
            {
                return false;
            }
            return assetRef.HasRef();
        }

        public int GetRefId(Object asset)
        {
            return asset.GetInstanceID();
        }

        List<int> tmps = new List<int>();
        //清理失效的资源引用
        public void ClearInValidRef()
        {
            tmps.Clear();
            foreach (var pair in assetRefs)
            {
                if (!pair.Value.valid)
                {
                    pair.Value.manager = null;
                    tmps.Add(pair.Key);
                }
            }
            foreach (var key in tmps)
            {
                assetRefs.Remove(key);
            }
            tmps.Clear();
        }

#if UNITY_EDITOR
        public List<AssetRef> GetUnClearRefs()
        {
            List<AssetRef> refs = new List<AssetRef>();
            foreach (var pair in assetRefs)
            {
                if (!pair.Value.valid)
                {
                    continue;
                }
                refs.Add(pair.Value);
            }
            return refs;
        }

        public void PrintUnClearRefs()
        {
            var refs = GetUnClearRefs();
            foreach (var notClearRef in refs)
            {
                Debug.Log(notClearRef.ToString());
            }
        }
#endif
    }
}
