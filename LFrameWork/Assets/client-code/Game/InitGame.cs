using System.Collections;
using System.Collections.Generic;
using LFrameWork.Common;
using UnityEngine;
using LFrameWork.Common.Resource;
using libx;

public class InitGame : MonoBehaviour
{
    [SerializeField] private bool development;
    public Transform root;
    ParticleInstance particle;

    // Start is called before the first frame update
    void Start()
    {
        if (development)
        {
            Assets.runtimeMode = false;
        }
        else
        {
            Assets.runtimeMode = true;
        }
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        /// 初始化
        var init = Assets.Initialize();
        yield return init;
        if (string.IsNullOrEmpty(init.error))
        {
            Debug.LogError("初始化成功");
            init.Release();
            Init();
        }
        else
        {
            Debug.LogError("初始化失败");
        }
    }

    private void Init()
    {
      
        //var assetPath = "Assets/Arts/ui/panel/StartPanel.prefab";
        //Assets.LoadAssetAsync(assetPath, typeof(UnityEngine.Object)).completed += delegate (AssetRequest request)
        //{
        //    if (!string.IsNullOrEmpty(request.error))
        //    {
        //        Debug.LogError(request.error);
        //        return;
        //    }
        //    GameObject go = (GameObject)Instantiate(request.asset);
        //    go.name = request.asset.name;
        //    go.transform.parent = root;
        //    go.transform.localPosition = Vector3.zero;
        //    go.transform.localScale = Vector3.one;
        //    /// 设置关注对象，当关注对象销毁时，回收资源
        //    request.Require(go);
        //    //Destroy(go, 3);
        //    /// 设置关注对象后，只需要释放一次 
        //    /// 这里如果之前没有调用 Require，下一帧这个资源就会被回收
        //    request.Release();
        //};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"add particle"))
        {
            PanelManager.GetInstance().ShowPanel("StartPanel", true, () => { Debug.LogError("嘿嘿"); });
            //AssetBundleManifest manifest;
            //string rootPath = "/Users/slade_zhou/Documents/MyProject/LFrameWork/LFrameWork/DataMAC/DataMAC/AssetBundle/AssetBundle";
            //AssetBundle ab = AssetBundle.LoadFromFile(rootPath);
            //if (ab == null)
            //{
            //    Debug.LogError("Load Manifest failed." + rootPath);
            //}
            //manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //if (manifest == null)
            //{
            //    ab.Unload(true);
            //    Debug.LogError("Load Manifest failed.");
            //}
            //AssetBundle ab=AssetBundle.LoadFromFile
            //particle = ParticleManager.GetInstance().GetParticleInstance("fx_01_boss_quidola_skill_s_ro", EParticleType.KiteTrailEffect);
            //particle.AddLoadedCallBack((effect) => {
            //    particle.Play();
            //    Debug.LogError("haha");
            //});
        }
        if (GUI.Button(new Rect(150, 0, 100, 100), "delete particle"))
        {
            PanelManager.GetInstance().ShowPanel("StartPanel", false, () => { Debug.LogError("嘿嘿false"); });
            //ParticleManager.GetInstance().AddCache(particle);
        }

        if (GUI.Button(new Rect(300, 0, 100, 100), "clear particle"))
        {
            PanelManager.GetInstance().DestroyPanel("StartPanel");
            //ParticleManager.GetInstance().ClearAll();
            //ParticleResMgr.GetInstance().ClearAllUseless();
            //Resources.UnloadUnusedAssets();
        }
    }
}
