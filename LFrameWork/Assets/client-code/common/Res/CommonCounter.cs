using UnityEngine;
using System;
using System.Collections.Generic;

namespace LFrameWork.Common
{
	public class CommonCounter<TType, TValue> where TValue : class
	{
		protected Dictionary<TType, List<TValue>> mPool = new Dictionary<TType, List<TValue>>();
		protected Int32 m_MaxCount = Int32.MaxValue;

		public Int32 maxCount
		{
			set
			{
				m_MaxCount = value;
			}
		}

		// 往计数器里添加一个引用;
		public Int32 AddValue(TType type, TValue value, bool autoDestory = false, bool autoDelRepeat = true)
		{
			if (value == null)
			{
				return 0;
			}

			List<TValue> valueList = GetList(type, true);
			if (valueList == null)
			{
				return 0;
			}

			if (autoDelRepeat)
			{
				if (valueList.Contains(value))
				{
					return valueList.Count;
				}
			}


			if (valueList.Count >= m_MaxCount && autoDestory && value is UnityEngine.Object)
			{
				UnityEngine.Object.Destroy(value as UnityEngine.Object);
				return valueList.Count;
			}

			valueList.Add(value);

			return valueList.Count;
		}

		public List<TType> GetAllTypes()
		{
			List<TType> types = new List<TType>();
			foreach (KeyValuePair<TType, List<TValue>> iter in mPool)
			{
				types.Add(iter.Key);
			}

			return types;
		}

		public List<TValue> GetList(TType type, bool autoAdd = false)
		{
			List<TValue> valueList = null;
			if (mPool.TryGetValue(type, out valueList))
			{
				return valueList;
			}

			if (!autoAdd)
			{
				return null;
			}

			valueList = new List<TValue>();
			mPool.Add(type, valueList);

			return valueList;
		}

		public Int32 GetTotalCount()
		{
			Int32 totalCount = 0;
			foreach (KeyValuePair<TType, List<TValue>> iter in mPool)
			{
				totalCount += iter.Value.Count;
			}
			return totalCount;
		}

		public Int32 GetTypeCount(TType type)
		{
			List<TValue> valueList = GetList(type);
			if (valueList == null)
			{
				return 0;
			}

			return valueList.Count;
		}

		public TValue PopupValue(TType type)
		{
			List<TValue> valueList = GetList(type);
			if (valueList == null || valueList.Count < 1)
			{
				return null;
			}

			int index = valueList.Count - 1;
			TValue value = valueList[index];
			valueList.RemoveAt(index);

			if (valueList.Count <= 0)
			{
				RemoveType(type);
			}

			return value;
		}

		public void RemoveType(TType type)
		{
			if (!mPool.ContainsKey(type))
			{
				return;
			}

			mPool.Remove(type);
		}

		public Int32 RemoveValue(TType type, TValue value)
		{
			if (value == null)
			{
				return 0;
			}

			List<TValue> valueList = GetList(type);
			if (valueList == null)
			{
				return 0;
			}

			if (valueList.Contains(value))
			{
				valueList.Remove(value);
			}

			if (valueList.Count <= 0)
			{
				RemoveType(type);
				return 0;
			}

			return valueList.Count;
		}
	}
}