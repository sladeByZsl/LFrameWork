#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections.Generic;
using UnityEngine;
using LFrameWork.AssetManagement;

namespace LFrameWork.AssetManagement
{
#if UNITY_EDITOR
    public class AssetRefHelperRegister
    {
        private static Dictionary<AssetRef, HashSet<GameObject>> assetRefDict = new Dictionary<AssetRef, HashSet<GameObject>>();

        public static GameObject[] FindGameObjects(AssetRef assetRef)
        {
            if (assetRefDict.TryGetValue(assetRef, out var set))
            {
                return set.ToArray();
            }
            return null;
        }

        public static void Capture()
        {
            assetRefDict.Clear();
            var helpers = GameObject.FindObjectsOfType(typeof(AssetRefHelper));
            foreach (var comp in helpers)
            {
                var helper = comp as AssetRefHelper;
                if (helper == null)
                {
                    continue;
                }
                if (helper.refList == null)
                {
                    continue;
                }
                foreach (var assetRef in helper.refList.ToArray())
                {
                    if (assetRefDict.TryGetValue(assetRef, out var set))
                    {
                        set.Add(helper.gameObject);
                    }
                    else
                    {
                        set = new HashSet<GameObject>();
                        set.Add(helper.gameObject);
                        assetRefDict.Add(assetRef, set);
                    }
                }
            }
        }
    }
#endif
}