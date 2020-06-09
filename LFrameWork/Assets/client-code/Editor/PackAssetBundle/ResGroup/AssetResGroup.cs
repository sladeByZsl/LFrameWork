using System.Collections.Generic;
using System.IO;

public class AssetResGroup : ResGroupBase
{
    private List<string> _assetList;
    private string _folder;
    private string _bundleType;


    public AssetResGroup(string groupName, string folder, BundleType bundleType, params string[] searchPatterns) : base(groupName)
    {
        _folder = folder;
        _bundleType = bundleType.ToString();
        _assetList = EditorCommon.GetAllFiles(folder, searchPatterns);
    }

    private string GetPrefabBundleName(string path,string suffix="")
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        fileName = fileName.ToLower();
        filePath = string.Format("{0}_{1}{2}{3}", _bundleType, fileName, suffix, ResourcesSetting.BundleExtensions);

        return filePath;
    }
    protected override bool DoCollectBundleAssets(AssetBundleBuilder builder)
    {
        foreach (string path in _assetList)
        {
            string prefab_bundleName = GetPrefabBundleName(path);
            //Debug.LogError("path:" + path+",prefab:"+ prefab_bundleName+",GroupName:"+GroupName+",combine:"+ Path.Combine(GroupName, prefab_bundleName));
            builder.AddAsset(GroupName, path, Path.Combine(GroupName, prefab_bundleName));
        }

        return true;
    }
}
