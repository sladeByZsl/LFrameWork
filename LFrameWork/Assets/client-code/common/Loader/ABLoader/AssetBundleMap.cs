using System.Collections.Generic;
using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;


namespace LFrameWork.AssetManagement
{
    public class AssetBundleMap
    {
        public static readonly string FILE_NAME = "bundle_map";

        public static bool forceLower = true;

        private struct AssetPathInfo
        {
            public string abPath;
            public string ext;
        }

        private Dictionary<string, AssetPathInfo> assetMap = new Dictionary<string, AssetPathInfo>();

        public bool GetAbPath(string assetPath, out string abPath, out string assetName)
        {
            string path = forceLower ? assetPath.ToLower() : assetPath;
            if (!assetMap.TryGetValue(path, out var info))
            {
                abPath = null;
                assetName = null;
                return false;
            }

            abPath = info.abPath;
            assetName = path + info.ext;
            return true;
        }

        public string GetBundle(string str)
        {
            string path = forceLower ? str.ToLower() : str;
            if (!assetMap.TryGetValue(path, out var info))
            {
                return null;
            }
            return info.abPath;
        }


        public static bool LoadTxt(string root, AssetBundleMap abMap)
        {
            if (forceLower)
            {
                Debug.LogError("====AssetBundleMap forceLower");
            }

            string path = Path.Combine(root, FILE_NAME + ".txt");

            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                if (fs != null)
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            ParseLine(line, abMap);
                            line = sr.ReadLine();
                        }

                    }
                }
            }

            return true;
        }

        public static IEnumerator LoadFromWeb(string url, AssetBundleMap abMap)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();
                string txt = uwr.downloadHandler.text;
                string[] lines = txt.Split('\n');
                foreach (var line in lines)
                {
                    ParseLine(line, abMap);
                }
            }
        }

        private static void ParseLine(string line, AssetBundleMap abMap)
        {
            string[] strs = line.Split(':');
            if (strs == null || strs.Length != 2)
            {
                Debug.LogError(string.Format("==== readLine Error {0}", line));
            }

            string assetFullPath = strs[0];
            string assetName = assetFullPath.Substring(0, assetFullPath.LastIndexOf('.'));
            if (forceLower)
            {
                assetName = assetName.ToLower();
            }
            string ext = Path.GetExtension(assetFullPath);
            string bundlePath = strs[1];

            abMap.assetMap[assetName] = new AssetPathInfo()
            {
                abPath = bundlePath,
                ext = ext,
            };
        }

        public static bool SaveTxt(string root, Dictionary<string, string> bundleMap)
        {

            string filePath = Path.Combine(root, FILE_NAME + ".txt");
            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    for (var e = bundleMap.GetEnumerator(); e.MoveNext();)
                    {
                        string assetName = e.Current.Key.Replace('\\', '/');
                        string bundleName = e.Current.Value.Replace('\\', '/');

                        //assetName = assetName.Substring(0, assetName.LastIndexOf('.'));

                        sw.WriteLine(string.Format("{0}:{1}", assetName, bundleName));
                    }

                    sw.Flush();
                }
            }

            return true;
        }
    }
}

