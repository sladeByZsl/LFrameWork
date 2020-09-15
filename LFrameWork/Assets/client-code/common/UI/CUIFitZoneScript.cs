//==================================================================================
/// UI适配区域
/// @用于将宽高比超过2:1的机型的UI通过加上左右bar条的方式将实际可用部分的比例限制为2：1
//==================================================================================

using UnityEngine;

public enum enFitZoneType
{
    Left,
    Right,
    Center
};

public class CUIFitZoneScript : MonoBehaviour
{
    //FitZone类型
    public enFitZoneType m_fitZoneType = enFitZoneType.Left;

    //目标宽高比
    private const float c_targetWithHeightRate = 2.0f;

    //需要进行缩减修正的宽度(按照form的参考尺寸)
    private float m_fixedWidth = 0;

    //Transform
    private RectTransform m_rectTransform;

    //检测Screen是否发生变化
    private float m_screenWidth;
    private float m_screenHeight;

    private float designWidth = 1080;
    private float designHeight = 1920;

    //--------------------------------------
    /// 初始化
    //--------------------------------------
    void Awake()
    {
        m_rectTransform = this.gameObject.transform as RectTransform;
        //适配
        Fit(Screen.width, Screen.height);
    }

    //--------------------------------------
    /// 清理
    //--------------------------------------
    //public override void UnInitialize()
    //{
    //    if (m_isInitialized)
    //    {
    //        base.UnInitialize();

    //        m_isInitialized = false;
    //    }
    //}

    //--------------------------------------
    /// 销毁
    //--------------------------------------
    void OnDestroy()
    {

    }

    //--------------------------------------
    /// Update
    //--------------------------------------
    void Update()
    {
        //屏幕尺寸有发生变化，适配一下
        if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
        {
            Fit(Screen.width, Screen.height);
        }
    }

    //--------------------------------------
    /// 计算需要修正（缩减）的宽度
    //--------------------------------------
    float CalculateFixedWidth(float screenWidth, float screenHeight)
    {
        if (screenWidth > screenHeight * 2)
        {
            m_fixedWidth = (screenWidth * designHeight / screenHeight) - (designWidth * 2);

            if (m_fixedWidth < 0)
            {
                m_fixedWidth = 0;
            }
        }
        else
        {
            m_fixedWidth = 0;
        }

        return m_fixedWidth;
    }

    //--------------------------------------
    /// 适配
    //--------------------------------------
    private void Fit(float screenWidth, float screenHeight)
    {
        CalculateFixedWidth(screenWidth, screenHeight);

        switch (m_fitZoneType)
        {
            case enFitZoneType.Left:
                {
                    m_rectTransform.pivot = new Vector2(0, 0.5f);
                    m_rectTransform.anchorMin = Vector2.zero;
                    m_rectTransform.anchorMax = new Vector2(0, 1);

                    m_rectTransform.offsetMin = new Vector2(0, 0);
                    m_rectTransform.offsetMax = new Vector2(m_fixedWidth / 2, 0);
                }
                break;

            case enFitZoneType.Right:
                {
                    m_rectTransform.pivot = new Vector2(1, 0.5f);
                    m_rectTransform.anchorMin = new Vector2(1, 0);
                    m_rectTransform.anchorMax = new Vector2(1, 1);

                    m_rectTransform.offsetMin = new Vector2(-m_fixedWidth / 2, 0);
                    m_rectTransform.offsetMax = new Vector2(0, 0);
                }
                break;

            case enFitZoneType.Center:
                {
                    m_rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    m_rectTransform.anchorMin = Vector2.zero;
                    m_rectTransform.anchorMax = new Vector2(1, 1);

                    m_rectTransform.offsetMin = new Vector2(m_fixedWidth / 2, 0);
                    m_rectTransform.offsetMax = new Vector2(-m_fixedWidth / 2, 0);
                }
                break;

            default:
                break;
        }

        //记录当前的ScreenWidth和ScreenHeight
        m_screenWidth = screenWidth;
        m_screenHeight = screenHeight;

        //小于2:1的时候要隐藏节点
        //if(m_fitZoneType != enFitZoneType.Center)
        //{
        if (m_fixedWidth <= 0)
        {
            ActiveChilds(this.transform, false);
        }
        else
        {
            ActiveChilds(this.transform, true);
        }
        // }
    }

    public static void ActiveChilds(Transform fatherRoot, bool isActive)
    {
        if (fatherRoot == null)
        {
            return;
        }

        for (int i = 0; i < fatherRoot.childCount; i++)
        {
            fatherRoot.GetChild(i).gameObject.SetActive(isActive);
        }
    }
}
