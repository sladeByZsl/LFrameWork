
/**************************************************************************************************
	Copyright (C) 2016 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：ResourceData.cs;
	作	者：jiabin;
	时	间：2016 - 03 - 29;
	注	释：特效打包之后的数据结构;
**************************************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceInfo
{
	public string mName = "";
	public string[] mResources = null;    // 当前特效所依赖的资源;
}

public class ResourceData
{
	public Dictionary<string, ResourceInfo> mDatas = new Dictionary<string, ResourceInfo>();

	public ResourceInfo GetData(string name)
	{
		if (mDatas.ContainsKey(name))
		{
			return mDatas[name];
		}

		return null;
	}

	public void AddData(string key, ResourceInfo data)
	{
		if (string.IsNullOrEmpty(key) || data == null)
		{
			return;
		}

		mDatas[key] = data;
	}

	public void Clear()
	{
		mDatas.Clear();
	}
}