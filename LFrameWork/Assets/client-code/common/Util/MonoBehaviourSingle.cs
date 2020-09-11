
using UnityEngine;

namespace LFrameWork.Common.Utility
{
	public abstract class MonoBehaviourSingle<T> : MonoBehaviour where T : MonoBehaviour
	{
		protected static T s_instance;

		protected Transform m_Tran;
		protected static bool _applicationIsQuitting = false;
		protected Transform tran
		{
			get
			{
				if (m_Tran == null)
				{
					m_Tran = gameObject.transform;
				}

				return m_Tran;
			}
		}

		public static T GetInstance()
		{
			if (s_instance == null && !_applicationIsQuitting)
			{
				s_instance = FindObjectOfType<T>();
				if (s_instance == null)
				{
					GameObject gObj = new GameObject(typeof(T).Name);
					GameObject.DontDestroyOnLoad(gObj);

					Transform tran = gObj.transform;
					tran.parent = null;
					tran.localPosition = Vector3.zero;
					tran.localEulerAngles = Vector3.zero;
					tran.localScale = Vector3.one;

					s_instance = gObj.AddComponent<T>();
				}
			}
			return s_instance;
		}

		public static T instance
		{
			get
			{
				return GetInstance();
			}
		}

		private void Awake()
		{
			OnInit();
		}

		private void Update()
		{
			OnUpdate(Time.deltaTime);
		}

		protected virtual void OnApplicationQuit()
		{
			_applicationIsQuitting = true;
			if (s_instance == null) return;
			Destroy(s_instance.gameObject);
			s_instance = null;
		}
		private void OnDestroy()
		{
            OnClear();
        }
        public virtual void OnInit() { }
		public virtual void OnUpdate(float deltaTime) { }
		public virtual void OnReConnect() { }
		public virtual void OnClear() { }
	}
}