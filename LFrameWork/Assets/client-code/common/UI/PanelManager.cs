using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using libx;
using LFrameWork.Common.Utility;

public class PanelManager : MonoBehaviourSingle<PanelManager>
{
    public Camera mUICamera;
    private Transform[] uiRoots = null;
    private Transform parent;
    private Dictionary<string, GameObject> panelInfo;
    protected Transform Parent
    {
        get
        {
            if (parent == null)
            {
                GameObject go = GameObject.Find("Canvas");
                if (go != null) parent = go.transform;
            }
            return parent;
        }
    }

    public override void OnInit()
    {
        mUICamera = GameObject.Find("GuiCamera").GetComponent<Camera>();
        panelInfo = new Dictionary<string, GameObject>();
    }

    private Transform GetUIRoot(int canvas_type)
    {
        if (uiRoots == null)
        {
            uiRoots = new Transform[4];
        }
        if (uiRoots[canvas_type] == null)
        {
            GameObject go = GameObject.Find("Canvas" + canvas_type);
            if (go == null)
            {
                go = new GameObject();
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
            }
            if (go != null) uiRoots[canvas_type] = go.transform;
        }
        return uiRoots[canvas_type];
    }


    public void ShowPanel(string panelName, bool isShow, Action callBack=null)
    {
        if (string.IsNullOrEmpty(panelName))
        {
            return;
        }
        if (isShow)
        {
            GameObject panelObj = null;
            if (panelInfo.TryGetValue(panelName, out panelObj))
            {
                panelObj.SetActive(true);
                callBack?.Invoke();
            }
            else
            {
                string assetPath = "Assets/Arts/ui/panel/" + panelName+ ".prefab";
                Assets.LoadAssetAsync(assetPath, typeof(UnityEngine.Object)).completed += delegate (AssetRequest request)
                {
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        Debug.LogError(request.error);
                        return;
                    }
                    GameObject go = (GameObject)Instantiate(request.asset);
                    go.name = request.asset.name;
                    go.transform.parent = GetUIRoot(0);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    request.Require(go);
                    request.Release();

                    panelInfo[panelName] = go;
                    callBack?.Invoke();
                };
            }
        }
        else
        {
            GameObject panelObj = null;
            if (panelInfo.TryGetValue(panelName, out panelObj))
            {
                panelObj.SetActive(false);
                callBack?.Invoke();
            }
        }
    }

    public void DestroyPanel(string panelName)
    {
        GameObject panelObj = null;
        if (panelInfo.TryGetValue(panelName, out panelObj))
        {
            Destroy(panelObj);
            panelInfo.Remove(panelName);
        }
    }

    public void DestroyAllPanel()
    {
        foreach(KeyValuePair<string,GameObject> item in panelInfo)
        {
            Destroy(item.Value);
        }
        panelInfo.Clear();
    }

    public override void OnClear()
    {
        
    }



}
