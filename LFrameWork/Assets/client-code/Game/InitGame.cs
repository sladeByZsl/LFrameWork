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

    List<MyParticleInstance> particleInstanceList;

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
            //var shaderRequeset = Assets.LoadShader();
            //yield return shaderRequeset;
                 
            Init();
        }
        else
        {
            Debug.LogError("初始化失败");
        }
    }

    private void Init()
    {
        GameObjectPoolManager.GetInstance().Init();
        MyParticleManager.GetInstance().Init();
        particleInstanceList = new List<MyParticleInstance>();

        //Assets.LoadAssetAsync("shaders", typeof(UnityEngine.Object)).completed += delegate (AssetRequest request)
        //{
        //    if (!string.IsNullOrEmpty(request.error))
        //    {
        //        Debug.LogError(request.error);
        //        return;
        //    }
        //    request.Retain();
        //};
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
    private bool IsScene(string asset)
    {
        return asset.EndsWith(".unity");
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"add particle"))
        {
            MyParticleInstance particleInstance=MyParticleManager.GetInstance().GetParticleInstance("fx_01_boss_quidola_skill_s_ro", () => {
                Debug.LogError("load ok");
            });
            //PanelManager.GetInstance().ShowPanel("StartPanel", true, () => { Debug.LogError("嘿嘿"); });
            //particleInstanceList.Add(particleInstance);
            //Scheduler.Delay(3, () => {
            //    Debug.LogError("嘿嘿");
            //    MyParticleManager.GetInstance().AddParticleToCache(particleInstance);
            //});


            //List<string> ls = new List<string>();
            //ls.Add("apple1");
            //ls.Add("apple2.unity");
            //ls.Add("apple3");
            //ls.Add("apple4");
            //ls.Add("apple5");

            //if(ls.Exists(IsScene)&&!ls.TrueForAll(IsScene))
            //{
            //    Debug.LogError("1");
            //}
            //else
            //{
            //    Debug.LogError("2");
            //}
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
            if(particleInstanceList.Count>0)
            {
                MyParticleInstance particleInstance = particleInstanceList[particleInstanceList.Count - 1];
                particleInstanceList.RemoveAt(particleInstanceList.Count - 1);
                MyParticleManager.GetInstance().AddParticleToCache(particleInstance);
            }
            else
            {
                Debug.LogError("沒啦");
            }
          
            //PanelManager.GetInstance().ShowPanel("StartPanel", false, () => { Debug.LogError("嘿嘿false"); });
            //ParticleManager.GetInstance().AddCache(particle);
        }

        //if (GUI.Button(new Rect(300, 0, 100, 100), "clear particle"))
        {
            //PanelManager.GetInstance().DestroyPanel("StartPanel");
            //ParticleManager.GetInstance().ClearAll();
            //ParticleResMgr.GetInstance().ClearAllUseless();
            //Resources.UnloadUnusedAssets();
        }
    }
}
