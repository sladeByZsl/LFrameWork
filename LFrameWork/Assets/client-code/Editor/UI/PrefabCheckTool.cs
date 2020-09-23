
/**************************************************************************************************
	Copyright (C) 2016 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：PrefabCheckTool.cs;
	作	者：jiabin;
	时	间：2017 - 04 - 26;
	注	释：监听Prefab的变化;
**************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabCheckTool
{
	private static List<string> checkedList = new List<string>();
	private static List<string> delayList = new List<string>();

	static string FxPrefab_Folder = "arts/effect/prefab";
    static string UIFxPrefab_Folder = "arts/ui/particle";
    [InitializeOnLoadMethod]
	static void StartInitializeOnLoadMethod()
	{
		// 监听Apply操作;		
        EditorApplication.update += OnEditorUpdate;
	}

	static void OnEditorUpdate()
	{
		checkedList.Clear();

		for ( int i=0; i<delayList.Count; i++ )
		{
			GameObject gObj = AssetDatabase.LoadAssetAtPath<GameObject>(delayList[i]);
			CheckPrefab(gObj, delayList[i]);
		}
		delayList.Clear();
    }

	public static void ApplyOptions(string prefabPath)
	{
		if (checkedList.Contains(prefabPath))
		{
			return;
		}
		checkedList.Add(prefabPath);

		GameObject gObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		if ( gObj == null )
		{
			if (!delayList.Contains(prefabPath))
			{
				delayList.Add(prefabPath);
			}
		}
		CheckPrefab(gObj, prefabPath);
	}

	public static void ApplyOptions(GameObject instance)
	{
		Debug.LogError(instance);
		if (instance == null)
		{
			return;
		}
		Object obj = PrefabUtility.GetCorrespondingObjectFromSource(instance);
        string prefabPath = AssetDatabase.GetAssetPath(obj);
		CheckPrefab(instance, prefabPath);
	}

	static void CheckPrefab(GameObject instance, string prefabPath)
	{
		if (instance == null)
		{
			return;
		}

		//#region 检测丢失引用的脚本;
		//List<string> errorList = new List<string>();
		//CheckComponents(instance.transform, prefabPath, errorList);
		//if (errorList.Count > 0)
		//{
		//	string errorInfo = string.Format("{0} 有丢失引用的脚本\n如果你不会处理请赶紧召唤杨旸", prefabPath);
		//	foreach (string error in errorList)
		//	{
		//		errorInfo = string.Format("{0}\n{1}", errorInfo, error);
		//	}

		//	EditorUtility.DisplayDialog("特效 Error", string.Concat("\n\n", errorInfo), "OK");
		//}
		//#endregion


		if (prefabPath.ToLower().Contains(FxPrefab_Folder))
		{
			CheckFxPrefab(instance, prefabPath);
        }
		else if(prefabPath.ToLower().Contains(UIFxPrefab_Folder))
		{
            CheckFxPrefab(instance, prefabPath);
        }
	}

	static void CheckComponents(Transform tran, string prefabPath, List<string> errorList)
	{
		if (tran == null)
		{
			return;
		}

		MonoBehaviour[] monobehaviours = tran.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monobehaviour in monobehaviours)
		{
			if (monobehaviour == null)
			{
				string fullPath = string.Format("\n{0}", GetGameObjectFullPath(tran));
				if (!errorList.Contains(fullPath))
				{
					Debug.LogError(string.Format("有丢失的脚本引用{0}", fullPath), tran);
					errorList.Add(fullPath);
				}
			}
		}

		for (int i = 0; i < tran.childCount; i++)
		{
			CheckComponents(tran.GetChild(i), prefabPath, errorList);
		}
	}

    static public string GetGameObjectFullPath(Transform tran)
    {
        if (tran == null)
        {
            return "";
        }

        string fullPath = tran.name;
        Transform parentTran = tran.parent;
        while (parentTran != null)
        {
            fullPath = string.Format("{0}/{1}", parentTran.name, fullPath);
            parentTran = parentTran.parent;
        }
        return fullPath;
    }

    static void CheckFxPrefab(GameObject instance, string prefabPath)
	{
		if ( instance == null )
		{
			return;
		}

		List<string> errorList = new List<string>();
		
        bool hasSave = ReplacePrefab(instance, prefabPath);
        if (errorList.Count > 0)
		{
			string errorInfo = string.Format("{0} 特效资源错误;", prefabPath);
			foreach (string error in errorList)
			{
				errorInfo = string.Format("{0}\n{1}", errorInfo, error);
			}

			EditorUtility.DisplayDialog("Prefab Error", string.Concat("\n\n", errorInfo), "OK");
		}
		if (hasSave)
		{
			AssetDatabase.Refresh();
		}
    }

    static bool ReplacePrefab(GameObject p_obj, string p_path)
    {
		bool shouldSave = false;
        Renderer[] renders = p_obj.GetComponentsInChildren<Renderer>();
        if (null != renders)
        {

            foreach (Renderer render in renders)
            {
                if(render.sharedMaterial == null || string.IsNullOrEmpty(render.sharedMaterial.name))
                {
                    continue;
                }
                if (render.sharedMaterial.name.Contains("Default"))
                {
                   // Debug.LogError(render.name);
                    render.sharedMaterials = new Material[render.sharedMaterials.Length];
					shouldSave = true;
                }
            }

			if (shouldSave)
			{
				PrefabUtility.SavePrefabAsset(p_obj);
			}
        }
		return shouldSave;
    }
}