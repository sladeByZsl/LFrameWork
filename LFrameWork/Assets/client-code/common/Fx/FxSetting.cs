
using UnityEngine;
using System;
using System.Collections.Generic;

public class FxSetting : MonoBehaviour
{
	public enum FxLevel
	{
		Start = -1,

		Low = 0,
		Med = 1,
		High = 2,

		End,
	}
	[System.Serializable]
	public class FxConf
	{
		public GameObject gameobject;
		public Int32 fxlevel;

		public bool IsPlay(Int32 level)
		{
			return (fxlevel & (1 << level)) != 0;
		}

#if UNITY_EDITOR
		public void SetLevel(Int32 level, bool isPlay, bool autoSetting = true)
		{
			if (level < 0)
			{
				return;
			}

			Int32 deltaValue = 1 << level;
			if (isPlay == true)
			{
				if (IsPlay(level) == false)
				{
					fxlevel |= deltaValue;
				}
			}
			else
			{
				if (IsPlay(level) == true)
				{
					fxlevel -= deltaValue;
				}
			}


			// 额外的自动操作，不需要可隐藏;
			if (!autoSetting)
			{
				return;
			}

			if (!isPlay)
			{
				for (int i = 0; i < level; i++)
				{
					SetLevel(i, isPlay, false);
				}
			}
			else
			{
				for (int i = level + 1; i < (int)FxLevel.End; i++)
				{
					SetLevel(i, isPlay, false);
				}
			}
		}
#endif
	}

	[HideInInspector] [SerializeField] List<FxConf> mFxConfList = new List<FxConf>();

	public void ApplyFx(Int32 level)
	{
		if (mFxConfList == null || mFxConfList.Count < 1)
		{
			return;
		}

		for (int i = 0; i < mFxConfList.Count; i++)
		{
			FxConf fxConf = mFxConfList[i];
			if (fxConf == null || fxConf.gameobject == null)
			{
				continue;
			}

			if (fxConf.IsPlay(level))
			{
				fxConf.gameobject.SetActive(true);
			}
			else
			{
				fxConf.gameobject.SetActive(false);
			}
		}

	}

	public void ApplyFx2Low(Int32 level)
	{
		if (mFxConfList == null || mFxConfList.Count < 1)
		{
			return;
		}

		for (int i = 0; i < mFxConfList.Count; i++)
		{
			FxConf fxConf = mFxConfList[i];
			if (fxConf == null || fxConf.gameobject == null)
			{
				continue;
			}

			if (fxConf.IsPlay(level))
			{
				fxConf.gameobject.SetActive(true);
			}
			else
			{
				GameObject.Destroy(fxConf.gameobject);
				fxConf.gameobject = null;
			}
		}
	}

#if UNITY_EDITOR
	public List<FxConf> fxConfList { get { return mFxConfList; } }

	public bool Contains(GameObject gObj)
	{
		if (gObj == null)
		{
			return false;
		}

		for (int i = 0; i < mFxConfList.Count; i++)
		{
			FxConf fxConf = mFxConfList[i];
			if (fxConf == null || fxConf.gameobject == null)
			{
				return false;
			}

			if (fxConf.gameobject == gObj)
			{
				return true;
			}
		}

		return false;
	}

	public FxConf GetFxConf(GameObject gObj)
	{
		if (gObj == null)
		{
			return CreateFxConf(gObj);
		}

		for (int i = 0; i < mFxConfList.Count; i++)
		{
			FxConf fxConf = mFxConfList[i];
			if (fxConf == null)
			{
				continue;
			}

			if (fxConf.gameobject == gameObject)
			{
				return fxConf;
			}
		}

		return CreateFxConf(gObj);
	}

	public static FxConf CreateFxConf(GameObject gObj)
	{
		FxConf fxConf = new FxConf();
		fxConf.gameobject = gObj;
		fxConf.SetLevel(0, true);

		return fxConf;
	}
#endif
}