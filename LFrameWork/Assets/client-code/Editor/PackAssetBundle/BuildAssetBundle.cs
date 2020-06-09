using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle 
{
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
    public static void MenuBuild()
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
