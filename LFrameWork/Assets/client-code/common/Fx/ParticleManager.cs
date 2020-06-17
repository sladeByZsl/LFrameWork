using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LFrameWork.Common.Utility;

namespace LFrameWork.Common
{
    public class ParticleManager : MonoBehaviourSingle<ParticleManager>
    {
        public enum EParticleState
        {
            State_Real,     // 有资源的特效;
            State_Fake,     // 无资源的假特效;
            State_Failed,     // 加载失败的特效;
            State_All,          // 全部特效;
        }

        // 激活的特效;
        CommonCounter<EParticleType, ParticleInstance> m_ParticleCounter = new CommonCounter<EParticleType, ParticleInstance>();
        // 隐藏的特效;
        CommonCounter<EParticleType, ParticleInstance> m_ParticleFakeCounter = new CommonCounter<EParticleType, ParticleInstance>();
        // 加载失败的特效;
        CommonCounter<EParticleType, ParticleInstance> m_ParticleFailedCounter = new CommonCounter<EParticleType, ParticleInstance>();

        protected MonoBehaviourPool<ParticleInstance> mParticlePool = new MonoBehaviourPool<ParticleInstance>();

        protected Transform m_ActiveParticleRootTran;

        public int FakeNum = 50;


        protected Transform activeRootTran
        {
            get
            {
                if (m_ActiveParticleRootTran == null)
                {
                    GameObject gObj = new GameObject("Fx_Active");
                    gObj.transform.localPosition = new Vector3(-10000, -10000, -10000);
                    GameObject.DontDestroyOnLoad(gObj);
                    m_ActiveParticleRootTran = gObj.transform;
                }
                return m_ActiveParticleRootTran;
            }
        }

#if !UNITY_EDITOR
        protected override void OnInit()
        {
            //throw new NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            //throw new NotImplementedException();
        }

        public override void OnReConnect()
        {
            //throw new NotImplementedException();
        }

        public override void Clear()
        {
            //throw new NotImplementedException();
        }
#endif
        protected override void OnApplicationQuit()
        {
            //Debug.LogError("particle clearall");
            mParticlePool.Clear();
            ClearAll();
            base.OnApplicationQuit();
        }

        public void Awake()
        {
            
        }
       

        /// <summary>
        /// 将不用的特效还回缓存池;
        /// </summary>
        /// <param name="parInstance"></param>
        public void AddCache(ParticleInstance parInstance)
        {
            if (parInstance == null)
            {
                return;
            }

            // 特效实例删除，意味着清除所有的资源;
            parInstance.Clear();

            if (mParticlePool == null)
            {
                GameObject.Destroy(parInstance.gameObject);
                return;
            }

            mParticlePool.AddToCache(parInstance);
        }

        public void AddCache(ParticleInstance parInstance, float durtion, bool unFollow = true)
        {
            if (parInstance == null)
            {
                return;
            }

            parInstance.SetAutoRemove(durtion, unFollow);
        }

        public void AddCache(ParticleInstance parInstance, float durtion, float destroyTime, bool unFollow = true, Action removeCompleteCallback = null)
        {
            if (parInstance == null)
            {
                return;
            }

            parInstance.SetAutoRemove(durtion, destroyTime, unFollow, removeCompleteCallback);
        }

        public void ClearAll()
        {
            for (int i = (int)EParticleType.Unknown; i <= (int)EParticleType.Max; i++)
            {
                ClearType((EParticleType)i);
            }
            // 
        }

        public void ClearType(EParticleType type)
        {
            List<ParticleInstance> parList = GetParticleList(type);
            if (parList == null || parList.Count < 1)
            {
                return;
            }

            for (int i = 0; i < parList.Count; i++)
            {
                AddCache(parList[i]);
            }
        }

        public ParticleInstance GetParticleInstance(string name, EParticleType type, Vector3 pos)
        {
            ParticleInstance pi = GetParticleInstance(name, type);
            if (pi != null)
            {
                pi.SetPostion(pos);
            }

            return pi;
        }

        /// <summary>
        /// 获取一个特效实例;
        /// </summary>
        /// <param name="name">特效名称</param>
        /// <param name="type">特效类型</param>
        /// <param name="gameObjectLayer">特效的层</param>
        /// <returns>特效实例;</returns>
        public ParticleInstance GetParticleInstance(string name, EParticleType type, int gameObjectLayer = 0)
        {
            if (mParticlePool == null || string.IsNullOrEmpty(name))
            {
                return null;
            }

            ParticleInstance pi = mParticlePool.GetMonoBehaviour();
            if (pi == null)
            {
                return null;
            }

            pi.gameObject.transform.parent = activeRootTran;
            pi.gameObject.transform.localPosition = Vector3.zero;
            pi.gameObject.SetActive(true);
            pi.gameObject.name = name;
            pi.particleType = type;

            bool useFake = hasFake(type);
            pi.LoadRes(name, useFake, gameObjectLayer);

            if (pi.loadFailed)
            {
                m_ParticleFailedCounter.AddValue(type, pi);
            }
            else
            {
                if (pi.useFake)
                {
                    m_ParticleFakeCounter.AddValue(type, pi);
                }
                else
                {
                    m_ParticleCounter.AddValue(type, pi);
                }
            }

            return pi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Int32 GetParticleCount(EParticleType type, EParticleState state)
        {
            switch (state)
            {
                case EParticleState.State_Real:
                    return m_ParticleCounter.GetTypeCount(type);

                case EParticleState.State_Fake:
                    return m_ParticleFakeCounter.GetTypeCount(type);

                case EParticleState.State_Failed:
                    return m_ParticleFailedCounter.GetTypeCount(type);

                case EParticleState.State_All:
                    return m_ParticleCounter.GetTypeCount(type) + m_ParticleFakeCounter.GetTypeCount(type) + m_ParticleFailedCounter.GetTypeCount(type);
            }

            return 0;
        }

        public List<ParticleInstance> GetParticleList(EParticleType type, EParticleState state)
        {
            switch (state)
            {
                case EParticleState.State_Real:
                    return m_ParticleCounter.GetList(type);

                case EParticleState.State_Fake:
                    return m_ParticleFakeCounter.GetList(type);

                case EParticleState.State_Failed:
                    return m_ParticleFailedCounter.GetList(type);
            }

            return null;
        }

        internal void RemoveFromParticleCounter(ParticleInstance parInstance)
        {
            if (parInstance == null)
            {
                return;
            }

            if (parInstance.loadFailed)
            {
                m_ParticleFailedCounter.RemoveValue(parInstance.particleType, parInstance);
                return;
            }

            if (parInstance.useFake)
            {
                m_ParticleFakeCounter.RemoveValue(parInstance.particleType, parInstance);
            }
            else
            {
                m_ParticleCounter.RemoveValue(parInstance.particleType, parInstance);
            }
        }

        internal void ChangeCounterToFake(ParticleInstance parInstance)
        {
            if (parInstance == null)
            {
                return;
            }

            m_ParticleFailedCounter.AddValue(parInstance.particleType, parInstance);

            m_ParticleCounter.RemoveValue(parInstance.particleType, parInstance);
            m_ParticleFakeCounter.RemoveValue(parInstance.particleType, parInstance);
        }

        internal bool hasFake(EParticleType type)
        {
            // 写了个假的用于测试功能;
            if (m_ParticleCounter.GetTypeCount(type) >= FakeNum)
            {
                return true;
            }

            return false;
        }

        internal List<ParticleInstance> GetParticleList(EParticleType type)
        {
            List<ParticleInstance> parList = new List<ParticleInstance>();

            // 激活的特效;			
            parList.CopyFrom(m_ParticleCounter.GetList(type));

            // 隐藏的特效;
            parList.CopyFrom(m_ParticleFakeCounter.GetList(type));

            // 加载失败的特效;
            parList.CopyFrom(m_ParticleFailedCounter.GetList(type));

            return parList;
        }


        void Update()
        {
            mParticlePool.Update();
        }
    }
}