using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LFrameWork.AssetManagement;
public class AssetBundleBuilder
{
    public static string ASSET_BUNDLE => AssetConst.AssetBundle;
    public static string MATERIAL_FEATURE_TEMPLETE_PATH = "Assets/Temp/Shaders/FeatureTemplete";

    public static string CSExt => ".cs";
    public static string DllExt => ".dll";
    public static string MaterialExt => ".mat";
    public static string BundleExt => AssetConst.BundleExtensions;
    public static string BundleShareExt => "_share" + BundleExt;
    public static string ShaderBundleName => AssetConst.ShaderBundleName;
    public static string FontBundleName => "Font/Font" + BundleExt;

    public static bool GenerateShaderFeatureTemplete { get; set; } = true;

    private static string ToBundleName(string path)
    {
        path = path.ToLower();
        string ext = Path.GetExtension(path);
        return path.Replace(ext, BundleExt);
    }

    private static string ToBundleShareName(string path)
    {
        path = path.ToLower();
        string ext = Path.GetExtension(path);
        return path.Replace(ext, BundleShareExt);
    }


    private static bool IsForceInShader(string path)
    {
        path = path.ToLower();

        if (path.EndsWith(".shader")
            || path.EndsWith(".cginc")
            || path.EndsWith(".shadervariants")
            || path.Contains("multi_compile_templete"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool IsForceInFont(string path)
    {
        path = path.ToLower();

        if (path.EndsWith(".ttf"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private static bool IsMaterial(string assetPath)
    {
        assetPath = assetPath.ToLower();
        return assetPath.EndsWith(MaterialExt);
    }

    private static bool IsUnpack(string path)
    {
        if (path.EndsWith(CSExt) || path.EndsWith(DllExt))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public class BundleGroup
    {
        public string groupName;


        // asset依赖谁
        public Dictionary<string, string[]> assetDeps = new Dictionary<string, string[]>();
        // 都被谁依赖
        public Dictionary<string, List<string>> depBy = new Dictionary<string, List<string>>();
        // <asset, bundleName>
        public Dictionary<string, string> bundleNameMap = new Dictionary<string, string>();

        public static bool packSharedDepGlobal = true;
        //全局依赖 asset,第一个bundleName
        public static Dictionary<string, string> gDepMap = new Dictionary<string, string>();
        //全局确定的共享依赖
        public static HashSet<string> gSharedDeps = new HashSet<string>();
        //全局已经标记BundleName的集合
        public static HashSet<string> gBundleSets = new HashSet<string>();
        //当前group中所有的依赖项
        public HashSet<string> groupDeps = new HashSet<string>();
        //直接依赖的set
        public static HashSet<string> gDirectDepSet = new HashSet<string>();

        public void AddAsset(string assetPath, string bundleName, bool packDeps = true)
        {

            bundleName = bundleName.ToLower();

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(string.Format("====BundleGroup assetPath is empty"));
                return;
            }
            //if (assetDeps.ContainsKey(assetPath) )
            //{
            //   Debug.LogError(string.Format("====BundleGroup {0} already contain asset {1}", groupName, assetDeps));
            //   return;
            //}
            if (IsUnpack(assetPath))
            {
                //Debug.LogError(string.Format("====BundleGroup {0} can not in assetbundle", assetPath));
                return;
            }

            if (!packDeps)
            {
                bundleNameMap[assetPath] = bundleName;
                gBundleSets.Add(assetPath);
                return;
            }

            string[] deps;
            if (!assetDeps.TryGetValue(assetPath, out deps))
            {
                deps = GetDependencies(assetPath);
                assetDeps.Add(assetPath, deps);

                if (!packSharedDepGlobal)
                {
                    for (int i = 0; i < deps.Length; ++i)
                    {
                        if (IsUnpack(deps[i]))
                        {
                            continue;
                        }

                        List<string> list = null;
                        if (!depBy.TryGetValue(deps[i], out list))
                        {
                            list = new List<string>();
                            depBy.Add(deps[i], list);
                        }
                        list.Add(assetPath);
                    }
                }
            }

            if (packSharedDepGlobal)
            {
                var dirDeps = AssetDatabase.GetDependencies(assetPath, false);
                foreach (var dep in dirDeps)
                {
                    gDirectDepSet.Add(dep);
                }
            }


            if (!string.IsNullOrEmpty(bundleName))
            {
                for (int i = 0; i < deps.Length; ++i)
                {
                    var depPath = deps[i];
                    if (IsUnpack(depPath))
                    {
                        continue;
                    }
                    if (bundleNameMap.ContainsKey(depPath))
                    {
                        continue;
                    }
                    //bundleNameMap[deps[i]] = bundleName;
                    if (packSharedDepGlobal)
                    {
                        if (gDepMap.TryGetValue(depPath, out var lastBundleName))
                        {
                            if (lastBundleName != bundleName)
                                gSharedDeps.Add(depPath);
                        }
                        else
                        {
                            gDepMap.Add(depPath, bundleName);
                        }
                        groupDeps.Add(depPath);
                    }
                    else
                    {
                        bundleNameMap[depPath] = bundleName;
                    }
                }
                bundleNameMap[assetPath] = bundleName;
                gBundleSets.Add(assetPath);
            }
        }

        private static void GetDependencies(string assetPath, HashSet<string> set)
        {
            var deps = AssetDatabase.GetDependencies(assetPath, false);
            foreach (var dep in deps)
            {
                if (IsUnpack(dep))
                {
                    continue;
                }
                set.Add(dep);
                GetDependencies(dep, set);
            }
        }

        private static string[] GetDependencies(string assetPath)
        {
            HashSet<string> set = new HashSet<string>();
            GetDependencies(assetPath, set);
            return set.ToArray();
        }

        public void AddDepAsset(string assetPath, string bundleName, string mainBundleName)
        {
            if (IsUnpack(assetPath))
            {
                return;
            }
            if (gSharedDeps.Contains(assetPath))
            {
                return;
            }
            if (gBundleSets.Contains(assetPath))
            {
                return;
            }
            bundleName = bundleName.ToLower();
            mainBundleName = mainBundleName.ToLower();
            if (BundleGroup.gDepMap.TryGetValue(assetPath, out var oldBundleName))
            {
                if (oldBundleName != mainBundleName)
                {
                    //BundleGroup.gSharedDeps.Add(assetPath);
                    return;
                }
            }
            gDepMap[assetPath] = bundleName;
            gBundleSets.Add(assetPath);
            bundleNameMap[assetPath] = bundleName;
        }

        public void PackDeps()
        {
            foreach (var dep in groupDeps)
            {
                //已经被标记了bundleName，主动加载的bundle
                if (gBundleSets.Contains(dep))
                {
                    continue;
                }
                if (!gSharedDeps.Contains(dep))
                {
                    //不是共享依赖，用第一次设置的bundleName
                    bundleNameMap.Add(dep, gDepMap[dep]);
                }
            }
        }

        public static void ClearGlobal()
        {
            gSharedDeps.Clear();
            gDepMap.Clear();
            gBundleSets.Clear();
        }

        public static bool IsTexture(string assetPath)
        {
            string ext = Path.GetExtension(assetPath).ToLower().TrimStart('.');
            return ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "tga";
        }

        public static bool IsAnim(string assetPath)
        {
            string ext = Path.GetExtension(assetPath).ToLower().TrimStart('.');
            return ext == "anim" || ext == "controller";
        }

        //通过asset路径，计算出安装文件夹合并的包名
        //{父目录}_{文件夹}
        // effect/texture0/1.png
        //      ->   effect_texture0
        public static string getBundleNameByFolder(string assetPath)
        {
            var dirPath = Path.GetDirectoryName(assetPath);
            var parent = Path.GetFileName(Path.GetDirectoryName(dirPath));
            var shareBundleName = parent + "_" + Path.GetFileName(dirPath);
            return shareBundleName;
        }

        private class FolderPacker
        {
            private Dictionary<string, List<string>> packMap = new Dictionary<string, List<string>>();
            public int maxAssetsPerPack;
            public string folderName;

            public FolderPacker(string targetFolder, int maxPerPack)
            {
                maxAssetsPerPack = maxPerPack;
                folderName = targetFolder;
            }

            public void AddAsset(string asset)
            {
                var bundleName = getBundleNameByFolder(asset);
                if (!packMap.TryGetValue(bundleName, out var list))
                {
                    list = new List<string>();
                    packMap[bundleName] = list;
                }
                list.Add(asset);
            }

            public void Pack(HashSet<string> processed, Action<string, string, string> markBundleName)
            {
                foreach (var pair in packMap)
                {
                    pair.Value.RemoveAll((dep) => { return processed.Contains(dep); });
                }

                //打贴图包
                foreach (var pair in packMap)
                {
                    var list = pair.Value;
                    var packName = pair.Key;
                    if (list.Count > maxAssetsPerPack)
                    {
                        list.Sort();
                        int index = 0;
                        int counter = 0;
                        foreach (var asset in list)
                        {
                            if (counter > maxAssetsPerPack)
                            {
                                counter = 0;
                                ++index;
                            }
                            else
                            {
                                ++counter;
                            }
                            markBundleName(asset, folderName, packName + "_" + index);
                        }
                    }
                    else if (list.Count == 1)
                    {
                        markBundleName(list[0], "others", packName);
                    }
                    else
                    {
                        foreach (var asset in list)
                        {
                            markBundleName(asset, folderName, packName);
                        }
                    }
                }
            }
        }

        public static void PackSharedDep(BundleGroup bg)
        {
            Dictionary<string, string> assetBundleMap = new Dictionary<string, string>();
            FolderPacker texPacker = new FolderPacker("pack_tex", 30);
            List<string> mats = new List<string>();

            Action<string, string, string> markBundleName = (assetPath, targetFolder, bundleName) =>
            {
                bundleName = bundleName.Replace(" ", "_");
                assetBundleMap[assetPath] = string.Format("shared/{0}/{1}{2}", targetFolder, bundleName, BundleExt);
            };

            Action<string, string> markBundleNameByFolder = (assetPath, targetFolder) =>
            {
                //按照目录打
                markBundleName(assetPath, targetFolder, getBundleNameByFolder(assetPath));
            };

            HashSet<string> processed = new HashSet<string>();
            foreach (var dep in gSharedDeps)
            {
                //已经被标记了bundleName，主动加载的bundle
                if (gBundleSets.Contains(dep))
                {
                    continue;
                }
                if (processed.Contains(dep))
                {
                    continue;
                }
                if (dep.EndsWith(".prefab"))
                {
                    //prefab依赖可能过多，导致加载一个包依赖了很多其他包
                    //单独一个包
                    var fileName = Path.GetFileNameWithoutExtension(dep);
                    var parent = Path.GetFileName(Path.GetDirectoryName(dep));
                    var bundleName = parent + "_" + fileName;
                    markBundleName(dep, "prefabs", bundleName);

                    //尝试把prefab的依赖打在一起
                    var depdeps = GetDependencies(dep);
                    foreach (var depdep in depdeps)
                    {
                        //必须是已经标为共享的资源
                        if (!gSharedDeps.Contains(depdep))
                        {
                            continue;
                        }
                        //必须是直接依赖的
                        if (!gDirectDepSet.Contains(depdep))
                        {
                            continue;
                        }
                        //处理过的资源排除掉
                        if (processed.Contains(depdep))
                        {
                            continue;
                        }
                        markBundleName(depdep, "prefabs", bundleName);
                        processed.Add(depdep);
                    }
                }
                // else if (IsTexture(dep))
                // {
                //     if (dep.Contains("/effect/"))
                //     {
                //         //延迟标记贴图包名
                //         texPacker.AddAsset(dep);
                //     }
                //     else
                //     {
                //         markBundleNameByFolder(dep,"others");
                //     }
                // }
                // else if (dep.EndsWith(".mat"))
                // {
                //     if (dep.Contains("/effect/"))
                //     {
                //         //TODO
                //         //延迟标记材质包名
                //         markBundleNameByFolder(dep,"pack_mats");
                //     }
                //     else
                //     {
                //         markBundleNameByFolder(dep,"others");
                //     }
                // }
                else
                {
                    markBundleNameByFolder(dep, "others");
                }
            }

            texPacker.Pack(processed, markBundleName);

            bg.bundleNameMap = assetBundleMap;
        }
    }


    private string root;
    public string Root => root;

    private Dictionary<string, BundleGroup> allGroups = new Dictionary<string, BundleGroup>();

    private List<string> externFiles = new List<string>();
    public List<string> ExternFiles => externFiles;


    private List<string> abFiles = new List<string>();
    public List<string> AbFiles => abFiles;


    public AssetBundleBuilder(string root_)
    {
        if (!Directory.Exists(root_))
        {
            Directory.CreateDirectory(root_);
        }
        root = root_.Replace('\\', '/');

        Debug.Log("====AssetBundleBuilder root = " + root);
        BundleGroup.ClearGlobal();
    }


    public BundleGroup GetGroupSafe(string groupName)
    {
        if (groupName == null)
        {
            groupName = string.Empty;
        }

        BundleGroup bg = null;
        if (!allGroups.TryGetValue(groupName, out bg))
        {
            bg = new BundleGroup
            {
                groupName = groupName
            };
            allGroups.Add(bg.groupName, bg);
        }
        return bg;
    }

    public void AddAsset(string groupName, string assetPath, string bundleName, bool packDeps = true)
    {
        BundleGroup bg = GetGroupSafe(groupName);
        bg.AddAsset(assetPath, bundleName, packDeps);
    }

    public void AddDepAsset(string groupName, string assetPath, string bundleName, string mainBundleName)
    {
        if (BundleGroup.packSharedDepGlobal)
        {
            BundleGroup bg = GetGroupSafe(groupName);
            bg.AddDepAsset(assetPath, bundleName, mainBundleName);
        }
        else
        {
            AddAsset(groupName, assetPath, bundleName);
        }
    }

    public void AddAssetFolder(string groupName, string assetFolder, string bundleName, string pattern, SearchOption option)
    {
        List<string> files = EditorCommon.GetAllFiles(assetFolder, option, pattern);

        BundleGroup bg = GetGroupSafe(groupName);
        for (int i = 0; i < files.Count; ++i)
        {
            bg.AddAsset(files[i], bundleName);
        }
    }

    private void CreateBundleName()
    {
        if (BundleGroup.packSharedDepGlobal)
        {
            foreach (var pair in allGroups)
            {
                pair.Value.PackDeps();
            }
            var shared = GetGroupSafe("Shared");
            BundleGroup.PackSharedDep(shared);
            return;
        }

        for (var eGroup = allGroups.GetEnumerator(); eGroup.MoveNext();)
        {
            BundleGroup bg = eGroup.Current.Value;
            // 生成包名
            for (var eDepBy = bg.depBy.GetEnumerator(); eDepBy.MoveNext();)
            {
                string assetPath = eDepBy.Current.Key;
                List<string> depByList = eDepBy.Current.Value;

                string bundleName;
                // 被多个依赖,标记为共享
                if (depByList.Count > 1)
                {
                    bundleName = ToBundleShareName(depByList[depByList.Count - 1]);
                }
                else
                {
                    bundleName = ToBundleName(depByList[0]);
                }
                // 没有标记才重新指定报名
                if (!bg.bundleNameMap.TryGetValue(assetPath, out string oldBundleName)
                    || string.IsNullOrEmpty(oldBundleName))
                {
                    bg.bundleNameMap.Add(assetPath, bundleName);
                }
            }
        }
    }



    public bool Build(BuildAssetBundleOptions options, BuildTarget platform)
    {
        string outputPath = Root;
        AssetBundleManifest manifest = null;
        // 准备好bundle名
        CreateBundleName();
        List<AssetBundleBuild> abbList = new List<AssetBundleBuild>();

        // <assetPath, bundleName> asset被打到哪个包里
        Dictionary<string, string> assetAndBundle = new Dictionary<string, string>();

        // 生成 AssetBundleBuild
        {
            // asset排重
            for (var e = allGroups.GetEnumerator(); e.MoveNext();)
            {
                BundleGroup bg = e.Current.Value;
                for (var eBundle = bg.bundleNameMap.GetEnumerator(); eBundle.MoveNext();)
                {
                    string assetPath = eBundle.Current.Key;
                    string bundleName = eBundle.Current.Value;

                    if (IsForceInShader(assetPath))
                    {
                        bundleName = ShaderBundleName;

                        // 将shader的依赖也标记在shader的包里,有可能会为shader中的texture指定默认纹理，所以会产生额外的依赖
                        string[] deps = AssetDatabase.GetDependencies(assetPath, true);
                        for (int iDeps = 0; iDeps < deps.Length; ++iDeps)
                        {
                            assetAndBundle[deps[iDeps]] = bundleName;
                        }
                    }
                    else if (IsForceInFont(assetPath))
                    {
                        bundleName = FontBundleName;
                    }

                    if (!assetAndBundle.ContainsKey(assetPath))
                    {
                        assetAndBundle[assetPath] = bundleName;
                    }
                }
            }

            if (GenerateShaderFeatureTemplete)
            {
                if (Directory.Exists(MATERIAL_FEATURE_TEMPLETE_PATH))
                {
                    Directory.Delete(MATERIAL_FEATURE_TEMPLETE_PATH, true);
                }
                Directory.CreateDirectory(MATERIAL_FEATURE_TEMPLETE_PATH);

                List<string> matList = new List<string>();
                for (var e = assetAndBundle.GetEnumerator(); e.MoveNext();)
                {
                    string assetPath = e.Current.Key;
                    if (IsMaterial(assetPath))
                    {
                        matList.Add(assetPath);
                    }
                }

                Material[] materials = BuildBundleUtils.GenerateShaderFeatureTempletes(matList.ToArray());
                for (int i = 0; i < materials.Length; ++i)
                {
                    string matPath = string.Format("{0}/{1}.mat", MATERIAL_FEATURE_TEMPLETE_PATH, materials[i].name);
                    AssetDatabase.CreateAsset(materials[i], matPath);
                    assetAndBundle[matPath] = ShaderBundleName;
                }
                AssetDatabase.Refresh();
            }


            // 按bundle整理，bundle里有哪个asset 
            Dictionary<string, List<string>> assetBundleBuild = new Dictionary<string, List<string>>();
            for (var e = assetAndBundle.GetEnumerator(); e.MoveNext();)
            {
                string assetName = e.Current.Key;
                string bundleName = e.Current.Value;

                List<string> assetNames;
                if (!assetBundleBuild.TryGetValue(bundleName, out assetNames))
                {
                    assetNames = new List<string>();
                    assetBundleBuild.Add(bundleName, assetNames);
                }

                assetNames.Add(assetName);
            }

            for (var e = assetBundleBuild.GetEnumerator(); e.MoveNext();)
            {
                string bundleName = e.Current.Key.ToLower();
                List<string> assetNames = e.Current.Value;
                abbList.Add(new AssetBundleBuild { assetBundleName = bundleName, assetNames = assetNames.ToArray() });
            }

            manifest = BuildAssetBundles(outputPath, abbList.ToArray(), options, platform);
            if (manifest == null)
            {
                throw (new Exception("AssetBundleBuilder BuildAssetBundles faild"));
            }
        }

        // 记下打出了那些bundle文件
        {
            abFiles.Add(ASSET_BUNDLE.Replace('\\', '/'));
            abFiles.Add(ASSET_BUNDLE + ".manifest".Replace('\\', '/'));

            for (int i = 0; i < abbList.Count; ++i)
            {
                abFiles.Add(abbList[i].assetBundleName.Replace('\\', '/'));
            }
        }

        // 清理额外的文件
        {
            Dictionary<string, string> allAbFiles = new Dictionary<string, string>();
            for (int i = 0; i < AbFiles.Count; ++i)
            {
                int index = AbFiles[i].IndexOf('.');
                if (index < 0)
                {
                    allAbFiles[AbFiles[i]] = string.Empty;
                }
                else
                {
                    allAbFiles[AbFiles[i].Substring(0, index)] = string.Empty;
                }
            }


            string[] allFiles = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; ++i)
            {
                string file = allFiles[i].Replace('\\', '/').Substring(Root.Length + 1);
                if (file.IndexOf('.') >= 0)
                {
                    file = file.Substring(0, file.IndexOf('.'));
                }
                if (!allAbFiles.ContainsKey(file))
                {

                    Debug.LogWarning("====AssetBundleBuilder delete file " + allFiles[i]);
                    File.Delete(allFiles[i]);
                }
            }
        }


        // 生成资源-包关系
        {
            Debug.Log("====AssetBundleBuilder Create Asset Map");
            AssetBundleMap.SaveTxt(outputPath, assetAndBundle);
        }

        // 包之间关系依赖
        if (manifest != null)
        {
            Debug.Log("====AssetBundleBuilder Create Bundle Dep Map");
            BundleDepMap.SaveTxt(outputPath, manifest);
        }

        // 资源的依赖关系
        {
            Debug.Log("====AssetBundleBuilder Create Asset Dep Map");
            Dictionary<string, string[]> assetDeps = new Dictionary<string, string[]>();
            // asset排重
            for (var e = allGroups.GetEnumerator(); e.MoveNext();)
            {
                var group = e.Current.Value;

                for (var eDep = group.assetDeps.GetEnumerator(); eDep.MoveNext();)
                {
                    string assetName = eDep.Current.Key;
                    string[] deps = eDep.Current.Value;

                    try
                    {
                        assetDeps[assetName] = deps;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
            }

            AssetDepMap.SaveTxt(outputPath, assetDeps);
        }


        return true;
    }


    static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions options, BuildTarget platform)
    {
        if (string.IsNullOrEmpty(outputPath) || builds == null || builds.Length < 1)
        {
            return null;
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, options, platform);

        return manifest;
    }
}