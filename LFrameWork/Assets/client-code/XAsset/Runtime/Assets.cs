//
// Assets.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2020 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#define LOG_ENABLE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace libx
{
    public sealed class Assets : MonoBehaviour
    {
        public static readonly string ManifestAsset = "Assets/Manifest.asset";
        public static readonly string Extension = ".unity3d";

        /// <summary>
        /// true为assetbundle的模式加载,false为原始资源
        /// </summary>
        public static bool runtimeMode = true;
        public static Func<string, Type, Object> loadDelegate = null;
        private const string TAG = "[Assets]";

        [Conditional("LOG_ENABLE")]
        private static void Log(string s)
        {
            Debug.Log(string.Format("{0}{1}", TAG, s));
        }

        #region API

        /// <summary>
        /// 读取所有资源路径
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllAssetPaths()
        {
            var assets = new List<string>();
            assets.AddRange(_assetToBundles.Keys);
            return assets.ToArray();
        }

        public static string basePath { get; set; }

        public static string updatePath { get; set; } 

        public static void AddSearchPath(string path)
        { 
            _searchPaths.Add(path);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static ManifestRequest Initialize()
        {
            var instance = FindObjectOfType<Assets>();
            if (instance == null)
            {
                instance = new GameObject("Assets").AddComponent<Assets>();
                DontDestroyOnLoad(instance.gameObject);
            }

            //初始化basePath和updatePath
            //basePath在streamingAssetsPath目录
            //updatePath在persistentDataPath目录
            //资源加载时，先在updatePath查找，再去basePath中查找
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar;
            }

            if (string.IsNullOrEmpty(updatePath))
            {
                updatePath = Application.persistentDataPath + Path.DirectorySeparatorChar;
            }

            Clear();

            Log(string.Format("Initialize with: runtimeMode={0}\nbasePath：{1}\nupdatePath={2}",runtimeMode, basePath, updatePath));



            //加载manifeset.asset
            var request = new ManifestRequest {url = ManifestAsset};
            AddAssetRequest(request);
            return request;
        }

        public static AssetRequest LoadShader()
        {
            var shader_request = LoadAssetAsync("shaders", typeof(UnityEngine.Object));            AddAssetRequest(shader_request);
            return shader_request;
        }

        public static void Clear()
        { 
            _searchPaths.Clear();
            _activeVariants.Clear();
            _assetToBundles.Clear();
            _bundleToDependencies.Clear();
        }

        private static SceneAssetRequest _runningScene;
        
        public static SceneAssetRequest LoadSceneAsync(string path, bool additive)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            path = GetExistPath(path);
            var asset = new SceneAssetAsyncRequest(path, additive);
            if (! additive)
            {
                if (_runningScene != null)
                {
                    _runningScene.Release();;
                    _runningScene = null;
                }
                _runningScene = asset;
            }
            asset.Load();
            asset.Retain();
            _scenes.Add(asset);
            Log(string.Format("LoadScene:{0}", path));
            return asset;
        }

        public static void UnloadScene(SceneAssetRequest scene)
        {
            scene.Release();
        }

        public static AssetRequest LoadAssetAsync(string path, Type type)
        {
            return LoadAsset(path, type, true);
        }

        public static AssetRequest LoadAsset(string path, Type type)
        {
            return LoadAsset(path, type, false);
        }

        public static void UnloadAsset(AssetRequest asset)
        {
            asset.Release();
        }

        #endregion

        #region Private

        internal static void OnLoadManifest(Manifest manifest)
        {
            _activeVariants.AddRange(manifest.activeVariants); 
            
            var assets = manifest.assets;
            var dirs = manifest.dirs;
            var bundles = manifest.bundles;

            foreach (var item in bundles)
                _bundleToDependencies[item.name] = Array.ConvertAll(item.deps, id => bundles[id].name);

            foreach (var item in assets)
            {
                var path = string.Format("{0}/{1}", dirs[item.dir], item.name);
                if (item.bundle >= 0 && item.bundle < bundles.Length)
                {
                    _assetToBundles[path] = bundles[item.bundle].name;
                }
                else
                {
                    Debug.LogError(string.Format("{0} bundle {1} not exist.", path, item.bundle));
                }
            }
        }

        /// <summary>
        /// 全部的AssetRequest
        /// </summary>
        private static Dictionary<string, AssetRequest> _assets = new Dictionary<string, AssetRequest>();

        /// <summary>
        /// 正在加载的AssetRequest
        /// </summary>
        private static List<AssetRequest> _loadingAssets = new List<AssetRequest>();

        private static List<SceneAssetRequest> _scenes = new List<SceneAssetRequest>();

        private static List<AssetRequest> _unusedAssets = new List<AssetRequest>();

        private void Update()
        {
            UpdateAssets();
            UpdateBundles();
        }

        private static void UpdateAssets()
        {
            for (var i = 0; i < _loadingAssets.Count; ++i)
            {
                var request = _loadingAssets[i];
                if (request.NotDone())
                    continue;
                _loadingAssets.RemoveAt(i);
                --i;
            }

            foreach (var item in _assets)
            {
                if (item.Value.isDone && item.Value.IsUnused())
                {
                    _unusedAssets.Add(item.Value);
                }
            }

            if (_unusedAssets.Count > 0)
            {
                for (var i = 0; i < _unusedAssets.Count; ++i)
                {
                    var request = _unusedAssets[i]; 
                    Log(string.Format("UnloadAsset:{0}", request.url));
                    _assets.Remove(request.url);
                    request.Unload(); 
                } 
                _unusedAssets.Clear();
            }

            for (var i = 0; i < _scenes.Count; ++i)
            {
                var request = _scenes[i];
                if (request.NotDone() || !request.IsUnused())
                    continue;
                _scenes.RemoveAt(i);
                Log(string.Format("UnloadScene:{0}", request.url));
                request.Unload(); 
                --i;
            }
        }

        private static void AddAssetRequest(AssetRequest request)
        {
            _assets.Add(request.url, request);
            _loadingAssets.Add(request);
            request.Load();
        }

        private static AssetRequest LoadAsset(string path, Type type, bool async)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            path = GetExistPath(path);

            //如果已经加载过，或者正在加载，引用+1
            AssetRequest request;
            if (_assets.TryGetValue(path, out request))
            {
                request.Retain();
                _loadingAssets.Add(request);
                return request;
            }

            string assetBundleName;
            if (GetAssetBundleName(path, out assetBundleName))
            {
                request = async
                    ? new BundleAssetAsyncRequest(assetBundleName)
                    : new BundleAssetRequest(assetBundleName);
            }
            else
            {
                if (path.StartsWith("http://", StringComparison.Ordinal) ||
                    path.StartsWith("https://", StringComparison.Ordinal) ||
                    path.StartsWith("file://", StringComparison.Ordinal) ||
                    path.StartsWith("ftp://", StringComparison.Ordinal) ||
                    path.StartsWith("jar:file://", StringComparison.Ordinal))
                    request = new WebAssetRequest();
                else
                    request = new AssetRequest();
            }

            request.url = path;
            request.assetType = type;
            AddAssetRequest(request);
            request.Retain();
            Log(string.Format("LoadAsset:{0}", path));
            return request;
        }

        #endregion

        #region Paths

        private static List<string> _searchPaths = new List<string>();

        private static string GetExistPath(string path)
        {
#if UNITY_EDITOR
            if (!runtimeMode)
            {
                if (File.Exists(path))
                    return path;

                foreach (var item in _searchPaths)
                {
                    var existPath = string.Format("{0}/{1}", item, path);
                    if (File.Exists(existPath))
                        return existPath;
                }

                Debug.LogError("找不到资源路径" + path);
                return path;
            }
#endif
            if (_assetToBundles.ContainsKey(path))
                return path;

            foreach (var item in _searchPaths)
            {
                var existPath = string.Format("{0}/{1}", item, path);
                if (_assetToBundles.ContainsKey(existPath))
                    return existPath;
            }

            Debug.LogError("资源没有收集打包" + path);
            return path;
        }

        #endregion

        #region Bundles

        private static readonly int MAX_BUNDLES_PERFRAME = 0;

        /// <summary>
        /// 当前系统中存在的所有BundleRequest
        /// </summary>
        private static Dictionary<string, BundleRequest> _bundles = new Dictionary<string, BundleRequest>();

        /// <summary>
        /// 正在加载的bundle
        /// </summary>
        private static List<BundleRequest> _loadingBundles = new List<BundleRequest>();

        private static List<BundleRequest> _unusedBundles = new List<BundleRequest>();

        /// <summary>
        /// 因为分帧加载而需要等待加载的bundle
        /// </summary>
        private static List<BundleRequest> _toloadBundles = new List<BundleRequest>();

        private static List<string> _activeVariants = new List<string>();

        private static Dictionary<string, string> _assetToBundles = new Dictionary<string, string>();

        private static Dictionary<string, string[]> _bundleToDependencies = new Dictionary<string, string[]>();

        internal static bool GetAssetBundleName(string path, out string assetBundleName)
        {
            return _assetToBundles.TryGetValue(path, out assetBundleName);
        }

        /// <summary>
        /// 获取bundle的依赖关系
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private static string[] GetAllDependencies(string bundle)
        {
            string[] deps;
            if (_bundleToDependencies.TryGetValue(bundle, out deps))
                return deps;

            return new string[0];
        }

        internal static BundleRequest LoadBundle(string assetBundleName)
        {
            return LoadBundle(assetBundleName, false);
        }

        internal static BundleRequest LoadBundleAsync(string assetBundleName)
        {
            return LoadBundle(assetBundleName, true);
        }

        internal static void UnloadBundle(BundleRequest bundle)
        {
            bundle.Release();
        }

        private static void UnloadDependencies(BundleRequest bundle)
        {
            for (var i = 0; i < bundle.dependencies.Count; i++)
            {
                var item = bundle.dependencies[i];
                item.Release();
            }

            bundle.dependencies.Clear();
        }

        /// <summary>
        /// 加载依赖资源
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="asyncRequest"></param>
        private static void LoadDependencies(BundleRequest bundle, string assetBundleName, bool asyncRequest)
        {
            //1.获取所有依赖资源
            var dependencies = GetAllDependencies(assetBundleName);
            if (dependencies.Length <= 0)
                return;
            for (var i = 0; i < dependencies.Length; i++)
            {
                var item = dependencies[i];
                bundle.dependencies.Add(LoadBundle(item, asyncRequest));
            }
        }

        internal static BundleRequest LoadBundle(string assetBundleName, bool asyncMode)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                Debug.LogError("assetBundleName == null");
                return null;
            }

            assetBundleName = RemapVariantName(assetBundleName);
            var url = GetDataPath(assetBundleName) + assetBundleName;

            BundleRequest bundle;

            if (_bundles.TryGetValue(url, out bundle))
            {
                bundle.Retain();
                _loadingBundles.Add(bundle);
                return bundle;
            }

            if (url.StartsWith("http://", StringComparison.Ordinal) ||
                url.StartsWith("https://", StringComparison.Ordinal) ||
                url.StartsWith("file://", StringComparison.Ordinal) ||
                url.StartsWith("ftp://", StringComparison.Ordinal))
                bundle = new WebBundleRequest();
            else
                bundle = asyncMode ? new BundleAsyncRequest() : new BundleRequest();

            bundle.url = url;
            _bundles.Add(url, bundle);

            if (MAX_BUNDLES_PERFRAME > 0 && (bundle is BundleAsyncRequest || bundle is WebBundleRequest))
            {
                _toloadBundles.Add(bundle);
            }
            else
            {
                bundle.Load();
                _loadingBundles.Add(bundle);
                Log("LoadBundle: " + url);
            }

            LoadDependencies(bundle, assetBundleName, asyncMode);

            bundle.Retain();
            return bundle;
        }

        /// <summary>
        /// 获取资源所在目录，先查找updatePath，如果不存在，就查找basePath
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private static string GetDataPath(string bundleName)
        {
            if (string.IsNullOrEmpty(updatePath))
                return basePath;

            if (File.Exists(updatePath + bundleName))
                return updatePath;

            return basePath;
        }

        private static void UpdateBundles()
        {
            var max = MAX_BUNDLES_PERFRAME;
            if (_toloadBundles.Count > 0 && max > 0 && _loadingBundles.Count < max)
            {
                //分帧加载，获取当前帧可以加载的最大数量的资源
                for (var i = 0; i < Math.Min(max - _loadingBundles.Count, _toloadBundles.Count); ++i)
                {
                    var item = _toloadBundles[i];
                    if (item.loadState == LoadState.Init)
                    {
                        item.Load();
                        _loadingBundles.Add(item);
                        _toloadBundles.RemoveAt(i);
                        --i;
                    }
                }
            }

            for (var i = 0; i < _loadingBundles.Count; i++)
            {
                var item = _loadingBundles[i];
                if (item.NotDone())
                    continue;
                _loadingBundles.RemoveAt(i);
                --i;
            }

            foreach (var item in _bundles)
            {
                if (item.Value.isDone && item.Value.IsUnused())
                {
                    _unusedBundles.Add(item.Value);
                }
            } 
            
            if (_unusedBundles.Count <= 0) return;
            {
                for (var i = 0; i < _unusedBundles.Count; i++)
                {
                    var item = _unusedBundles[i];
                    if (item.isDone)
                    {
                        UnloadDependencies(item);
                        item.Unload();
                        _bundles.Remove(item.url);
                        Log("UnloadBundle: " + item.url); 
                    }  
                }
                _unusedBundles.Clear();
            }
        }

        private static string RemapVariantName(string assetBundleName)
        {
            var bundlesWithVariant = _activeVariants;
            // Get base bundle path
            var baseName = assetBundleName.Split('.')[0];

            var bestFit = int.MaxValue;
            var bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (var i = 0; i < bundlesWithVariant.Count; i++)
            {
                var curSplit = bundlesWithVariant[i].Split('.');
                var curBaseName = curSplit[0];
                var curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                var found = bundlesWithVariant.IndexOf(curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found >= bestFit)
                    continue;
                bestFit = found;
                bestFitIndex = i;
            }

            if (bestFit == int.MaxValue - 1)
                Debug.LogWarning(
                    "Ambiguous asset bundle variant chosen because there was no matching active variant: " +
                    bundlesWithVariant[bestFitIndex]);

            return bestFitIndex != -1 ? bundlesWithVariant[bestFitIndex] : assetBundleName;
        }

        #endregion
    }
}