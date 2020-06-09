using UnityEngine;

namespace LFrameWork.AssetManagement
{
    public class AssetRefHelper : MonoBehaviour
    {
        internal AssetRefList refList;

        public void RefAsset(AssetRef assetRef)
        {
            if (refList == null)
            {
                refList = new AssetRefList();
            }
            refList.AddRef(assetRef);
        }

        public void UnRef(AssetRef assetRef)
        {
            if (assetRef == null)
            {
                return;
            }
            if (refList == null)
            {
                return;
            }
            refList.RemoveRef(assetRef);
        }

        void OnDestroy()
        {
            if (refList != null)
            {
                refList.ClearRef();
            }
        }
    }
}
