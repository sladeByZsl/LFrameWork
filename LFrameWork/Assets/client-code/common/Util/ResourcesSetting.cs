using System.Collections;
using System.Collections.Generic;
using System.IO;


public enum BundleType
{
    None = -1,

    Particle = 0,
    UIPrefab = 1,
    UIIcon = 2,
    UITexture = 3,
    UISpine = 4,
    UIParticle = 5,

    Cinema = 6,
    Scene = 7,
    Spine = 8,

    Object = 9,
    Audio = 10,
    Vedio = 11,
}


public class ResourcesSetting
{
    public const string BundleExtensions = ".ab";
    public static string PrefabFxFolder = "Assets/Arts/effect/prefab";
    public static string BundleFxFolder = "particle";


    public static string UICommonFolder = "Assets/Arts/ui/common";   // 静态图标，预先就规划好图集;
    public static string UIIconFolder = "Assets/Arts/ui/icon";     // 动态图标，加载后合并图集，比如物品图标;
    public static string UITextureFolder = "Assets/Arts/ui/texture";  // 动态图片，加载后不合并图集，比如小地图、半身像;
    public static string UITextureFolder_HeroIcon = "Assets/Arts/ui/texture/HeroIcon";
    public static string UITextureFolder_Special = "Assets/Arts/ui/texture/Special";
    public static string UITextureFolder_NoneAlpha = "Assets/Arts/ui/texture/NoneAlpha";
    public static string UITextureFolder_Alpha = "Assets/Arts/ui/texture/Alpha";
    public static string UITextureFolder_Scene = "Assets/Arts/ui/texture/Scene";


#if UNITY_ANDROID
	public static string BundleFolder = "DataAndroid";
#elif UNITY_IOS
	public static string BundleFolder = "DataIOS";
#elif UNITY_STANDALONE_WIN
	public static string BundleFolder = "DataPC";
#elif UNITY_STANDALONE_OSX
    public static string BundleFolder = "DataMAC";
#else
	public static string BundleFolder = "DataPC";
#endif

    public static string PackBundlePath
    {
        get
        {
#if UNITY_EDITOR && !UPDATE_TEST
            return string.Format("{0}/{0}", BundleFolder);
#else
			return string.Format("{0}", BundleFolder);
#endif
        }
    }

    public static string GetPrefabBundleName(string path, string folder, string p_type, string suffix = "")
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        fileName = fileName.ToLower();
        filePath = string.Format("{0}_{1}{2}{3}", p_type, fileName, suffix, ResourcesSetting.BundleExtensions);

        string folderPath = PackBundlePath + "/" + folder + "/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return filePath;
    }

    public static string GetBundleName(string path, string folder, bool isRes, string p_type)
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileName(filePath);
        string folderPath;
        folderPath = isRes ? "res/" : "";
        folderPath = folderPath.ToLower();

        fileName = fileName.Replace('.', '_');
        fileName = fileName.Replace('#', '_');
        fileName = fileName.ToLower();

        filePath = string.Format("{0}{1}_{2}{3}", folderPath, p_type, fileName, ResourcesSetting.BundleExtensions);


        folderPath = PackBundlePath + "/" + folder + "/" + folderPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return filePath;
    }

    public static string GetBlockBundleName(string path, string folder)
    {
        string filePath = path;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        fileName = fileName.ToLower();
        filePath = string.Format("{0}_obs{1}", fileName, ResourcesSetting.BundleExtensions);

        string folderPath = ResourcesSetting.BundleFolder + "/" + folder + "/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return filePath;
    }


    public static string GetExtensions(string path, string name)
    {
        string filePath = string.Format("{0}/{1}.png", path, name);

        if (File.Exists(filePath))
        {
            return ".png";
        }

        string directoryName = Path.GetDirectoryName(filePath);

        string[] files = Directory.GetFiles(directoryName);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains(name))
            {
                return Path.GetExtension(files[i]);
            }
        }
        return ".png";
    }
    public static string GetTextureFolder(string p_name)
    {
        string filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder_HeroIcon, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder_HeroIcon;
        }
        filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder_Special, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder_Special;
        }
        filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder_NoneAlpha, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder_NoneAlpha;
        }
        filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder_Alpha, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder_Alpha;
        }
        filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder_Scene, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder_Scene;
        }
        filePath = string.Format("{0}/{1}.png", ResourcesSetting.UITextureFolder, p_name);
        if (File.Exists(filePath))
        {
            return ResourcesSetting.UITextureFolder;
        }
        return "";
    }
#if UNITY_EDITOR
    public static string GetAudioFilePath(string path, string name)
    {
        List<string> audioList = GetAllFiles(path, "*.wav", "*.ogg", "*.mp3");
        foreach (string audio in audioList)
        {
            if (audio.Contains(name))
            {
                return audio;
            }
        }
        return "";
    }

    public static List<string> GetAllFiles(string folder, params string[] searchPatterns)
    {
        List<string> searchPatternList = new List<string>();
        if (string.IsNullOrEmpty(folder))
        {
            return searchPatternList;
        }

        if (!Directory.Exists(folder))
        {
            return searchPatternList;
        }

        if (searchPatterns == null || searchPatterns.Length < 1)
        {
            searchPatternList.Add("");
        }
        else
        {
            foreach (string searchPattern in searchPatterns)
            {
                searchPatternList.Add(searchPattern);
            }
        }

        List<string> fileList = new List<string>();

        GetFileList(ref fileList, searchPatternList, folder);
        return fileList;
    }

    private static void GetFileList(ref List<string> fileList, List<string> searchPatternList, string folder, SearchOption option = SearchOption.AllDirectories)
    {
        foreach (string searchPattern in searchPatternList)
        {
            string[] files = null;
            if (string.IsNullOrEmpty(searchPattern))
            {
                files = Directory.GetFiles(folder, "", option);
            }
            else
            {
                files = Directory.GetFiles(folder, searchPattern, option);
            }

            foreach (string file in files)
            {
                if (fileList.Contains(file))
                {
                    continue;
                }

                string path = file.Replace("\\", "/");
                fileList.Add(path);
            }
        }
    }
#endif
}
