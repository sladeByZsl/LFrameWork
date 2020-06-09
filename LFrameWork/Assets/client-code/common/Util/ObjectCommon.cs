using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCommon
{

	/// <summary>
	/// 
	/// </summary>
	/// <param name="go"></param>
	/// <param name="active"></param>
	public static void SetObjActive(GameObject go, bool active)
	{
		if (go == null)
		{
			return;
		}
		go.SetActive(active);
	}

	public static Transform GetTransChild(Transform trans, string name, bool includeSelf = false)
	{
		if (trans == null || string.IsNullOrEmpty(name))
		{
			return null;
		}

		if (includeSelf && trans.name == name)
		{
			return trans;
		}

		Transform tranChild = trans.Find(name);
		if (tranChild != null)
		{
			return tranChild;
		}

		return null;
	}

	// name -- 支持“x/y/z”表示层级关系。
	public static GameObject GetChild(Transform trans, string name, bool includeSelf = false)
	{
		Transform tranChild = GetTransChild(trans, name, includeSelf);
		if (tranChild == null)
		{
			return null;
		}
		return tranChild.gameObject;
	}

	/// <summary>
	///  获取子节点上的Component
	/// </summary>
	/// <typeparam name="T">Component类型</typeparam>
	/// <param name="go">父节点</param>
	/// <param name="name">子节点名称或者路径</param>
	/// <returns></returns>
	public static T GetChildComponent<T>(Transform trans, string name) where T : Component
	{
		if (trans == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return trans.GetComponent<T>();
		}
		Transform child = GetTransChild(trans, name);
		if (child != null)
		{
			return child.GetComponent<T>();
		}
		return null;
	}

	public static T[] GetComponents<T>(Transform trans, string name) where T : Component
	{
		if (trans == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return trans.GetComponents<T>();
		}
		Transform child = GetTransChild(trans, name);
		if (child != null)
		{
			return child.GetComponents<T>();
		}
		return null;
	}

	public static T AddChildComponent<T>(GameObject go, string name) where T : Component
	{
		if (go == null || string.IsNullOrEmpty(name))
		{
			return null;
		}
		GameObject child = GetChild(go.transform, name);
		if (child != null)
		{
			T component = child.GetComponent<T>();
			if (component == null)
			{
				return child.AddComponent<T>();
			}
			else
			{
				return component;
			}
		}
		else
		{
			return null;
		}
	}

	public static T GetComponentAddIfNotExist<T>(GameObject go) where T : Component
	{
		if (go == null)
		{
			return null;
		}

		T component = go.GetComponent<T>();
		if (component == null)
		{
			return go.AddComponent<T>();
		}
		return component;
	}

	public static GameObject GetDirectChild(GameObject go, string name)
	{
		if (go == null)
		{
			return null;
		}

		Transform rootTransform = go.transform;
		for (int i = 0; i < rootTransform.childCount; i++)
		{
			GameObject goChild = rootTransform.GetChild(i).gameObject;
			if (goChild.name == name)
			{
				return goChild;
			}
		}

		return null;
	}

	public static void SetActive(GameObject gobj, bool active)
	{
		if (gobj == null)
		{
			return;
		}

		for (int i = 0; i < gobj.transform.childCount; i++)
		{
			GameObject childObj = gobj.transform.GetChild(i).gameObject;
			if (childObj != null)
			{
				SetActive(childObj, active);
			}
		}

		if (gobj.activeSelf != active)
		{
			gobj.SetActive(active);
		}

	}

	public static bool SetObjAllChildLayer(GameObject parentObj, int layer)
	{
		if (parentObj == null)
			return false;
		parentObj.layer = layer;
		Transform[] trans = parentObj.GetComponentsInChildren<Transform>(true);
		if (trans != null)
		{
			foreach (Transform tran in trans)
			{
				tran.gameObject.layer = layer;
			}
		}
		return true;
	}

	public static void SetObjectAndChildrenLayer(GameObject gObj, int layer)
	{
		if (gObj == null)
		{
			return;
		}

		gObj.layer = layer;

		Transform rootTransform = gObj.transform;
		for (int i = 0; i < rootTransform.childCount; i++)
		{
			GameObject childObj = rootTransform.GetChild(i).gameObject;
			if (childObj != null)
			{
				SetObjectAndChildrenLayer(childObj, layer);
			}
		}

	}

	public static void SetChildrenActive(GameObject gobj, bool active)
	{
		if (gobj == null)
			return;

		Transform rootTransform = gobj.transform;
		for (int i = 0; i < rootTransform.childCount; i++)
		{
			GameObject childObj = rootTransform.GetChild(i).gameObject;
			if (childObj != null)
				SetActive(childObj, active);
		}
	}

	/* 清除一个Object的所有子节点 */
	public static void DestoryChildren(GameObject gobj)
	{
		Transform rootTransform = gobj.transform;
		List<GameObject> gobjchilds = new List<GameObject>();
		for (int i = 0; i < rootTransform.childCount; i++)
		{
			GameObject gobjChild = rootTransform.GetChild(i).gameObject;
			DestoryChildren(gobjChild);
			gobjchilds.Add(gobjChild);

		}

		rootTransform.DetachChildren();
		foreach (GameObject obj in gobjchilds)
		{
			obj.SetActive(false);
			GameObject.Destroy(obj);
		}
	}

	static public GameObject GetParentGameObject(GameObject childObj, string name)
	{
		if (childObj == null)
		{
			return null;
		}

		Transform t = childObj.transform.parent;
		while (t != null)
		{
			if (t.name == name)
			{
				return t.gameObject;
			}
			t = t.parent;
		}

		return null;
	}

	public static T Instantiate<T>(T go, GameObject parent = null)
		where T : MonoBehaviour
	{
		T info = GameObject.Instantiate(go);
		info.gameObject.SetActive(true);
		info.transform.parent = parent.transform;
		info.transform.localScale = Vector3.one;
		return info;
	}

	public static GameObject Instantiate(GameObject go, GameObject parent = null)
	{
		GameObject info = GameObject.Instantiate(go);
		info.gameObject.SetActive(true);
		info.transform.parent = parent.transform;
		info.transform.localScale = Vector3.one;
		return info;
	}
}
