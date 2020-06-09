using UnityEngine;
using System;
using LFrameWork.Common.Resource;
using System.Collections.Generic;

namespace LFrameWork.Common
{
	/// <summary>
	/// 特效类型;
	/// </summary>
	public enum EParticleType
	{
		Unknown = 0,
		BombEffect = 1,
		KiteTrailEffect = 2,
		KiteEffect = 3,
		KiteFlyingPrefab = 4,
		DestroyChipEffect = 5,
		HitChipEffect = 6,
		HRocketEffect = 7,
		VRocketEffect = 8,
		RainbowEffect = 9,
		RainbowLightningEffect = 10,
		RainbowLightPathEffect = 11,
		RainbowEndEffect = 12,
		Static = 13,
		UI = 14,
		ChipDestroy = 16,
		BulletEffect = 17,
		SwapNormalEffect = 18,
		Other = 19,
		DropEffect = 20,
		RocketEffect = 21,


		Max,    //计数用 如新增特效类型需在上面↑↑↑↑↑↑↑↑↑↑
	}

	public class ParticleInstance : MonoBehaviour
	{
		public delegate void ParticleLoadedComplete(ParticleInstance particleSystem);
		public ParticleLoadedComplete OnParticleLoadedComplete;

		protected GameObject m_Root;
		protected Transform m_RootTran;

		public string m_ParticleKey = "";
		protected int m_Layer = 0; //LayerMask.NameToLayer("Default") = 0

		protected bool m_AutoRemove = false;
		protected float m_RemoveTime = 0;

		public int Id { get; set; }
		protected GameObject root
		{
			get
			{
				if (m_Root == null)
				{
					m_Root = gameObject;
				}
				return m_Root;
			}
		}

		public int layer
		{
			set
			{
				if (m_Layer == value)
				{
					return;
				}

				m_Layer = value;

				ObjectCommon.SetObjectAndChildrenLayer(gameObject, m_Layer);
			}
		}

		public Transform rootTran
		{
			get
			{
				if (m_RootTran == null && root != null)
				{
					m_RootTran = root.transform;
				}

				return m_RootTran;
			}
		}

		protected GameObject m_ParticleObj = null;
		protected bool m_UseFake = false;
		protected bool m_Loaded = false;
		protected bool m_Failed = false;

		protected bool m_Playing = false;

		protected EParticleType mParticleType;

		//新版 Unity 粒子系统对象;
		protected ParticleSystem[] mParticleSystem;

		protected ParticleSystem m_MainParticleSystem;

		protected ParticleComplete mParticleCompleteFun;

		protected Action mClearCompleteFun;

		protected bool m_UseLogicDuration = false;
		protected bool m_IsLoop = false;
		protected float m_Duration = 0;
		protected float m_StartTime = 0f;

		public ParticleInfo particleInfo { get; set; }
		protected float m_delayTime = 0;
		protected bool m_needDelay = false;

		public EParticleType particleType
		{
			get { return mParticleType; }

			set { mParticleType = value; }
		}

		public void SetAutoRemove(float durtion, float delayTime, bool unFollow, Action removeCompleteCallback = null)
		{
			mClearCompleteFun = removeCompleteCallback;
			if (delayTime == 0)
			{
				SetAutoRemove(durtion, unFollow);
			}
			else
			{
				m_needDelay = true;
				m_delayTime = Time.time + durtion;
				SetAutoRemove(durtion + delayTime, unFollow);
			}
		}

		public void SetAutoRemove(float durtion, bool unFollow)
		{
			m_AutoRemove = true;
			m_RemoveTime = Time.time + durtion;

			if (unFollow == true)
			{
				rootTran.parent = null;
			}
		}

		public bool useFake
		{
			get
			{
				return m_UseFake;
			}
		}

		public bool loadFailed
		{
			get
			{
				return m_Failed;
			}
		}

		public bool empty
		{
			get
			{
				return useFake || loadFailed;
			}
		}

		public float Duration
		{
			get
			{
				if (m_UseLogicDuration)
				{
					return m_Duration;
				}

				if (m_MainParticleSystem != null)
				{
					return m_MainParticleSystem.main.duration;
				}

				return 5f;
			}
		}

		public ParticleSystem[] ParticleSystems
		{
			get
			{
				if (mParticleSystem == null)
				{
					if (m_ParticleObj != null)
						mParticleSystem = m_ParticleObj.GetComponentsInChildren<ParticleSystem>(true);
				}
				return mParticleSystem;
			}
		}
		public GameObject ParticleObj
		{
			get { return m_ParticleObj; }
		}
		public float StartTime
		{
			get
			{
				if (m_MainParticleSystem != null)
				{
					return m_MainParticleSystem.main.startLifetimeMultiplier;
				}
				return 0;
			}
		}

		

		private int effectCamera;
		public int EffectCamera { get => effectCamera; set => effectCamera = value; }

		

		public void Play()
		{
			m_StartTime = Time.time;
			m_Playing = true;
			if (m_ParticleObj == null)
			{
				return;
			}

			if (ParticleSystems == null)
			{
				return;
			}

			for (int i = 0; i < ParticleSystems.Length; i++)
			{
				if (ParticleSystems[i] == null)
					continue;

				//ParticleSystems[i].enableEmission = true;
				ParticleSystems[i].Play(true);
			}
		}

		public void PlayerOnce(bool unFollow)
		{
			SetAutoRemove(Duration, unFollow);
			Play();
		}

		public void Stop()
		{
			m_Playing = false;
			if (m_ParticleObj == null)
			{
				return;
			}

			if (ParticleSystems != null && ParticleSystems.Length > 0)
			{
				for (Int32 i = ParticleSystems.Length - 1; i >= 0; i--)
				{
					if (ParticleSystems[i] == null)
					{
						continue;
					}

					ParticleSystems[i].Stop();
				}
			}

		}
		public void SetRotation(Quaternion p_rotation)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.rotation = p_rotation;
		}

		public void SetLocalRotation(Quaternion p_rotation)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.localRotation = p_rotation;
		}

		public void SetParent(Transform p_root)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.SetParent(p_root);
		}
		public void SetPostion(Vector3 pos)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.position = pos;
		}

		public void SetLocalPostion(Vector3 pos)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.localPosition = pos;
		}

		public void SetScale(Vector3 scale)
		{
			if (rootTran == null)
			{
				return;
			}

			rootTran.localScale = scale;
		}

		public void SetLocalRotation(List<int> rotateInfo)
		{
			if (rootTran == null || rotateInfo.Count != 3)
			{
				return;
			}

			rootTran.localRotation = Quaternion.Euler(rotateInfo[0], rotateInfo[1], rotateInfo[2]);
		}

		public Vector3 GetScale()
		{
			if (rootTran == null)
			{
				return Vector3.one;
			}

			return rootTran.localScale;
		}

		public void LookAtTarget(Vector3 worldPosTarget, bool onlyParent)
		{
			if (m_ParticleObj != null && m_ParticleObj.transform != null)
			{
				m_ParticleObj.transform.LookAt(worldPosTarget);
			}
		}

		public void UpToTarget(Vector3 vec)
		{
			if (m_ParticleObj != null && m_ParticleObj.transform != null)
			{
				vec.z = 0;
				m_ParticleObj.transform.up = vec;
			}
		}

		public void SetTarget(Vector3 target)
		{

		}

		void OnDestroy()
		{
			Clear();
		}

		internal void Clear(bool recycleObj = true)
		{
			m_AutoRemove = false;
			m_RemoveTime = 0;

			m_needDelay = false;
			m_delayTime = 0;
			mParticleCompleteFun = null;

			if (!ParticleResMgr.s_playing)
			{
				return;
			}
			if (particleInfo != null)
			{
				particleInfo.Reset();
			}
			Stop();
			OnParticleLoadedComplete = null;
			// 删除的时候需要从计数管理器中将实例清除;
			ParticleManager.GetInstance().RemoveFromParticleCounter(this);

			if (recycleObj && m_ParticleObj != null)
			{
				ParticleResMgr.GetInstance().AddCache(m_ParticleKey, m_ParticleObj);

			}
			m_ParticleObj = null;
			m_Loaded = false;
			mParticleSystem = null;
			m_MainParticleSystem = null;

			m_ParticleKey = "";
			m_Layer = 0;
			m_UseFake = false;
			m_Playing = false;
			m_Failed = false;


			m_UseLogicDuration = false;
			m_Duration = 0;
			m_IsLoop = false;
			particleInfo = null;
			if (mClearCompleteFun != null)
			{
				mClearCompleteFun();
				mClearCompleteFun = null;
			}

		}

		/// <summary>
		/// 加载资源，内部触发，不交由外部调用;
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="useFake">是否使用假的;</param>
		internal void LoadRes(string key, bool useFake, int layer = 0)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}
			m_ParticleKey = key;
			m_Layer = layer;
			if (root == null)
			{
				return;
			}

			root.name = key;
			m_UseFake = useFake;

			if (useFake)
			{
				m_Loaded = true;
				return;
			}

			mParticleCompleteFun = OnParticleLoaded;
			ParticleResMgr.GetInstance().LoadRes(key, mParticleCompleteFun);
		}

		// 资源加载完毕;
		protected bool OnParticleLoaded(string key, GameObject gObj)
		{
			// 如果key为空，或者key和Particle的key不同，则直接将对象还给资源缓存池;
			if (string.IsNullOrEmpty(key) || !string.Equals(key, m_ParticleKey) || !gameObject.activeSelf)
			{
				ParticleResMgr.GetInstance().AddCache(key, gObj);
				return false;
			}

			m_Loaded = true;

			// 如果加载失败，则消除;
			if (!m_UseFake && gObj == null)
			{
				m_Failed = true;
				ParticleManager.GetInstance().ChangeCounterToFake(this);
				return false;
			}

			if (m_UseFake || gObj == null || rootTran == null)
			{
				return false;
			}



			m_ParticleObj = gObj;
			particleInfo = m_ParticleObj.GetComponent<ParticleInfo>();
			FxSetting qua = m_ParticleObj.GetComponent<FxSetting>();
			if (qua != null)
			{
				int level = 1;
				qua.ApplyFx(level);
			}
			Transform particleTran = m_ParticleObj.transform;

			particleTran.parent = rootTran;
			particleTran.localPosition = Vector3.zero;
			particleTran.localScale = Vector3.one;
			particleTran.localEulerAngles = Vector3.zero;
			ObjectCommon.SetObjectAndChildrenLayer(particleTran.gameObject, m_Layer);
			m_MainParticleSystem = m_ParticleObj.GetComponent<ParticleSystem>();
			if (m_Playing)
			{
				Play();
			}
			if (OnParticleLoadedComplete != null)
			{
				OnParticleLoadedComplete(this);
				OnParticleLoadedComplete = null;
			}
			return true;
		}

		/// <summary>
		///  仅限模型加载完毕
		/// </summary>
		/// <param name="callBack"></param>
		/// <returns></returns>
		public bool AddLoadedCallBack(ParticleLoadedComplete callBack)
		{
			if (callBack == null)
			{
				return false;
			}

			OnParticleLoadedComplete = callBack;
			LoadsuccessCallBack();

			return true;
		}

		protected bool LoadsuccessCallBack()
		{
			if (m_Loaded == true)
			{
				if (OnParticleLoadedComplete != null)
				{
					OnParticleLoadedComplete(this);
				}
				OnParticleLoadedComplete = null;
			}

			return true;
		}

		void Update()
		{
			if (m_AutoRemove)
			{
				if (Time.time > m_RemoveTime)
				{
					m_AutoRemove = false;
					ParticleManager.GetInstance().AddCache(this);
				}
			}
			if (m_needDelay)
			{
				if (Time.time > m_delayTime)
				{
					m_needDelay = false;
					if (particleInfo != null)
					{
						particleInfo.DoAction();
					}
				}
			}
		}

		public bool IsEnd()
		{
			if (m_UseLogicDuration && m_IsLoop)
			{
				return false;
			}

			if (Time.time > m_StartTime + Duration)
			{
				return true;
			}

			return false;
		}

		public void SetDuration(bool loop, float duration)
		{
			m_UseLogicDuration = true;
			m_Duration = duration;
			m_IsLoop = loop;
		}

		public float GetDelayDestroyTime()
		{
			if (particleInfo == null)
			{
				return 0;
			}
			else
			{
				return particleInfo.m_DelayOpen;
			}
		}

		public float GetDelayDuration()
		{
			return 1;
			/*if (particleInfo == null)
			{
				return Match3Config.Instance.effect_delay_destroy_time;
			}
			else
			{
				if (particleInfo.duration == 0)
				{
					return Match3Config.Instance.effect_delay_destroy_time;
				}
				else
				{
					return particleInfo.duration;
				}
			}*/
		}
	}
}