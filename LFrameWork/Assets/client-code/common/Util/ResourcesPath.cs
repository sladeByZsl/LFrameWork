
/**************************************************************************************************
	Copyright (C) 2016 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：ResourcesPath.cs;
	作	者：jiabin;
	时	间：2017 - 03 - 21;
	注	释：;
**************************************************************************************************/

using UnityEngine;

public enum AssetPathType
{
    Type_Default,   // 只有使用Default时，才有可能使用Resources加载;

    Type_StreamingAssets,   // StreamingAssets文件夹;		
    Type_StreamingAssetsBundle, // StreamingAssets文件夹下存放Bundle的根目录;	
    Type_Local, // 本地路径;
    Type_LocalBundle,   // 本地路径存放Bundle的根目录;

    Type_RunAssets, // 为了以后可能存在的优化使用（优先使用local，没有则使用streamingassets）;
}

public class ResourcesPath
{
    protected static string mStreamingAssetsPath = null;
    protected static string mStreamingAssetsBundlePath = null;
    protected static string mStreamingAssetsBundlePathzip = null;
    protected static string mLocalPath = null;
    protected static string mLocalBundlePath = null;
    protected static string mLocalBundlePathzip = null;
    protected static string mServerPath = null;
    protected static string mAppContentPath = null;
    protected static string mAppContentBundlePath = null;
    protected static string m_StreamingAssetWWWPath = null;


    public static string streamingAssetsPath
    {
        get
        {
            if (string.IsNullOrEmpty(mStreamingAssetsPath))
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.WindowsEditor:
                        {
                            mStreamingAssetsPath = string.Format("{0}/../", Application.dataPath);
                            break;
                        }
                    default:
                        mStreamingAssetsPath = string.Format("{0}/", Application.streamingAssetsPath);
                        break;
                }
            }

            return mStreamingAssetsPath;
        }
    }

    public static string streamingAssetWWWPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_StreamingAssetWWWPath))
            {
#if UNITY_EDITOR
                m_StreamingAssetWWWPath = string.Format("file://{0}/", Application.streamingAssetsPath);
#elif UNITY_ANDROID
				m_StreamingAssetWWWPath = string.Format("{0}/", Application.streamingAssetsPath);
#else
				m_StreamingAssetWWWPath = string.Format("file://{0}/", Application.streamingAssetsPath);
#endif
            }

            return m_StreamingAssetWWWPath;
        }
    }


    public static string streamingAssetsBundlePath
    {
        get
        {
            if (string.IsNullOrEmpty(mStreamingAssetsBundlePath))
            {
                mStreamingAssetsBundlePath = string.Format("{0}{1}/", streamingAssetsPath, ResourcesSetting.BundleFolder);
            }

            return mStreamingAssetsBundlePath;
        }
    }
    public static string streamingAssetsBundlePathZip
    {
        get
        {
            if (string.IsNullOrEmpty(mStreamingAssetsBundlePathzip))
            {
                mStreamingAssetsBundlePathzip = string.Format("{0}{1}.7z", streamingAssetsPath, ResourcesSetting.BundleFolder);
            }

            return mStreamingAssetsBundlePathzip;
        }
    }
    public static string localPath
    {
        get
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            mLocalPath = string.Format("{0}/../", Application.dataPath);
#else
			mLocalPath = string.Format("{0}/", Application.persistentDataPath);
#endif
            return mLocalPath;
        }
    }

    public static string localBundlePath
    {
        get
        {
            if (string.IsNullOrEmpty(mLocalBundlePath))
            {
                mLocalBundlePath = string.Format("{0}{1}/", localPath, ResourcesSetting.PackBundlePath);
            }
            return mLocalBundlePath;
        }
    }
    public static string localBundlePathZip
    {
        get
        {
            if (string.IsNullOrEmpty(mLocalBundlePathzip))
            {
                mLocalBundlePathzip = string.Format("{0}{1}.7z", localPath, ResourcesSetting.BundleFolder);
            }
            return mLocalBundlePathzip;
        }
    }
    public static string AppContentPath
    {
        get
        {
            return streamingAssetsPath;
        }

    }

    public static string AppContentBundlePath
    {
        get
        {
            return streamingAssetsBundlePath;
        }
    }

    public static string AppContentBundlePathZip
    {
        get
        {
            return streamingAssetsBundlePathZip;
        }
    }
}