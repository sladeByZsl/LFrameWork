
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonUtils
{
	/// <summary>
	/// 将一个List拷贝入另一个List;
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="dest">目标</param>
	/// <param name="src">源数据</param>
	/// <param name="autoUniqueness">自动去除重复</param>
	public static void CopyFrom<T>(this List<T> dest, List<T> src, bool autoUniqueness = true)
	{
		if (src == null || dest == null || src.Count < 1)
		{
			return;
		}

		for (int i = 0; i < src.Count; i++)
		{
			T t = src[i];
			if (autoUniqueness && dest.Contains(t))
			{
				continue;
			}

			dest.Add(t);
		}
	}

	public static Int32 GetMask(string str)
	{
		Int32 mask = 0;
		char[] charArray = str.ToCharArray();
		Int32 size = charArray.Length;
		for (Int32 i = 0; i < size; ++i)
		{
			if (charArray[size - i - 1] == '1')
			{
				mask |= (1 << i);
			}
		}
		return mask;
	}

	/*--------------------------------------------------------*/
	// 是否按下;
	/*--------------------------------------------------------*/
	public static bool IsPressed()
	{
		if (Input.GetMouseButton(0))
		{
			return true;
		}
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				return true;
			}
		}
		return false;
	}
}