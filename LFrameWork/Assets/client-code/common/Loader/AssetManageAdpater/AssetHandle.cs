using UnityEngine;

namespace LFrameWork.AssetManagement
{
    public struct AssetHandle
    {
        public static AssetHandle invalid = new AssetHandle(null, null);

        public UnityEngine.Object asset { get; private set; }
        internal AssetRef assetRef { get; }

        internal AssetHandle(UnityEngine.Object asset, AssetRef assetRef)
        {
            this.asset = asset;
            this.assetRef = assetRef;
        }

        public bool isValid { get { return asset != null && assetRef != null; } }

        public void RefAsset()
        {
            if (isValid && assetRef.valid)
            {
                assetRef.Ref();
            }
        }

        public void RleaseAssetRef()
        {
            if (isValid && assetRef.valid)
            {
                assetRef.UnRef();
            }
        }
    }
}