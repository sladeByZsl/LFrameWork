
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using System;

public partial class BuildBundleUtils
{

    public static Material[] GenerateShaderFeatureTempletes(string[] mats)
    {
        if ( mats == null )
        {
            return null;
        }

        Dictionary<Shader, Dictionary<string, List<string>>> shaderKeywords = new Dictionary<Shader, Dictionary<string, List<string>>>();

        // 收集所有shaderKeyWord
        for (int i = 0; i < mats.Length; ++i)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mats[i]);
            if ( mat == null)
            {
                Debug.LogError("BuildBundleUtils.GenerateShaderFeatureTempletes load material faild " + mats[i]);
                continue;
            }
            if ( mat.shader == null )
            {
                Debug.LogError("BuildBundleUtils.GenerateShaderFeatureTempletes shader is null " + mats[i], mat);
                continue;
            }

            string kwStr = string.Empty;
            List<string> enableKWs = new List<string>();

            string[] keywords = mat.shaderKeywords;
            for ( int ikw = 0; ikw < keywords.Length; ++ikw )
            {
                if ( mat.IsKeywordEnabled(keywords[ikw]) )
                {
                    enableKWs.Add(keywords[ikw]);
                    kwStr += keywords[ikw];
                }
            }

            if ( enableKWs.Count > 0 )
            {
                Dictionary<string, List<string>> kwDic = null;

                if (!shaderKeywords.TryGetValue(mat.shader, out kwDic))
                {
                    kwDic = new Dictionary<string, List<string>>();
                    shaderKeywords.Add(mat.shader, kwDic);
                }

                kwDic[kwStr] = enableKWs;
            }
        }


        List<Material> materialTempletes = new List<Material>();

        int shaderIndex = 0;
        for (var e = shaderKeywords.GetEnumerator(); e.MoveNext(); ++shaderIndex)
        {
            Shader shader = e.Current.Key;
            Dictionary<string, List<string>> kwDic = e.Current.Value;

            int kwIndex = 0;
            for (var eKW = kwDic.GetEnumerator(); eKW.MoveNext(); ++kwIndex)
            {
                List<string> enbleKWs = eKW.Current.Value;

                Material mat = new Material(shader);
                mat.name = string.Format("KeywordTemplete_{0}_{1}", shaderIndex, kwIndex);

                for (int i = 0; i < mat.shaderKeywords.Length; ++i)
                {
                    string kw = mat.shaderKeywords[i];
                    if (enbleKWs.Contains(kw))
                    {
                        mat.EnableKeyword(kw);
                    }
                    else
                    {
                        mat.DisableKeyword(kw);
                    }
                }
                materialTempletes.Add(mat);
            }
        }
    
        return materialTempletes.ToArray();
    }


    private static string AssetsTemp = "Assets/Temp";
    public static void CreateFileList(string packRoot, BuildAssetBundleOptions options, BuildTarget platform)
    {
        DataListInfo dataListInfo = new DataListInfo();
        DirectoryInfo source = new DirectoryInfo(packRoot);
        dataListInfo = AllFileInDir(source, "", dataListInfo);

        BuildFileListInfo(dataListInfo, packRoot, options, platform);
    }

    private static DataListInfo AllFileInDir(DirectoryInfo source, string path, DataListInfo dataListInfo)
    {
        // Check if the target directory exists, if not, create it.
        if (Directory.Exists(source.FullName) == false)
        {
            return dataListInfo;
        }

        //string bdlPath = Application.dataPath.ToLower().Replace("assets", ResourcesSetting.BundleFolder);
        // Copy each file into it's new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            if (fi == null || string.IsNullOrEmpty(fi.Name) || fi.Name.Contains(".meta") ||
                fi.Name.Contains(".manifest") || fi.Name.Contains(ResourcesSetting.PackBundlePath) ||
                fi.Name.Contains("filelist.bundle") || fi.Name.Contains("filelist.txt") || fi.Name.Contains(".DS_Store"))
            {
                continue;
            }

            if (fi.Name.Contains(" "))
            {
                UnityEngine.Debug.LogError("文件路径中含有非法字符（空格）会导致资源无法下载 = " + fi.Name);
                continue;
            }

            string filePath = fi.Name;
            if (!string.IsNullOrEmpty(path))
            {
                filePath = string.Format("{0}/{1}", path, fi.Name);
            }
            UInt32 crc = 0;
            if (filePath.Contains(".bundle"))
            {
                // BuildPipeline.GetCRCForAssetBundle需要存在bundle的manifest
                // 所以路径重新定位到上层目录的原始bundle,生成crc
                string temp = fi.FullName;
                temp = temp.Insert(temp.Length - filePath.Length, "../");
                BuildPipeline.GetCRCForAssetBundle(temp, out crc);
            }
            else
            {
                crc = Util.ComputeCRC32(fi.FullName);
            }

            dataListInfo.AddDataInfo(filePath, crc, (UInt32)fi.Length);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            string dcPath = diSourceSubDir.Name;
            if (!string.IsNullOrEmpty(path))
            {
                dcPath = string.Format("{0}/{1}", path, diSourceSubDir.Name);
            }
            AllFileInDir(diSourceSubDir, dcPath, dataListInfo);
        }

        return dataListInfo;
    }

    static void BuildFileListInfo(DataListInfo dataListInfo, string outDataRoot, BuildAssetBundleOptions options, BuildTarget platform)
    {
        if (dataListInfo == null)
        {
            return;
        }

        if (Directory.Exists(AssetsTemp) == false)
        {
            //Directory.Delete(AssetsTemp, true);
            Directory.CreateDirectory(AssetsTemp);
            AssetDatabase.Refresh();
        }
        //XmlConfigManager.SaveToXmlFile(mDataListInfo, string.Format("{0}/filelist.xml", sPath));

        string dataJson = JsonMapper.ToJson(dataListInfo);
        string tmpConfig = string.Format("{0}/filelist.txt", outDataRoot);
        FileStream fs = File.Open(tmpConfig, System.IO.FileMode.Create);
        StreamWriter writer = new StreamWriter(fs);
        writer.Write(dataJson);
        writer.Flush();
        writer.Close();
        fs.Close();
        //生成临时文件用于打包
        string dirConfig = string.Format("{0}/filelist.txt", AssetsTemp);
        File.Copy(tmpConfig, dirConfig, true);
        AssetDatabase.Refresh();

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = string.Format("filelist{0}", ResourcesSetting.BundleExtensions);
        abb.assetNames = new string[] { dirConfig };

        BuildPipeline.BuildAssetBundles(ResourcesSetting.PackBundlePath, new AssetBundleBuild[] { abb }, options, platform);
        AssetDatabase.Refresh();
    }
}
