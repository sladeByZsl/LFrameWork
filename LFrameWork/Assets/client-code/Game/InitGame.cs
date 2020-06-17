using System.Collections;
using System.Collections.Generic;
using LFrameWork.Common;
using UnityEngine;
using LFrameWork.Common.Resource;

public class InitGame : MonoBehaviour
{
    ParticleInstance particle;
    // Start is called before the first frame update
    void Start()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        {
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"add particle"))
        {
            AssetBundleManifest manifest;
            string rootPath = "/Users/slade_zhou/Documents/MyProject/LFrameWork/LFrameWork/DataMAC/DataMAC/AssetBundle/AssetBundle";
            AssetBundle ab = AssetBundle.LoadFromFile(rootPath);
            if (ab == null)
            {
                Debug.LogError("Load Manifest failed." + rootPath);
            }
            manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null)
            {
                ab.Unload(true);
                Debug.LogError("Load Manifest failed.");
            }
            //AssetBundle ab=AssetBundle.LoadFromFile
            //particle = ParticleManager.GetInstance().GetParticleInstance("fx_01_boss_quidola_skill_s_ro", EParticleType.KiteTrailEffect);
            //particle.AddLoadedCallBack((effect) => {
            //    particle.Play();
            //    Debug.LogError("haha");
            //});
        }
        if (GUI.Button(new Rect(150, 0, 100, 100), "delete particle"))
        {
            //ParticleManager.GetInstance().AddCache(particle);
        }

        if (GUI.Button(new Rect(300, 0, 100, 100), "clear particle"))
        {
            //ParticleManager.GetInstance().ClearAll();
            //ParticleResMgr.GetInstance().ClearAllUseless();
            //Resources.UnloadUnusedAssets();
        }
    }
}
