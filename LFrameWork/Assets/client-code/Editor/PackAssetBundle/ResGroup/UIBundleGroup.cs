using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class UIBundleGroup : ResGroupBase
{

    public UIBundleGroup() : base("UI")
    {

    }

    private static string GetPrefabBundleName(string path, string folder, string p_type, string suffix = "")
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        fileName = fileName.ToLower();
        filePath = string.Format("{0}_{1}{2}{3}", p_type, fileName, suffix, ResourcesSetting.BundleExtensions);

        return filePath;
    }

    protected override bool DoCollectBundleAssets(AssetBundleBuilder builder)
    {
        // UI依赖的贴图资源按目录先统一打包;
        List<string> texList = EditorCommon.GetTextures(ResourcesSetting.UICommonFolder);
        foreach (string tex in texList)
        {
            //string dep_bundleName = string.Format("{0}/{1}{2}", BundleType.UITexture.ToString(), tex, ResourcesSetting.BundleExtensions);
            string folderName = Path.GetFileName(Path.GetDirectoryName(tex));
            string dep_bundleName = string.Format("{0}{1}", folderName, ResourcesSetting.BundleExtensions);
            builder.AddAsset(GroupName, tex, "gui/common/" + dep_bundleName);
        }

        // 动态图标，加载后合并图集，比如物品图标;
        List<string> iconList = EditorCommon.GetTextures(ResourcesSetting.UIIconFolder);
        UIBuildAssetBundle(builder, iconList, ResourcesSetting.BundleUIIconFolder, BundleType.UIIcon.ToString(), "gui/icon/");

        // 动态图标，加载后不合并图集，比如小地图、半身像;
        List<string> textureList = EditorCommon.GetTextures(ResourcesSetting.UITextureFolder);
        UIBuildAssetBundle(builder, textureList, ResourcesSetting.BundleUITextureFolder, BundleType.UITexture.ToString(), "gui/texture/");

        //UI特效
        List<string> particleList = EditorCommon.GetPrefabs(ResourcesSetting.UIParticleFolder);
        UIBuildAssetBundle(builder, particleList, ResourcesSetting.BundleUIParticleFolder, BundleType.UIParticle.ToString(), "gui/particle/");

        List<string> spineList = EditorCommon.GetSpines(ResourcesSetting.UISpineFolder);
        UISpineBuildAssetBundle(builder, spineList, ResourcesSetting.BundleUISpineFolder, BundleType.UISpine.ToString(), "gui/spine/");

        List<string> prefabList = EditorCommon.GetPrefabs(ResourcesSetting.PrefabUIFolder);
        foreach (string prefab in prefabList)
        {
            //PackPrefabBundle(builder, prefab);

            string path = prefab;
            string prefab_bundleName = "gui/prefab/" + GetPrefabBundleName(path, ResourcesSetting.BundleUIPrefabFolder, BundleType.UIPrefab.ToString());
            builder.AddAsset(GroupName, path, prefab_bundleName);
        }

        return true;
    }


    private void UISpineBuildAssetBundle(AssetBundleBuilder builder, List<string> datalist, string p_folder_path, string p_type, string folder)
    {
        foreach (string data in datalist)
        {
            string dep_bundleName = GetPrefabBundleName(data, p_folder_path, p_type);
            builder.AddAsset(GroupName, data, folder + dep_bundleName);
        }
    }

    private void UIBuildAssetBundle(AssetBundleBuilder builder, List<string> datalist, string p_folder_path, string p_type, string folder)
    {
        foreach (string data in datalist)
        {
            string dep_bundleName = GetPrefabBundleName(data, p_folder_path, p_type);
            builder.AddAsset(GroupName, data, folder + dep_bundleName);
        }
    }
}
