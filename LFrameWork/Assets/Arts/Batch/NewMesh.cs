using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMesh : MonoBehaviour
{
    public Transform cube1;
    public Transform cube2;
    public Transform a1;
    public Transform a2;
    public Transform a3;
    // Start is called before the first frame update
    void Start()
    {
        if (cube1.GetComponent<MeshFilter>().sharedMesh == cube2.GetComponent<MeshFilter>().sharedMesh)
        {
            Debug.LogError("mesh is the same!");
        }
        ChangeCube1Mesh();
    }

    void ChangeCube1Mesh()
    {
        Mesh mesh = cube1.GetComponent<MeshFilter>().sharedMesh; //这里是mesh
        mesh.Clear();
        Vector3 v1 = gameObject.transform.InverseTransformPoint(a1.position);
        Vector3 v2 = gameObject.transform.InverseTransformPoint(a2.position);
        Vector3 v3 = gameObject.transform.InverseTransformPoint(a3.position);
        mesh.vertices = new Vector3[] { v1, v2, v3 };
        mesh.triangles = new int[] { 0, 1, 2 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
