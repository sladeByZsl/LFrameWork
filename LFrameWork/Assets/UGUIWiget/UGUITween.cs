using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUITween : MonoBehaviour
{
    public Canvas canvas;
    //image物体
    private RectTransform m_rectTransform;
    //image父物体
    RectTransform m_parent;

    void Start () {
        m_rectTransform = this.transform as RectTransform;
        m_parent = this.transform.parent as RectTransform;
    }

    /*
     * RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                            RectTransform rect,
                                                            Vector2 screenPoint,
                                                            Camera cam,
                                                            out Vector2 localPoint);
        参数 rect：
        这个参数需要你提供一个父物体的RectTransform。因为这个方法是取得UI坐标，而UI坐标都是局部坐标，所以一定需要一个父物体，才能计算出局部坐标。(有父物体才有局部坐标对吧！)
        最后，这个方法就会把屏幕上的点转化为这个父物体下的局部坐标。
        参数 screenPoint：
        这个参数需要你提供一个屏幕空间 的坐标 (屏幕坐标)。
        最后，这个方法会把这个屏幕坐标 转化为ui坐标。
        参数 cam：
        这个参数需要你指定一个相机。
        因为UI坐标是根据相机来确定的。(如果Canvas是Screen Space-overlay模式，cam参数应为null)
        参数 localPoint：
        这个参数需要你提供一个装"返回值"的容器给方法。(如果不能理解高级参数out，可以在这里了解下：https://www.cnblogs.com/linfenghp/p/6618580.html)
        最后，这个方法会把屏幕点的UI坐标的结果，装到这个变量中。
        你也可以这样理解：你给指定一个Vector2变量给这个参数，这个方法最后会把转换好的UI坐标赋值给你指定的这个变量。
        返回值 - bool类型：
        这个方法有一个返回值，但是我自己没使用过这个返回值（感觉用处不大？）。
        官方的说法是：这个返回值是，判断此点是否在Rect所在的平面上。
        如果在，就返回true。
     */

    void Update () {
        if (Input.GetMouseButton (0)) {
            Vector2 _parentLocalPosWithScreenPos = Vector2.zero;
            //将屏幕空间点转换为位于矩形平面上的RectTransform的局部空间中的位置。
            //cam参数应该是与屏幕点相关联的摄像机。
            //对于画布中的RectTransform设置为Screen Space - Overlay模式，cam参数应该为null。
            //当从提供PointerEventData对象的事件处理程序中使用ScreenPointToLocalPointInRectangle时，
            //可以通过使用PointerEventData获得正确的摄像机。
            //enterEventData(用于悬停功能)或PointerEventData。
            //pressEventCamera(用于单击功能)。这将自动为给定的事件使用正确的相机(或null)。
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
                    m_parent,
                    Input.mousePosition,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay?null : canvas.worldCamera,
                    out _parentLocalPosWithScreenPos)) {

                //_parentLocalPosWithScreenPos为targetRectTransform的局部坐标
                Rect _parentRect = m_parent.rect;
                _parentLocalPosWithScreenPos.x = Mathf.Clamp (
                    _parentLocalPosWithScreenPos.x,
                    //_parent的枢轴点在中心
                    -_parentRect.width * 0.5f + m_rectTransform.rect.width * 0.5f,
                    _parentRect.width * 0.5f - m_rectTransform.rect.width * 0.5f
                );
                _parentLocalPosWithScreenPos.y = Mathf.Clamp (
                    _parentLocalPosWithScreenPos.y,
                    -_parentRect.height * 0.5f + m_rectTransform.rect.height * 0.5f,
                    _parentRect.height * 0.5f - m_rectTransform.rect.height * 0.5f
                );
                //运动
                m_rectTransform.anchoredPosition = Vector2.Lerp (m_rectTransform.anchoredPosition, _parentLocalPosWithScreenPos, .5f);

            }

        }
    }
}
