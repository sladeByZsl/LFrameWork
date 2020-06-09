using System.Collections;
using System.Collections.Generic;
using LFrameWork.Common;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        {
            ParticleInstance particle = ParticleManager.GetInstance().GetParticleInstance("fx_01_boss_quidola_skill_s_ro", EParticleType.KiteTrailEffect);
            particle.AddLoadedCallBack((effect) => {
                Debug.LogError("haha");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
