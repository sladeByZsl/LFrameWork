using System.IO;
using System;
using System.Collections.Generic;
using LFrameWork.Common.Utility;
using LFrameWork.AssetManagement;
using UnityEngine;
using System.Collections;

namespace LFrameWork.Common.Resource
{
    public class AssetManageAdpater : MonoBehaviourSingle<AssetManageAdpater>
    {
        public static readonly string STR_ASSET_BUNDLE = "AssetBundle";

        protected override void OnDestroy()
        {
            Release();
            AssetManager.Release();
        }

        protected override void OnInit()
        {
            AssetManager.Init();
            var assetMgr = AssetManager.GetInstance();
            assetMgr.isEditorMode = ResourcesSetting.LoaderType != ELoaderType.LoaderType_AssetBundle;
            assetMgr.maxLoadCountPerFrame = 5;
            AbCacheConf conf = new AbCacheConf()
            {
                cacheSize = 25,
                checkCacheMethod = (abPath) =>
                {
                    //TODO
                    //场景资源不缓存
                    if (abPath.StartsWith("scene/"))
                    {
                        return false;
                    }
                    return true;
                }
            };

            //Action<BundleType> register = (bundleType) => { assetMgr.AddBundle(GetAbRoot(bundleType), AppConst.BundleEncyptMode,conf); };

            if (ResourcesSetting.LoaderType == ELoaderType.LoaderType_AssetBundle)
            {
                assetMgr.AddBundle(Path.Combine(ResourcesPath.localBundlePath, STR_ASSET_BUNDLE), conf);
            }
        }

        public void Release()
        {
        }

        private HashSet<string> loadingUIs = new HashSet<string>();
        private PriorityQueue<AssetLoadRequest> loadingQueue = new PriorityQueue<AssetLoadRequest>();
        private Coroutine loadUIProcess = null;
        private bool isLoadingScene = false;
        private IEnumerator LoadUIProcess()
        {
            while (true)
            {
                if (!loadingQueue.IsEmpty())
                {
                    var req = loadingQueue.Dequeue();
                    AssetManager.GetInstance().StartLoad(req);
                    yield return req;
                    var cb = req.GetData("cb") as Action<string, GameObject>;
                    var name = req.GetData("name") as string;
                    if (!loadingUIs.Contains(name))
                    {
                        yield break;
                    }
                    var goAsset = req.asset as GameObject;
                    if (goAsset == null)
                    {
                        Debug.LogError("load panel failed." + req.path);
                        loadingUIs.Remove(name);
                        yield break;
                    }
                    if (ResourcesSetting.LoaderType == ELoaderType.LoaderType_AssetBundle)
                    {
                        goAsset.SetActive(false); //让资源初始是非激活状态，ui资源实例化成本过高
                    }
                    //实例化后往往需要进行mesh重构，刷新界面等耗时操作
                    //初始非激活，可以单纯进行序列化，等一帧后再进行ui操作
                    yield return null;
                    var go = AssetManager.Instantiate(goAsset); //纯实例化
                    if (ResourcesSetting.LoaderType == ELoaderType.LoaderType_AssetBundle && !isLoadingScene)
                    {
                        yield return null; //ab模式下，可以等1帧实例化的消耗，Awake的消耗下帧触发
                    }
                    //go.SetActive(true); //awake
                    go.name = name;
                    loadingUIs.Remove(name);
                    if (cb != null)
                    {
                        cb(name, go);
                    }
                    AssetLoadRequest.Free(req);
                }
                yield return null;
            }
        }

        public void LoadPanel(string name, Action<string, GameObject> cb, int priority)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.PrefabUIFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }

            if (priority <= AssetLoadRequest.Priority_Sync)
            {
                var assetHandle = AssetManager.GetInstance().LoadAsset<GameObject>(reqPath);
                var goAsset = assetHandle.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load panel failed." + reqPath);
                    return;
                }
                var go = AssetManager.Instantiate(goAsset);
                go.name = name;
                if (cb != null)
                {
                    cb(name, go);
                }
                return;
            }

            if (loadingUIs.Contains(name))
            {
                return;
            }

            var req = AssetLoadRequest.Get(false);
            req.path = reqPath;
            req.AttachData("cb", cb);
            req.AttachData("name", name);

            req.priority = priority;
            loadingUIs.Add(name);
            loadingQueue.Enqueue(priority, req);

            if (loadUIProcess == null)
            {
                loadUIProcess = StartCoroutine(LoadUIProcess());
            }
        }

        public void LoadUIParticle(string name, GameObject loadGo, Action<GameObject> cb)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.UIParticleFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }
            AssetManager.GetInstance().LoadAssetAsync<GameObject>(reqPath, loadGo, (reqRet) =>
            {
                var goAsset = reqRet.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load ui particle failed." + name);
                    if (cb != null)
                    {
                        cb(null);
                    }
                    return;
                }
                if (cb != null)
                {
                    cb(goAsset);
                }
            });
        }

      
        public void LoadUITexture(string name, UnityEngine.UI.RawImage img, Action<Texture2D> cb)
        {
            var reqPath = GetTextureABPath(name);
            if (string.IsNullOrEmpty(reqPath))
            {
                Debug.LogError("load texture failed." + name);
                if (cb != null)
                {
                    cb(Texture2D.whiteTexture);
                }
                return;
            }
            AssetManager.GetInstance().LoadAssetAsync<Texture2D>(reqPath, img.gameObject, (reqRet) =>
            {
                var asset = reqRet.asset as Texture2D;
                if (asset == null)
                {
                    Debug.LogError("load texture failed." + name);
                    return;
                }
                if (img.texture != null && img.texture != asset)
                {
                    //删除之前资源的引用
                    AssetManager.GetInstance().RemoveAssetRef(img.gameObject, img.texture);
                }
                if (cb != null)
                {
                    cb(asset);
                }
            }, AssetLoadRequest.Priority_Fast);
        }

        public void LoadTexture(string name, GameObject loadGo, Action<Texture2D> cb)
        {
            var reqPath = GetTextureABPath(name);
            AssetManager.GetInstance().LoadAssetAsync<Texture2D>(reqPath, gameObject, (reqRet) =>
            {
                var asset = reqRet.asset as Texture2D;
                if (asset == null)
                {
                    Debug.LogError("load texture failed." + name);
                    return;
                }
                if (cb != null)
                {
                    cb(asset);
                }
            }, AssetLoadRequest.Priority_Fast);
        }

        public void LoadUISprite(string atlasName, string spriteName, UnityEngine.UI.Image img, Action<Sprite> cb)
        {
            var reqPath = string.Format("{0}/{1}/{2}", ResourcesSetting.UICommonFolder, atlasName, spriteName);
            if (ResourcesSetting.LoaderType != ELoaderType.LoaderType_AssetBundle)
            {
                if (AssetManager.GetInstance().isEditorMode)
                {
                    reqPath += ResourcesSetting.GetExtensions(Path.GetDirectoryName(reqPath), spriteName);
                }
            }
            AssetManager.GetInstance().LoadAssetAsync<Sprite>(reqPath, img.gameObject, (reqRet) =>
            {
                var asset = reqRet.asset as Sprite;
                if (asset == null)
                {
                    Debug.LogError("load sprite failed." + atlasName + "," + spriteName);
                    return;
                }
                if (img.sprite != null && img.sprite != asset)
                {
                    //删除之前资源的引用
                    AssetManager.GetInstance().RemoveAssetRef(img.gameObject, img.sprite);
                }
                if (cb != null)
                {
                    cb(asset);
                }
            }, AssetLoadRequest.Priority_Fast - 10);
        }

        public void LoadSprite(string atlasName, string spriteName, UnityEngine.SpriteRenderer spr, Action<Sprite> cb)
        {
            var reqPath = string.Format("{0}/{1}/{2}", ResourcesSetting.UICommonFolder, atlasName, spriteName);
            AssetManager.GetInstance().LoadAssetAsync<Sprite>(reqPath, spr.gameObject, (reqRet) =>
            {
                var asset = reqRet.asset as Sprite;
                if (asset == null)
                {
                    Debug.LogError("load sprite failed." + atlasName + "," + spriteName);
                }
                if (spr.sprite != null && spr.sprite != asset)
                {
                    AssetManager.GetInstance().RemoveAssetRef(spr.gameObject, spr.sprite);
                }
                if (cb != null)
                {
                    cb(asset);
                }
            }, AssetLoadRequest.Priority_Fast);
        }

        //运行时图集
        public void LoadIcon(string name, Action<Texture> cb)
        {
            string reqPath = string.Format("{0}/{1}", ResourcesSetting.UIIconFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ResourcesSetting.GetExtensions(Path.GetDirectoryName(reqPath), name);
            }
            //不需要管理引用，会渲染到一张新rt上
            AssetManager.GetInstance().LoadAssetAsync<Texture2D>(reqPath, (reqRet) =>
            {
                var asset = reqRet.asset as Texture2D;
                if (asset == null)
                {
                    Debug.LogError("load icon failed." + reqPath);
                }
                cb(asset);
            }, AssetLoadRequest.Priority_Fast - 1);
        }

        public void LoadParticle(string name, Action<GameObject> cb)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.PrefabFxFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }
            AssetManager.GetInstance().LoadAssetAsync<GameObject>(reqPath, (reqRet) =>
            {
                var goAsset = reqRet.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load particle failed." + name);
                    return;
                }
                var go = AssetManager.Instantiate(goAsset);
                go.name = name;
                if (cb != null)
                {
                    cb(go);
                }
            });
        }

        public void LoadSpine(string name, Action<GameObject> cb)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.PrefabSpineFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }
            AssetManager.GetInstance().LoadAssetAsync<GameObject>(reqPath, (reqRet) =>
            {
                var goAsset = reqRet.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load spine failed." + name);
                    return;
                }
                var go = AssetManager.Instantiate(goAsset);
                go.name = name;
                if (cb != null)
                {
                    cb(go);
                }
            });
        }

        public void LoadObject(string name, Action<GameObject> cb)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.PrefabObjectFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }
            AssetManager.GetInstance().LoadAssetAsync<GameObject>(reqPath, (reqRet) =>
            {
                var goAsset = reqRet.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load object failed." + name);
                    return;
                }
                var go = AssetManager.Instantiate(goAsset);
                go.name = name;
                if (cb != null)
                {
                    cb(go);
                }
            });
        }

        public void LoadCinema(string name, Action<GameObject> cb)
        {
            var reqPath = string.Format("{0}/{1}", ResourcesSetting.PrefabCinemaFolder, name);
            if (AssetManager.GetInstance().isEditorMode)
            {
                reqPath += ".prefab";
            }
            AssetManager.GetInstance().LoadAssetAsync<GameObject>(reqPath, (reqRet) =>
            {
                var goAsset = reqRet.asset as GameObject;
                if (goAsset == null)
                {
                    Debug.LogError("load cinema failed." + name);
                    return;
                }
                if (cb != null)
                {
                    cb(goAsset);
                }
            });
        }

        //private bool firstEnter = true;
        public IEnumerator LoadScene(string sceneName)
        {
            //if (firstEnter)
            {
                //firstEnter = false;
                //Debug.LogError("=====LoadScene  wait for 1 sceond " + sceneName);
                //yield return new WaitForSeconds(1f);
            }
            isLoadingScene = true;
            string scenePath = string.Format("{0}{1}", ResourcesSetting.ScenePath, sceneName);
            yield return AssetManager.GetInstance().LoadScene(scenePath);
            isLoadingScene = false;
        }

        /*public AssetBundle LoadLua(string path)
        {
            string abFullPath = string.Format("{0}/{1}{2}", Path.Combine(ResourcesPath.localBundlePath, STR_ASSET_BUNDLE)
                , path.ToLower()
                , ResourcesSetting.BundleExtensions);
            if (!Path.HasExtension(abFullPath))
            {
                abFullPath += ResourcesSetting.BundleExtensions;
            }
            var handle = AssetManager.GetInstance().PreloadAssetBundle(abFullPath, true);
            if (!handle.isValid)
            {
                Debug.LogError("load lua failed." + path);
            }
            return handle.asset as AssetBundle;
        }*/

        private static bool AbManagerContains(string path)
        {
            return AssetManagement.AssetManager.GetInstance().Contains(path);
        }

        internal static string GetTextureABPath(string p_name)
        {
            if (AssetManager.GetInstance().isEditorMode)
            {
                string t_folder = ResourcesSetting.GetTextureFolder(p_name);
                string extensions = ResourcesSetting.GetExtensions(t_folder, p_name);
                return string.Format("{0}/{1}{2}", t_folder, p_name, extensions);
            }

            string filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder_HeroIcon, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder_Special, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder_NoneAlpha, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder_Alpha, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder_Scene, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            filePath = string.Format("{0}/{1}", ResourcesSetting.UITextureFolder, p_name);
            if (AbManagerContains(filePath))
            {
                return filePath;
            }
            Debug.LogError("can not find any." + p_name);
            return "";
        }
    }
}
