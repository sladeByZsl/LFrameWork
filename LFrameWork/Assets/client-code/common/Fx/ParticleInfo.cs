using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInfo : MonoBehaviour
{
    [Header("是否动态销毁,如果为勾选,不用填销毁时间")]
    public bool IsDynamic = false;
    [Header("销毁时间")]
    public float duration = 2.0f;

    [Header("需要停止的节点列表")]
    public List<GameObject> m_StopList = new List<GameObject>();
    [Header("需要开启的节点列表")]
    public List<GameObject> m_OpenList = new List<GameObject>();

    [Header("延迟时间")]
    public float m_DelayOpen = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoAction()
    {
        if (m_StopList != null && m_StopList.Count > 0)
        {
            for (int i = 0; i < m_StopList.Count; i++)
            {
                if (m_StopList[i] != null)
                {
                    m_StopList[i].SetActive(false);
                }
            }
        }

        if (m_OpenList != null && m_OpenList.Count > 0)
        {
            for (int i = 0; i < m_OpenList.Count; i++)
            {
                if (m_OpenList[i] != null)
                {
                    m_OpenList[i].SetActive(true);
                }
            }
        }
    }

    public void Reset()
    {
        if (m_StopList != null && m_StopList.Count > 0)
        {
            for (int i = 0; i < m_StopList.Count; i++)
            {
                if (m_StopList[i] != null)
                {
                    m_StopList[i].SetActive(true);
                }
            }
        }

        if (m_OpenList != null && m_OpenList.Count > 0)
        {
            for (int i = 0; i < m_OpenList.Count; i++)
            {
                if (m_OpenList[i] != null)
                {
                    m_OpenList[i].SetActive(false);
                }
            }
        }
    }
}
