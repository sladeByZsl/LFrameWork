
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LFrameWork.Common.Utility;

namespace LFrameWork.Common
{
	public class MonoBehaviourPool<T> where T : MonoBehaviour
	{
		protected Transform mRootTran;  // 父节点，所有本缓存池中的东西都挂载在内部;		
		protected T m_Templete;     // 模板;
		protected string m_TempleteName;

		protected List<T> mMonoList = new List<T>();
		protected List<T> mCacheList = new List<T>();

		protected string templeteName
		{
			get
			{
				if (string.IsNullOrEmpty(m_TempleteName))
				{
					m_TempleteName = typeof(T).Name;
				}

				return m_TempleteName;
			}
		}

		protected Transform rootTran
		{
			get
			{
				if (mRootTran == null)
				{
					GameObject gObj = new GameObject(string.Format("{0}_Pool", templeteName));
					GameObject.DontDestroyOnLoad(gObj);
					mRootTran = gObj.transform;
					mRootTran.parent = null;
				}

				return mRootTran;
			}
		}

		protected T templete
		{
			get
			{
				if (m_Templete == null && rootTran != null)
				{
					GameObject gObj = new GameObject(string.Format("{0}_t", templeteName));
					m_Templete = gObj.AddComponent<T>();

					Transform tran = gObj.transform;
					tran.localPosition = Vector3.zero;
					tran.localEulerAngles = Vector3.zero;
					tran.localScale = Vector3.one;
					tran.parent = rootTran;

					gObj.SetActive(false);
				}
				return m_Templete;
			}
		}

		public T GetMonoBehaviour()
		{
			T mono = null;

			if (mMonoList.Count > 0)
			{
				int index = mMonoList.Count - 1;

				mono = mMonoList[index];
				mMonoList.RemoveAt(index);

				return mono;
			}

			if (templete == null)
			{
				return null;
			}

			mono = GameObject.Instantiate(templete) as T;
			if (mono == null)
			{
				return null;
			}

			mono.gameObject.name = templeteName;

			return mono;
		}

		public void AddToCache(T instance)
		{
			if (instance == null || rootTran == null)
			{
				return;
			}

			Component[] cs = instance.gameObject.GetComponents<Component>();
			if (cs != null)
			{
				for (int i = cs.Length - 1; i >= 0; i--)
				{
					if (cs[i] == instance || cs[i].GetType() == typeof(Transform))
					{
						continue;
					}

					GameObject.Destroy(cs[i]);
				}
			}

			instance.gameObject.SetActive(false);
			instance.gameObject.name = templeteName;

			Transform instanceTran = instance.gameObject.transform;
			if (instanceTran != null)
			{
				instance.gameObject.transform.parent = rootTran;
				instance.gameObject.transform.localEulerAngles = Vector3.zero;
				instance.gameObject.transform.localPosition = Vector3.zero;
				instance.gameObject.transform.localScale = Vector3.one;
			}


			if (mMonoList.Contains(instance))
			{
				return;
			}
			if (mCacheList.Contains(instance))
			{
				return;
			}
			mCacheList.Add(instance);
		}

		public void Clear()
		{
			if (rootTran != null)
			{
				GameObject.Destroy(rootTran.gameObject);
				mRootTran = null;
			}

			m_Templete = null;

			// 如果是Clear，就直接new一个新的出来;
			if (mMonoList != null)
			{
				mMonoList.Clear();
			}
			mMonoList = new List<T>();
		}

		public void Update()
		{
			for (int i = 0; i < mCacheList.Count; i++)
			{
				T instance = mCacheList[i];
				if (instance == null)
				{
					continue;
				}

				if (mMonoList.Contains(instance))
				{
					return;
				}

				mMonoList.Add(instance);
			}
			mCacheList.Clear();
		}
	}
}