
/**************************************************************************************************
	Copyright (C) 2017 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：FPSInfo.cs;
	作	者：jiabin;
	时	间：2017 - 05 - 31;
	注	释：;
**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSInfo : MonoBehaviour
{
	public Text m_fpsLabel;

	public float fpsMeasuringDelta = 2.0f;
	private float timePassed;
	private int m_FrameCount = 0;
	private float m_FPS = 0.0f;
	void Awake()
	{
        //Application.targetFrameRate = 60;
#if !Debug
        this.gameObject.SetActive(false);
#else
        this.gameObject.SetActive(true);
#endif
    }
    // Update is called once per frame
    void Update()
	{
		m_FrameCount = m_FrameCount + 1;
		timePassed = timePassed + Time.deltaTime;

		if (timePassed > fpsMeasuringDelta)
		{
			m_FPS = m_FrameCount / timePassed;

			timePassed = 0.0f;
			m_FrameCount = 0;

			if (m_fpsLabel != null)
			{
				m_fpsLabel.text = m_FPS.ToString("0");
            }
		}
	}
}