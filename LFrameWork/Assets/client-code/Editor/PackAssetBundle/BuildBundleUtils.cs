
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;


public partial class BuildBundleUtils
{

    public static Material[] GenerateShaderFeatureTempletes(string[] mats)
    {
        if ( mats == null )
        {
            return null;
        }

        Dictionary<Shader, Dictionary<string, List<string>>> shaderKeywords = new Dictionary<Shader, Dictionary<string, List<string>>>();

        // 收集所有shaderKeyWord
        for (int i = 0; i < mats.Length; ++i)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mats[i]);
            if ( mat == null)
            {
                Debug.LogError("BuildBundleUtils.GenerateShaderFeatureTempletes load material faild " + mats[i]);
                continue;
            }
            if ( mat.shader == null )
            {
                Debug.LogError("BuildBundleUtils.GenerateShaderFeatureTempletes shader is null " + mats[i], mat);
                continue;
            }

            string kwStr = string.Empty;
            List<string> enableKWs = new List<string>();

            string[] keywords = mat.shaderKeywords;
            for ( int ikw = 0; ikw < keywords.Length; ++ikw )
            {
                if ( mat.IsKeywordEnabled(keywords[ikw]) )
                {
                    enableKWs.Add(keywords[ikw]);
                    kwStr += keywords[ikw];
                }
            }

            if ( enableKWs.Count > 0 )
            {
                Dictionary<string, List<string>> kwDic = null;

                if (!shaderKeywords.TryGetValue(mat.shader, out kwDic))
                {
                    kwDic = new Dictionary<string, List<string>>();
                    shaderKeywords.Add(mat.shader, kwDic);
                }

                kwDic[kwStr] = enableKWs;
            }
        }


        List<Material> materialTempletes = new List<Material>();

        int shaderIndex = 0;
        for (var e = shaderKeywords.GetEnumerator(); e.MoveNext(); ++shaderIndex)
        {
            Shader shader = e.Current.Key;
            Dictionary<string, List<string>> kwDic = e.Current.Value;

            int kwIndex = 0;
            for (var eKW = kwDic.GetEnumerator(); eKW.MoveNext(); ++kwIndex)
            {
                List<string> enbleKWs = eKW.Current.Value;

                Material mat = new Material(shader);
                mat.name = string.Format("KeywordTemplete_{0}_{1}", shaderIndex, kwIndex);

                for (int i = 0; i < mat.shaderKeywords.Length; ++i)
                {
                    string kw = mat.shaderKeywords[i];
                    if (enbleKWs.Contains(kw))
                    {
                        mat.EnableKeyword(kw);
                    }
                    else
                    {
                        mat.DisableKeyword(kw);
                    }
                }
                materialTempletes.Add(mat);
            }
        }
    
        return materialTempletes.ToArray();
    }
}
