using System.IO;

namespace LFrameWork.AssetManagement
{
    //检查某个ab是否需要缓存
    public delegate bool CheckCache(string abPath);
    //Ab读取Stream创建者
    public delegate Stream BundleStreamCreator(string path);

    public struct AbCacheConf
    {
        public int cacheSize;
        public CheckCache checkCacheMethod;
    }
}
