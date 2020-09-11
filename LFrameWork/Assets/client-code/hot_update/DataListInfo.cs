
using System;
using System.Collections.Generic;
using UnityEngine;
public class DataInfo
{
	public string DataPath;
	public UInt32 CRC;
	public UInt32 Size;
	public bool StreamFile = false;
}

public class DataListInfo
{
	public List<DataInfo> FileList = new List<DataInfo>();  // 必需的基础资源包列表;


	public DataListInfo()
	{
		Debug.Log("----new DataListInfo");
	}

	public DataInfo GetDataInfo(string dataPath)
	{
		// 查找需要下载的资源;
		for (int index = 0; index < FileList.Count; index++)
		{
			DataInfo localDataInfo = FileList[index];
			if (localDataInfo == null)
			{
				continue;
			}

			if (localDataInfo.DataPath == dataPath)
			{
				return localDataInfo;
			}
		}
		return null;
	}

	public static void GetSysFile(DataListInfo localFileList, ref List<string> sysFileList)
	{
		if (localFileList == null)
		{
			return;
		}

		if (sysFileList == null)
		{
			sysFileList = new List<string>();
		}
		string localDataFolder = ResourcesPath.localBundlePath;
		bool find = false;

		// 本地没有安装包里有;
		List<DataInfo> localList = localFileList.FileList;

		// 查找需要下载的资源;
		for (int index = 0; index < localList.Count; index++)
		{
			DataInfo localDataInfo = localList[index];
			if (localDataInfo == null)
			{
				continue;
			}

			if (!localDataInfo.StreamFile)
			{
				continue;
			}

			string path = localDataInfo.DataPath;
			if (string.IsNullOrEmpty(path))
			{
				continue;
			}

			find = System.IO.File.Exists(string.Format("{0}{1}", localDataFolder, localDataInfo.DataPath));
			if (find)
			{
				continue;
			}

			sysFileList.Add(path);
		}
	}

	public static bool CheckAllRes(DataListInfo localListInfo)
	{
		string localDataFolder = ResourcesPath.localBundlePath;

		if (localListInfo == null)
		{
			Debug.Log("CheckAllRes localListInfo == null!!!");
			return false;
		}

		bool find = true;
		// 本地没有安装包里有;
		List<DataInfo> localList = localListInfo.FileList;

		// 查找需要下载的资源;
		for (int index = 0; index < localList.Count; index++)
		{
			DataInfo localDataInfo = localList[index];
			if (localDataInfo == null)
			{
				continue;
			}

			if (localDataInfo.StreamFile)
			{
				continue;
				// 安装包里的不用检查是否存在 ;
				//find = FindStreamFile(localDataInfo.m_DataPath);
			}
			else
			{
				find = System.IO.File.Exists(localDataFolder + localDataInfo.DataPath);
			}
			// 如果列表记录了，但是本地又没有，只能再次下载;
			if (!find)
			{
				Debug.Log(localDataFolder + localDataInfo.DataPath + "----no file!!!!");
				return false;
			}
		}
		return true;
	}

	// 微端判断下载，只做一步检测，资源不在才更新，资源存在跟我没关系;
	public static void GetMiniDiff(ref List<DataInfo> downLoadList, DataListInfo serverListInfo, DataListInfo localListInfo)
	{
		if (downLoadList == null || serverListInfo == null)
		{
			return;
		}
		downLoadList.Clear();

		List<DataInfo> serverList = serverListInfo.FileList;
		if (serverList == null)
		{
			return;
		}

		// 查找需要下载的资源;
		for (int index = 0; index < serverList.Count; index++)
		{
			DataInfo serverDataInfo = serverList[index];
			if (serverDataInfo == null)
			{
				continue;
			}

			bool find = FindLocalFile(serverDataInfo.DataPath);

			if (find && localListInfo != null)
			{
				try
				{
					DataInfo localDataInfo = localListInfo.GetDataInfo(serverDataInfo.DataPath);
					if (localDataInfo != null && localDataInfo.CRC != serverDataInfo.CRC)
					{
						string localDataPath = ResourcesPath.localBundlePath + localDataInfo.DataPath;
						System.IO.File.Delete(localDataPath);
						find = false;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
			}
			// 只有本地没有，才会通过微端下载;
			if (!find)
			{
				downLoadList.Add(serverDataInfo);
			}
		}
	}

	/// <summary>
	/// 查找安装包里的系统文件 ;
	/// </summary>
	/// <param name="dataPath"></param>
	/// <returns></returns>
	public static bool FindStreamFile(string dataPath)
	{
		bool find = false;
#if UNITY_ANDROID && !UNITY_EDITOR

#elif UNITY_IOS && !UNITY_EDITOR
		if (System.IO.File.Exists(string.Format("{0}/Raw/{1}", Application.dataPath, dataPath)))
#endif
		{
			find = true;
		}
		return find;
	}

	/// <summary>
	/// 检查本地可写文件;
	/// </summary>
	/// <param name="dataPath"></param>
	/// <returns></returns>
	public static bool FindLocalFile(string dataPath)
	{
		string localDataFolder = ResourcesPath.localBundlePath;


		bool find = System.IO.File.Exists(localDataFolder + dataPath);
		return find;
	}

	public static void GetRemoveList(ref List<DataInfo> removeList, DataListInfo localListInfo,
		DataListInfo serverListInfo)
	{
		List<DataInfo> localList = localListInfo.FileList;
		List<DataInfo> serverList = serverListInfo.FileList;
		if (localList == null)
		{
			return;
		}
		// 查找需要删除的资源;
		for (int index = 0; index < localList.Count; index++)
		{
			DataInfo localDataInfo = localList[index];
			DataInfo serverDataInfo = GetDataInfo(localDataInfo.DataPath, serverList);

			// 如果本地没有这个文件;
			if (serverDataInfo == null)
			{
				removeList.Add(localDataInfo);
				continue;
			}

		}
	}

	// 获取两个DataList之间的差别;
	public static void GetDiff(ref List<DataInfo> downLoadList, ref List<DataInfo> removeList, DataListInfo localListInfo, DataListInfo serverListInfo)
	{
		if (downLoadList == null || removeList == null || localListInfo == null || serverListInfo == null)
		{
			return;
		}

		downLoadList.Clear();
		removeList.Clear();

		List<DataInfo> localList = localListInfo.FileList;
		List<DataInfo> serverList = serverListInfo.FileList;
		if (serverList == null)
		{
			return;
		}
		// 查找需要下载的资源;
		for (int index = 0; index < serverList.Count; index++)
		{
			DataInfo serverDataInfo = serverList[index];
			if (serverDataInfo == null)
			{
				continue;
			}

			DataInfo localDataInfo = GetDataInfo(serverDataInfo.DataPath, localList);

			// 如果本地FileList没有记录这个文件;
			if (localDataInfo == null)
			{
				downLoadList.Add(serverDataInfo);
				continue;
			}

			// 如果MD5为空，只能重新下载;
			//if (string.IsNullOrEmpty(localDataInfo.m_DataMD5) == true)
			//{
			//    downLoadList.Add(serverDataInfo);
			//    continue;
			//}

			// 如果服务器上的比本地的要新，则需要重新下载;
			if (serverDataInfo.CRC != localDataInfo.CRC)
			{
				downLoadList.Add(serverDataInfo);
				continue;
			}

#if !UNITY_EDITOR
				bool find = false;
				if (localDataInfo.StreamFile)
				{
					find = FindStreamFile(serverDataInfo.DataPath);
				}
				else
				{
					find = FindLocalFile(serverDataInfo.DataPath);
				}

				// 如果列表记录了，但是本地又没有，只能再次下载;
				if (!find)
				{
					downLoadList.Add(serverDataInfo);
					continue;
				}
#else
			bool find = FindLocalFile(serverDataInfo.DataPath);
			if (!find)
			{
				downLoadList.Add(serverDataInfo);
				continue;
			}
#endif
		}

		GetRemoveList(ref removeList, localListInfo, serverListInfo);
	}

	/// <summary>
	/// 重置所有文件的存储标志;
	/// </summary>
	/// <param name="listInfo"></param>
	/// <param name="isStreamFile"></param>
	/// <returns></returns>
	public static bool ResetAllFileFlag(ref DataListInfo listInfo, bool isStreamFile)
	{
		if (listInfo == null)
		{
			return false;
		}

		List<DataInfo> localList = listInfo.FileList;
		if (localList == null)
		{
			return false;
		}

		for (int index = 0; index < localList.Count; index++)
		{
			DataInfo data = localList[index];
			if (data == null)
			{
				continue;
			}
			data.StreamFile = isStreamFile;
		}
		return true;
	}

	/// <summary>
	/// 获取两个DataList之间的差别，并检索StreamingAssets下的资源是否完整;
	/// </summary>
	/// <param name="updateLoadList"></param>
	/// <param name="localListInfo"></param>
	/// <param name="streamListInfo"></param>
	/// <returns></returns>
	public static void GetStreamingAssetsDiff(ref List<DataInfo> updateLoadList, ref DataListInfo localListInfo, DataListInfo streamListInfo)
	{
		if (updateLoadList == null || streamListInfo == null)
		{
			return;
		}

		if (localListInfo == null)
		{
			localListInfo = new DataListInfo();
		}

		updateLoadList.Clear();

		List<DataInfo> localList = localListInfo.FileList;
		List<DataInfo> serverList = streamListInfo.FileList;
		if (serverList == null)
		{
			return;
		}

		// 查找需要下载的资源;
		for (int index = 0; index < serverList.Count; index++)
		{
			DataInfo streamDataInfo = serverList[index];
			if (streamDataInfo == null)
			{
				continue;
			}
			//以streamDataInfo中的路径为key对localList进行查找
			DataInfo localDataInfo = GetDataInfo(streamDataInfo.DataPath, localList);

			// 在安装包中 ;
			streamDataInfo.StreamFile = true;

			bool find = FindStreamFile(streamDataInfo.DataPath);
			if (!find)
			{
				// 安装包里没有有文件;
				if (localDataInfo != null)
				{
					localDataInfo.StreamFile = false;
				}
				continue;
			}
			updateLoadList.Add(streamDataInfo);
		}
	}

	// 删除某个节点;
	public bool RemoveDataInfo(DataInfo dataInfo)
	{
		if (dataInfo == null)
		{
			return false;
		}

		if (FileList.Contains(dataInfo))
		{
			FileList.Remove(dataInfo);
			return true;
		}

		for (int i = 0; i < FileList.Count; i++)
		{
			DataInfo dataInfo2 = FileList[i];
			if (string.Equals(dataInfo.DataPath, dataInfo2.DataPath) == true)
			{
				FileList.RemoveAt(i);
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 向FileList中添加一个资源信息;
	/// </summary>
	/// <param name="path"></param>
	/// <returns>-1.succed 1.type error 2.</returns>
	public DataInfo AddDataInfo(string path, UInt32 crc, uint size = 0)
	{
		DataInfo dataInfo = GetDataInfo(path);

		if (dataInfo == null)
		{
			dataInfo = new DataInfo();
			FileList.Add(dataInfo);
		}

		dataInfo.DataPath = path;
		dataInfo.CRC = crc;
		dataInfo.Size = size;
		return dataInfo;
	}

	public DataInfo RestDataCRC(string path, UInt32 crc)
	{
		DataInfo dataInfo = GetDataInfo(path);
		dataInfo.CRC = crc;
		return dataInfo;
	}

	public void ClearAll()
	{
		FileList.Clear();
	}

	protected static bool ContainsPath(string path, List<DataInfo> dataList)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			DataInfo dataInfo = dataList[i];
			if (string.Equals(dataInfo.DataPath, path) == true)
			{
				return true;
			}
		}

		return false;
	}

	// 根据path，取得DataInfo信息;
	protected static DataInfo GetDataInfo(string path, List<DataInfo> dataList)
	{
		if (dataList == null)
		{
			return null;
		}
		for (int i = 0; i < dataList.Count; i++)
		{
			DataInfo dataInfo = dataList[i];
			if (string.Equals(dataInfo.DataPath, path) == true)
			{
				return dataInfo;
			}
		}

		return null;
	}

	// 获取DataList的大小;
	public UInt64 GetAllSize()
	{
		UInt64 size = 0;

		size += GetListSize(FileList);
		return size;
	}

	// 获取一个List列表的大小;
	public static UInt64 GetListSize(List<DataInfo> dataList)
	{
		if (dataList == null || dataList.Count < 1)
		{
			return 0;
		}

		UInt64 size = 0;

		for (int i = 0; i < dataList.Count; i++)
		{
			DataInfo dataInfo = dataList[i];
			if (dataInfo == null)
			{
				continue;
			}

			size += dataList[i].Size;
		}

		return size;
	}

}