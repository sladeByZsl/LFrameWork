using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
# if UNITY_EDITOR
namespace LFrameWork.AssetManagement
{
    internal class EditorLoader
    {
        AssetRefManager editorAssetRefMagr = new AssetRefManager("EditorAssets");
        private Dictionary<string, Object> assetCaches = new Dictionary<string, Object>();
        private LRU_1 lru = new LRU_1(256);

        internal void Release()
        {
            assetCaches.Clear();
            editorAssetRefMagr.Clear();
        }

        internal bool ContainAsset(string assetPath)
        {
            return false;
        }

        internal bool TryGetLoaded(string assetPath, out AssetHandle assetHandle)
        {
            assetHandle = AssetHandle.invalid;
            if (!assetCaches.TryGetValue(assetPath, out var asset))
            {
                return false;
            }
            assetHandle = new AssetHandle(asset, editorAssetRefMagr.GetOrCreateRef(asset));
            return true;
        }

        //无类型，如果同名，返回第一个
        private string GetRealPathQuick(string assetPath)
        {
            try
            {
                string folder = Path.GetDirectoryName(assetPath);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                string[] files = Directory.GetFiles(folder);
                foreach (var file in files)
                {
                    if (Path.GetFileNameWithoutExtension(file) == assetName)
                        return file;
                }
                return assetPath;
            }
            catch (Exception e)
            {
                Debug.LogError("Path not valid." + assetPath);
                Debug.LogError(e.ToString());
                return assetPath;
            }

        }

        private string[] tmpStrArr = new string[1];
        //"Assets/Bundle/Test" -> "Assets/Bundle/Test.prefab
        private string GetRealPath(string assetPath, System.Type type)
        {
            string assetName = Path.GetFileName(assetPath);
            string searchName = assetName;
            if (type != null)
            {
                searchName += " t:" + Path.GetExtension(type.ToString()).TrimStart('.'); // UnityEngine.GameObject -> GameObject
            }
            tmpStrArr[0] = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(tmpStrArr[0]))
            {
                return assetPath;
            }
            var files = AssetDatabase.FindAssets(searchName, tmpStrArr);
            if (files.Length == 0)
            {
                return assetPath;
            }
            else if (files.Length == 1)
            {
                return AssetDatabase.GUIDToAssetPath(files[0]);
            }
            else
            {
                //相同文件夹下，多个文件名字有包含关系
                //Test.png,Test_1.png
                foreach (var guid in files)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Path.GetFileNameWithoutExtension(path) == assetName)
                    {
                        return path;
                    }
                }
            }
            return assetPath;
        }

        internal AssetHandle LoadAssetAtEditor(string assetPath, System.Type type)
        {
            if (assetCaches.TryGetValue(assetPath, out var cachedAsset))
            {
                lru.Put(assetPath);
                return new AssetHandle(cachedAsset, editorAssetRefMagr.GetOrCreateRef(cachedAsset));
            }

            string realPath = Path.HasExtension(assetPath) ? assetPath : GetRealPathQuick(assetPath);
            var asset = AssetDatabase.LoadAssetAtPath(realPath, type == null ? typeof(UnityEngine.Object) : type);
            if (asset == null)
            {
                Debug.LogWarning("load failed:" + realPath);
                return AssetHandle.invalid;
            }
            assetCaches[assetPath] = asset;
            lru.Put(assetPath);
            return new AssetHandle(asset, editorAssetRefMagr.GetOrCreateRef(asset));
        }

        private HashSet<string> tmps = new HashSet<string>();
        internal IEnumerator AutoClear()
        {
            while (true)
            {
                foreach (var pair in assetCaches)
                {
                    var key = pair.Key;
                    var asset = pair.Value;
                    if (editorAssetRefMagr.HasRef(asset))
                    {
                        continue;
                    }
                    if (lru.Contains(key))
                    {
                        continue;
                    }
                    tmps.Add(key);
                }
                foreach (var key in tmps)
                {
                    editorAssetRefMagr.DestroyRef(assetCaches[key]);
                    assetCaches.Remove(key);
                }
                tmps.Clear();
                yield return Resources.UnloadUnusedAssets();
                yield return new WaitForSeconds(60);
            }
        }
    }
}
#endif