using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    [MenuItem("LFrameWork/ResLoadMode", false, 200)]
    static void CheckResLoadMode()
    {
        bool isToggled = !EditorPrefs.GetBool(ResourcesSetting.kResLoadMode, false);
        EditorPrefs.SetBool(ResourcesSetting.kResLoadMode, isToggled);
        ResourcesSetting.LoaderType = UnityEditor.EditorPrefs.GetBool(ResourcesSetting.kResLoadMode, false) ? ELoaderType.LoaderType_Local : ELoaderType.LoaderType_AssetBundle;
    }

    [MenuItem("LFrameWork/ResLoadMode", true)]
    static bool ValidateSimulationMode()
    {
        bool isToggled = EditorPrefs.GetBool(ResourcesSetting.kResLoadMode, false);
        Menu.SetChecked("LFrameWork/ResLoadMode", isToggled);
        return true;
    }



#if UNITY_ANDROID
	public static BuildTarget platform = BuildTarget.Android;
#elif UNITY_IOS
	public static BuildTarget platform = BuildTarget.iOS;
#elif UNITY_STANDALONE_OSX
    public static BuildTarget platform = BuildTarget.StandaloneOSX;
#else
    public static BuildTarget platform = BuildTarget.StandaloneWindows;
#endif

    [MenuItem("LFrameWork/BuildAll", false, 100)]
    public static void Build()
    {

        BuildBundle();

        string outputPath = ResourcesSetting.PackBundlePath;
        string abRoot = Path.Combine(outputPath, "../", AssetBundleBuilder.ASSET_BUNDLE);
        string copyAbPath = Path.Combine(outputPath, "AssetBundle");
        CopyAssetBundles(abRoot, copyAbPath, false);
        /// createFileList
        string root = Application.dataPath.ToLower().Replace("assets", ResourcesSetting.PackBundlePath);

        var opts = BuildAssetBundleOptions.DeterministicAssetBundle
           | BuildAssetBundleOptions.ChunkBasedCompression;
        BuildBundleUtils.CreateFileList(root, opts, platform);
    }

    public static bool CopyAssetBundles(string abRoot, string copyPath, bool encypt)
    {
        Debug.Log("*********CopyAssetBundles Start*********");
        Debug.Log("*********abRoot = " + abRoot);
        Debug.Log("*********copyPath = " + copyPath);

        if (Directory.Exists(copyPath))
        {
            Directory.Delete(copyPath, true);
        }
        Directory.CreateDirectory(copyPath);

        DirectoryInfo abRootInfo = new DirectoryInfo(abRoot);
        DirectoryInfo copyPathInfo = new DirectoryInfo(copyPath);

        FileInfo[] fileInfos = abRootInfo.GetFiles("*.*", SearchOption.AllDirectories);

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            try
            {
                FileInfo fileInfo = fileInfos[i];
                string filePath = fileInfo.FullName;
                if (fileInfo.Extension == ".manifest" && fileInfo.Name != abRootInfo.Name + ".manifest")
                {
                    // 只拷贝主manifst其他的不拷贝
                    continue;
                }
                else
                {
                    string copyFile = filePath.Replace(abRootInfo.FullName, copyPathInfo.FullName);
                    string copyDir = fileInfo.Directory.FullName.Replace(abRootInfo.FullName, copyPathInfo.FullName);
                    if (!Directory.Exists(copyDir))
                    {
                        Directory.CreateDirectory(copyDir);
                    }
                    fileInfo.CopyTo(copyFile, true);

                   
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        Debug.Log("*********CopyAssetBundles End*********");

        return true;
    }

    public static void BuildBundle()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        var opts = BuildAssetBundleOptions.DeterministicAssetBundle
            | BuildAssetBundleOptions.ChunkBasedCompression;
        string errStr = string.Empty;
        try
        {
            BuildResPipelineFull(ResourcesSetting.PackBundlePath, opts, platform);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            errStr = ex.ToString();
            Debug.LogError("Build Fail:" + errStr);
            return;
        }
        finally
        {
            sw.Stop();
        }

        if (string.IsNullOrEmpty(errStr))
        {
            Debug.Log(string.Format("Build Success 用时{0}秒!!!!!!", sw.ElapsedMilliseconds / 1000.0f));
        }
    }

    public static bool BuildResPipelineFull(string outputPath, BuildAssetBundleOptions options, BuildTarget platform)
    {
        string abRoot = Path.Combine(outputPath, "../", AssetConst.AssetBundle);
        BuildBundle(abRoot, options, platform);

        return true;
    }

    public static bool BuildBundle(string outputPath, BuildAssetBundleOptions options, BuildTarget platform)
    {
        Debug.Log("*********Build Start*********");
        Debug.Log(string.Format("*********{0}*********", outputPath));
        Debug.Log(string.Format("*********{0}*********", options));
        Debug.Log(string.Format("*********{0}*********", platform));

        AssetBundleBuilder builder = new AssetBundleBuilder(outputPath);
        List<ResGroupBase> resGroupList = new List<ResGroupBase>();
        resGroupList.Add(new AssetResGroup("Particle", ResourcesSetting.PrefabFxFolder, BundleType.Particle, "*.prefab"));
        resGroupList.Add(new AssetResGroup("Spine", ResourcesSetting.PrefabSpineFolder, BundleType.Spine, "*.prefab"));
        resGroupList.Add(new AssetResGroup("Video", ResourcesSetting.PrefabVideoFolder, BundleType.Video, "*.mp4"));
        resGroupList.Add(new AssetResGroup("Audio", ResourcesSetting.AudioFolder, BundleType.Audio, "*.wav", "*.ogg", "*.mp3"));
        resGroupList.Add(new AssetResGroup("Object", ResourcesSetting.PrefabObjectFolder, BundleType.Object, "*.prefab"));
        resGroupList.Add(new AssetResGroup("Cinema", ResourcesSetting.PrefabCinemaFolder, BundleType.Cinema, "*.prefab"));

        //resGroupList.Add(new UIBundleGroup());
        //resGroupList.Add(new SceneBundleGroup());


        try
        {
            //打包前做些预处理工作，比如导出lua，修改资源等
            {
                Debug.Log("*********BeforeBuild*********");
                // beforeBuild
                for (int i = 0; i < resGroupList.Count; ++i)
                {
                    ResGroupBase group = resGroupList[i];
                    group.BeforeBuild(builder);
                }
            }
            AssetDatabase.Refresh();

            //收集需要打包的资源
            {
                Debug.Log("*********CollectBundleAssets*********");
                // beforeBuild
                for (int i = 0; i < resGroupList.Count; ++i)
                {
                    ResGroupBase group = resGroupList[i];
                    group.CollectBundleAssets(builder);
                }
            }

            //build
            {
                builder.Build(options, platform);
            }

            //打包后操作，比如清理资源，生成文件索引文件
            {
                Debug.Log("*********AfterBuild*********");
                // beforeBuild
                for (int i = 0; i < resGroupList.Count; ++i)
                {
                    ResGroupBase group = resGroupList[i];
                    group.AfterBuild(builder);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
            throw (ex);
        }
        Debug.Log("*********Build Finish*********");

        return true;
    }
}
