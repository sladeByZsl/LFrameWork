using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        //1.material  改变属性的物体不参与合批,但是具体怎么组织合并的关系，对于静态合批，取决于渲染顺序    动态合批没影响
        //render.material.color = Color.red;
        //2.可以合批
        //render.sharedMaterial.color = Color.red;
        //3.改变属性的物体不参与合批,其他未改变的依然可以合批
        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        render.GetPropertyBlock(prop);
        prop.SetColor("_TintColor", Color.red);
        render.SetPropertyBlock(prop);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
