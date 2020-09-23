using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Light mLight;
    private List<Material> mMatList = new List<Material>();
    // Start is called before the first frame update
    void Start()
    {

        SkinnedMeshRenderer[] renderlist = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var render in renderlist)
        {
            if (render == null)
                continue;

            mMatList.Add(render.material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShader();
    }

    private void UpdateShader()
    {
        Vector4 worldpos = transform.position;

        //Vector4 projdir = new Vector4(-0.06323785f, -0.9545552f, -0.2912483f, 1.0f);
        //mLight.transform.rotation = Quaternion.LookRotation(projdir);

        Vector4 projdir = mLight.transform.forward;

        Vector4 _LightDir = new Vector4(projdir.x, projdir.y, projdir.z, 0.1f);

        foreach (var mat in mMatList)
        {
            if (mat == null)
                continue;
            mat.SetVector("_LightDir", _LightDir);
            //mat.SetVector("_WorldPos", worldpos);
            //mat.SetVector("_ShadowProjDir", projdir);
            //mat.SetVector("_ShadowPlane", new Vector4(0.0f, 1.0f, 0.0f, 0.1f));
            //mat.SetVector("_ShadowFadeParams", new Vector4(0.0f, 1.5f, 0.7f, 0.0f));
            //mat.SetFloat("_ShadowFalloff", 1.35f);
        }
    }
}
