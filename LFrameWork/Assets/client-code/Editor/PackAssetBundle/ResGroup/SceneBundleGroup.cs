using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;


public class SceneBundleGroup : ResGroupBase
{
    public SceneBundleGroup() : base("scene")
    {

    }


    protected override bool DoCollectBundleAssets(AssetBundleBuilder builder)
    {
        string[] scenePaths = Directory.GetFiles(ResourcesSetting.ScenePath, "*.unity");

        if (scenePaths == null || scenePaths.Length < 1)
        {
            return false;
        }

        for (int i = 0; i < scenePaths.Length; i++)
        {
            string scenePath = scenePaths[i];
            PackSceneBundle(builder, scenePath);
        }
        return true;
    }

    public static string GetPrefabBundleName(string path, string folder, string p_type, string suffix = "")
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        fileName = fileName.ToLower();
        filePath = string.Format("{0}_{1}{2}{3}", p_type, fileName, suffix, ResourcesSetting.BundleExtensions);

        return filePath;
    }


    private void PackSceneBundle(AssetBundleBuilder builder, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        // 先标记主资源
        string prefab_bundleName = GetPrefabBundleName(path, ResourcesSetting.BundleSceneFolder, BundleType.Scene.ToString(), "_main");
        string sceneBundleName = "scene/" + prefab_bundleName;
        builder.AddAsset(GroupName, path, sceneBundleName, false);


        string dep_bundleName = GetPrefabBundleName(path, ResourcesSetting.BundleSceneFolder, BundleType.Scene.ToString(), "_dep");
        string[] deps = AssetDatabase.GetDependencies(path, true);
        foreach (string dep in deps)
        {
            if (dep == path)
            {
                continue;
            }

            builder.AddDepAsset(GroupName, dep, "scene/" + dep_bundleName, sceneBundleName);
        }
    }
}
