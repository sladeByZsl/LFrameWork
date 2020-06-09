using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class EditorCommon
{
	public static int PackMode = 0;

	public static List<string> GetPrefabs(string folder)
	{
		return GetAllFiles(folder, "*.prefab");
	}
	public static List<string> GetBlocks(string folder)
	{
		return GetAllFiles(folder, "*.obs");
	}

	public static List<string> GetBundles(string folder)
	{
		return GetAllFiles(folder, "*" + ResourcesSetting.BundleExtensions);
	}
	public static List<string> GetTextures(string folder)
	{
		return GetAllFiles(folder, "*.png", "*.jpg");
	}
	public static List<string> GetSpines(string folder)
	{
		return GetAllFiles(folder, "*.asset");
	}
	public static List<string> GetAudio(string folder)
	{
		return GetAllFiles(folder, "*.wav", "*.ogg", "*.mp3");
	}
	public static List<string> GetLua(string folder)
	{
		return GetAllFiles(folder, "*.lua");
	}
	public static List<string> GetConf(string folder)
	{
		return GetAllFiles(folder, "*.dat", "*.bin");
	}

	public static List<string> GetVedio(string folder)
	{
		return GetAllFiles(folder, "*.mp4");
	}

	public static List<string> GetAllFiles(string folder, SearchOption option, params string[] searchPatterns)
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

		GetFileList(ref fileList, searchPatternList, folder, option);
		return fileList;
	}

    /// <summary>
    /// 获取文件相对于Assets的目录
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="searchPatterns"></param>
    /// <returns></returns>
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

		//string[] directories = Directory.GetDirectories(folder);
		//foreach (string directory in directories)
		//{
		//	List<string> files = GetAllFiles(directory, searchPattern);
		//	if (files == null || files.Count < 1 )
		//	{
		//		continue;
		//	}

		//	foreach (string file in files)
		//	{
		//		if (fileList.Contains(file))
		//		{
		//			continue;
		//		}

		//		fileList.Add(file);
		//	}
		//}

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
	public static bool IsPrefab(string path)
	{
		return IsRes(path, ".prefab");
	}
	public static bool IsAtlas(string path)
	{
		return false;
		//bool result = IsRes(path, ".prefab");
		//if (!result)
		//{
		//	return false;
		//}

		//GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

		//return obj.GetComponent<UIAtlas>() != null;
	}

	public static bool IsTexture(string path)
	{
		return IsRes(path, ".jpg", ".tga", ".dds", ".png", ".dxt", ".psd", ".bmp", ".cubemap");
	}

	public static bool IsFont(string path)
	{
		return IsRes(path, ".ttf");
	}

	public static bool IsAnimatorController(string path)
	{
		return IsRes(path, ".controller");
	}

	public static bool IsModel(string path)
	{
		return IsRes(path, ".fbx");
	}
	public static bool isMat(string path)
	{
		return IsRes(path, ".mat");
	}
	public static bool isShader(string path)
	{
		return IsRes(path, ".shader");
	}


	public static bool IsCS(string path)
	{
		return IsRes(path, ".cs");
	}

	public static bool IsRes(string path, params string[] extensions)
	{
		if (string.IsNullOrEmpty(path) || extensions == null || extensions.Length < 1)
		{
			return false;
		}

		string extension = System.IO.Path.GetExtension(path);
		if (string.IsNullOrEmpty(extension))
		{
			return false;
		}
		extension = extension.ToLower();

		for (int i = 0; i < extensions.Length; i++)
		{
			if (string.Equals(extensions[i], extension))
			{
				return true;
			}
		}

		return false;
	}
}