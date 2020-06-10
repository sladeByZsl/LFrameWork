using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace LFrameWork.AssetManagement
{
    public class AssetDepMap
    {
        private Dictionary<string, string[]> assetDepMap = new Dictionary<string, string[]>();


        public static readonly string FILE_NAME = "asset_dep_map";


        public static bool SaveTxt(string root, Dictionary<string, string[]> assetDeps)
        {
            if (assetDeps == null)
            {
                Debug.LogError("AssetDepMap.SaveTxt false manifest is null.");
                return false;
            }

            string filePath = Path.Combine(root, FILE_NAME + ".txt");
            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    int i = 0;
                    for (var e = assetDeps.GetEnumerator(); e.MoveNext(); ++i)
                    {
                        string assetPath = e.Current.Key;
                        string[] deps = e.Current.Value;

                        string str = string.Format("{0}\t", i);
                        str += string.Format("{0}\t", assetPath);
                        str += string.Format("{0}\t", deps != null ? deps.Length : 0);
                        for (int iDeps = 0; iDeps < deps.Length; ++iDeps)
                        {
                            if (iDeps == deps.Length - 1)
                            {
                                str += deps[iDeps];
                            }
                            else
                            {
                                str += string.Format("{0},", deps[iDeps]);
                            }
                        }

                        sw.WriteLine(str);
                    }
                    sw.Flush();
                }
            }
            return true;
        }

        public static bool LoadTxt(string root, AssetDepMap assetDdepMap)
        {
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
                            string[] strs = line.Split('\t');

                            if (strs.Length >= 2)
                            {
                                string assetPath = strs[1];
                                string[] deps = null;
                                if (strs.Length >= 4)
                                {
                                    deps = strs[3].Split(',');
                                }

                                assetDdepMap.assetDepMap[assetPath] = deps;
                            }
                            else
                            {
                                Debug.LogError("BundleDepMap.LoadTxt line error, strs.Length < 2 " + line);
                            }

                            line = sr.ReadLine();
                        }
                    }
                }
            }

            return true;
        }
    }
}
