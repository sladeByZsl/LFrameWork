
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using LFrameWork.Common.Utility;

namespace LFrameWork.Common.Resource
{
	public delegate bool ParticleComplete(string key, GameObject gObj);

	// 只在Editor下暴露出来访问权限，用于给Inspector获取信息;
#if UNITY_EDITOR
	public class ParticleResMgr : MonoBehaviourSingle<ParticleResMgr>
#else
	internal class ParticleResMgr : MonoBehaviourSingle<ParticleResMgr>
#endif
	{
		protected ResourceData m_ParticleData;
		protected Int32 m_CacheMaxCount = 2;

		internal static bool s_playing = true;

		void Awake()
		{
			s_playing = true;
			cacheMaxCount = 2;
		}

		protected override void OnApplicationQuit()
		{
			s_playing = false;
			base.OnApplicationQuit();
		}

		protected Dictionary<string, GameObject> m_ParTempMap = new Dictionary<string, GameObject>();   // 特效的模板map;
		protected CommonCounter<string, GameObject> m_CacheCounter = new CommonCounter<string, GameObject>();   // 缓存池计数器;
		protected CommonCounter<string, GameObject> m_ActiveCounter = new CommonCounter<string, GameObject>();  // 真实资源计数器;
		protected List<string> m_UselessList = new List<string>();  // 无用的，待删除列表;

		// -------------------------------------------------------------------------------------------------------- //
		public Int32 cacheMaxCount
		{
			get { return m_CacheMaxCount; }

			set
			{
				m_CacheMaxCount = value;
				m_CacheCounter.maxCount = m_CacheMaxCount;
			}
		}

		/// <summary>
		/// 将不需要的特效，放入缓存池;
		/// </summary>
		/// <param name="key"></param>
		/// <param name="gObj"></param>
		public void AddCache(string key, GameObject gObj)
		{
			if (gObj == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(key))
			{
				GameObject.Destroy(gObj);
				return;
			}

			//if ( m_CacheCounter.GetTypeCount(key) >= cacheMaxCount)
			//{
			//	GameObject.Destroy(gObj);
			//}
			//else
			{
				gObj.name = key;
				gObj.SetActive(false);
				gObj.transform.parent = tran;
				gObj.layer = UnityEngine.LayerMask.NameToLayer("Default");
				m_CacheCounter.AddValue(key, gObj, true);
			}

			int activeCount = m_ActiveCounter.RemoveValue(gObj.name, gObj);
			if (activeCount <= 0 && !m_UselessList.Contains(key))
			{
				m_UselessList.Add(key);
			}

		}

		public void SetParticleData(ResourceData data)
		{
			m_ParticleData = data;
		}

		/// <summary>
		/// 加载特效资源;
		/// </summary>
		/// <param name="key">特效名称</param>
		/// <param name="parIns">特效实例</param>
		public void LoadRes(string key, ParticleComplete fun)
		{
			if (string.IsNullOrEmpty(key))
			{
				ActionLoadCompleteDel(fun, key, null);
				return;
			}

			GameObject gObj = GetParticleObj(key);
			if (gObj == null)
			{
				AssetManageAdpater.GetInstance().LoadParticle(key, (go) => {
					ActionLoadCompleteDel(fun, key, go);
				});
				return;
			}

			ActionLoadCompleteDel(fun, key, gObj);
		}

		public void ClearAllUseless()
		{
			for (int i = m_UselessList.Count - 1; i >= 0; i--)
			{
				ClearUseless(m_UselessList[i]);
			}
			m_UselessList.Clear();
		}


		public void ClearUseless(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}

			if (!m_UselessList.Contains(key))
			{
				return;
			}

			DestoryTemplete(key);
			DestoryCache(key);

			m_UselessList.Remove(key);
		}

		// -------------------------------------------------------------------------------------------------------- //

		protected void DestoryTemplete(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}

			GameObject gObj = null;
			if (!m_ParTempMap.TryGetValue(key, out gObj))
			{
				return;
			}

			if (gObj == null)
			{
				return;
			}

			m_ParTempMap.Remove(key);
			GameObject.Destroy(gObj);
		}

		protected void DestoryCache(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}

			List<GameObject> particleCacheList = m_CacheCounter.GetList(key);
			m_CacheCounter.RemoveType(key);

			if (particleCacheList == null)
			{
				return;
			}

			for (int i = particleCacheList.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(particleCacheList[i]);
			}
			particleCacheList.Clear();
		}

		/// <summary>
		/// 从缓存池获取或者从模板中clone一个实例;
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected GameObject GetParticleObj(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}

			// 从缓存池中弹出一个特效;
			GameObject gObj = m_CacheCounter.PopupValue(key);
			if (gObj != null)
			{
				gObj.SetActive(true);
				return gObj;
			}

			// 如果缓存池中没有，则从模板表中获取;
			GameObject tempObj = null;
			m_ParTempMap.TryGetValue(key, out tempObj);
			if (tempObj == null)
			{
				return null;
			}

			GameObject parObj = GameObject.Instantiate(tempObj) as GameObject;
			parObj.name = key;
			parObj.SetActive(true);

			return parObj;
		}

		protected void ActionLoadCompleteDel(ParticleComplete fun, string key, GameObject gObj)
		{
			if (fun == null)
			{
				return;
			}

			try
			{
				bool result = fun(key, gObj);
				if (!result)
				{
					return;
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError(ex.ToString());

				m_CacheCounter.AddValue(key, gObj);
				return;
			}

			if (gObj == null)
			{
				return;
			}

			m_ActiveCounter.AddValue(key, gObj);
			if (m_UselessList.Contains(key))
			{
				m_UselessList.Remove(key);
			}
		}

		protected void AddTemplete(string key, GameObject gObj)
		{
			if (string.IsNullOrEmpty(key) || gObj == null)
			{
				return;
			}

			if (m_ParTempMap.ContainsKey(key))
			{
				return;
			}

			gObj.transform.parent = tran;
			gObj.SetActive(false);

			m_ParTempMap.Add(key, gObj);
		}

		#region Inspector_Function
#if UNITY_EDITOR
		public enum EParticleResState
		{
			EParticleResState_Active,
			EParticleResState_Cache,
			EParticleResState_All,
		}

		public List<string> GetParticleTypes(EParticleResState state)
		{
			switch (state)
			{
				case EParticleResState.EParticleResState_Active:
					return m_ActiveCounter.GetAllTypes();

				case EParticleResState.EParticleResState_Cache:
					return m_CacheCounter.GetAllTypes();
			}

			return new List<string>();
		}

		public Int32 GetAllParticleCount(EParticleResState state)
		{
			switch (state)
			{
				case EParticleResState.EParticleResState_Active:
					return m_ActiveCounter.GetTotalCount();

				case EParticleResState.EParticleResState_Cache:
					return m_CacheCounter.GetTotalCount();

				case EParticleResState.EParticleResState_All:
					return m_ActiveCounter.GetTotalCount() + m_CacheCounter.GetTotalCount();
			}

			return 0;
		}

		public Int32 GetParticleCount(string key, EParticleResState state)
		{
			if (string.IsNullOrEmpty(key))
			{
				return 0;
			}

			switch (state)
			{
				case EParticleResState.EParticleResState_Active:
					return m_ActiveCounter.GetTypeCount(key);

				case EParticleResState.EParticleResState_Cache:
					return m_CacheCounter.GetTypeCount(key);
			}

			return 0;
		}

		public List<string> GetUselessList()
		{
			return m_UselessList;
		}
#endif
		#endregion
	}
}