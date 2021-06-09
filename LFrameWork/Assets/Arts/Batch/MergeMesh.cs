using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MergeMeshMethod();
    }

    public void MergeMeshMethod()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for(int i=0;i<meshFilters.Length;i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;//本地坐标转换成世界坐标的矩阵
        }
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combineInstances);
        gameObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
