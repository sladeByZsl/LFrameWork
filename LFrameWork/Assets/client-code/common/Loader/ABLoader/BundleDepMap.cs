using System.IO;
using System.Collections.Generic;

using UnityEngine;

namespace LFrameWork.AssetManagement
{
    public class BundleDepMap
    {
        private Dictionary<string, List<string>> bundleDepMap = new Dictionary<string, List<string>>();


        public static readonly string FILE_NAME = "bundle_dep_map";


        public static bool SaveTxt(string root, AssetBundleManifest manifest)
        {
            if (manifest == null)
            {
                Debug.LogError("BundleDepMap.SaveTxt false manifest is null.");
                return false;
            }

            string[] allBundles = manifest.GetAllAssetBundles();

            string filePath = Path.Combine(root, FILE_NAME + ".txt");
            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < allBundles.Length; ++i)
                    {
                        string bundle = allBundles[i];
                        string[] deps = manifest.GetAllDependencies(bundle);
                        string str = string.Format("{0}\t", i);
                        str += string.Format("{0}\t", bundle);
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

        public static bool LoadTxt(string root, BundleDepMap depMap)
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
                                string bundleName = strs[1];
                                List<string> depList = new List<string>();
                                if (strs.Length >= 4)
                                {
                                    string[] deps = strs[3].Split(',');
                                    depList.AddRange(deps);
                                }

                                depMap.bundleDepMap[bundleName] = depList;
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