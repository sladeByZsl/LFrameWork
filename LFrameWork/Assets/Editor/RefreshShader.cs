using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RefreshShader 
{
    [MenuItem("Build/RefreshMat", false, 501)]
    static void RefreshMat()
    {
        EditorUtility.DisplayProgressBar("刷新材质", "正在刷新材质", 0);
        var guids = AssetDatabase.FindAssets("t:Material");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.ToLower().EndsWith("mat"))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat && mat.shader)
                {
                    Debug.LogFormat("{0}\n{1}\n{2}\n{3}\n", path, mat.shader.name,
                        mat.shader.GetInstanceID(),
                        Shader.Find(mat.shader.name).GetInstanceID());
                    mat.shader = Shader.Find(mat.shader.name);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("ok", "成功", "好的");
    }
}
