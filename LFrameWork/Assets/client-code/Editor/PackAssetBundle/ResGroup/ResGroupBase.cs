

using UnityEngine;


public class ResGroupBase
{
    protected readonly string _groupName;

    public string GroupName
    { get { return _groupName; } }


    protected ResGroupBase(string groupName)
    {
        _groupName = groupName;
    }

    // 打包前做的一些工作，比如lua二进制导出等
    public bool BeforeBuild(AssetBundleBuilder builder)
    {
        Debug.Log(string.Format("===={0} BeforeBuild Begin", this.GetType().ToString()));
        bool b = DoBeforeBuild(builder);
        Debug.Log(string.Format("===={0} BeforeBuild End", this.GetType().ToString()));
        return b;
    }
    protected virtual bool DoBeforeBuild(AssetBundleBuilder builder)
    {
        return true;
    }

    // 收集需要打包assetbundle的资源
    public bool CollectBundleAssets(AssetBundleBuilder builder)
    {
        Debug.Log(string.Format("===={0} CollectBundleAssets Begin", this.GetType().ToString()));
        bool b = DoCollectBundleAssets(builder);
        Debug.Log(string.Format("===={0} CollectBundleAssets End", this.GetType().ToString()));
        return b;
    }
    protected virtual bool DoCollectBundleAssets(AssetBundleBuilder builder)
    {
        return true;
    }

    // 打包后统计文件，删除文件等
    public bool AfterBuild(AssetBundleBuilder builder)
    {
        Debug.Log(string.Format("===={0} AfterBuild Begin", this.GetType().ToString()));
        bool b = DoAfterBuild(builder);
        Debug.Log(string.Format("===={0} AfterBuild End", this.GetType().ToString()));
        return b;
    }
    protected virtual bool DoAfterBuild(AssetBundleBuilder builder)
    {
        return true;
    }
}
