using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;

namespace FunPlus.Resources
{
    public class ResPostProcess : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            // return;
            if (assetImporter == null)
            {
                return;
            }

            TextureImporter textureImporter = assetImporter as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            TextureType textureType = TextureType.DEFAULT;
            string texPath = assetPath.ToLower();
            if (texPath.Contains("scene/special"))
            {
                return;
            }

            string fileName = System.IO.Path.GetFileNameWithoutExtension(texPath);
            int markCharIndex = fileName.LastIndexOf("_");
            string markChar = string.Empty;
            if (markCharIndex >= 0)
            {
                markChar = fileName.Substring(markCharIndex);
            }
           if (markChar == "_terrainmask")
            {
                textureType = TextureType.TerrainMask;
            }
            else if (texPath.Contains("/arts/ui/common") || texPath.Contains("/arts/ui/artfont"))
            {
                textureType = TextureType.UICommon;
            }
            else if (texPath.Contains("/arts/ui/icon"))
            {
                textureType = TextureType.UIIcon;
            }
            else if (texPath.Contains("/arts/ui/texture"))
            {
                if (texPath.Contains("/arts/ui/texture/special"))
                {
                    textureType = TextureType.UISpecTex;
                }
                else
                {
                    textureType = TextureType.UITexture;
                }
            }
            else if (texPath.Contains("/arts/spine/"))
            {
                if (texPath.Contains("/special/"))
                {
                    textureType = TextureType.SpeSpine;
                }
                else
                {
                    textureType = TextureType.Spine;
                }
            }
           else if (texPath.Contains("/arts/model/"))
           {
               if (markChar == "_n")
               {
                   textureType = TextureType.ModelNormalTex;
               }
               else
               {
                   textureType = TextureType.ModelTex;
               }
           }
            else if (texPath.Contains("/arts/scene"))
            {
                if (texPath.Contains( "/arts/scene/city/"))
                {
                    return;
                }
               
                else if (markChar == "_2d")
                {
                    textureType = TextureType.SceneSpriteTex;
                }
                else if (markChar == "_n")
                {
                    textureType = TextureType.SceneNormalTex;
                }
                else if (markChar == "_reflectionprobe")
                {
                    textureType = TextureType.SceneCubTex;
                }
                else
                {
                    textureType = TextureType.SceneTex;
                }
            }
            else if (texPath.Contains("/arts/effect/res/texture"))
            {
                if (texPath.Contains("/arts/effect/res/texture/512"))
                {
                    textureType = TextureType.Particle512Tex;
                }
                else if (texPath.Contains("/arts/effect/res/texture/special"))
                {
                    textureType = TextureType.ParticleSpecTex;
                }
                else
                {
                    textureType = TextureType.ParticleTex;
                }
            }
            else
            {
                Debug.LogError(string.Format("{0}----Unknown Hook Texture Type !!!", texPath));
                textureType = TextureType.DEFAULT;
                return;
            }

            TextureImportUtil.TryResetTextureImporter(textureImporter, textureType, texPath);
            //AssetDatabase.Refresh();
        }



        void OnPreprocessModel()
        {
            ModelImporter mi = (ModelImporter)assetImporter;
            if (mi == null)
            {
                return;
            }

            // 所有的fbx文件，不导入material;
            mi.importMaterials = false;
            string modelPath = assetPath.ToLower();
            if(modelPath.Contains("/special/"))
            {
                return;
            }
            if (modelPath.Contains("/arts/effect/res/mesh"))
            {
                mi.importNormals = ModelImporterNormals.None;
            }
        }

        public void OnPostprocessModel(GameObject model)
        {
            Renderer[] renders = model.GetComponentsInChildren<Renderer>();
            if (null != renders)
            {
                foreach (Renderer render in renders)
                {
                    render.sharedMaterials = new Material[render.sharedMaterials.Length];
                }
            }
            //AssetDatabase.Refresh();
            EditorUtility.SetDirty(model);
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                //Debug.Log(path);
                string assetPath = path.ToLower();
                string extension = System.IO.Path.GetExtension(assetPath);

                if (string.Equals(extension, ".prefab"))
                {
                    PrefabCheckTool.ApplyOptions(path);
                }
                else if (string.Equals(extension, ".controller") && assetPath.Contains("character/export/animation"))
                {
                    OnPreprocessAniController(path);
                }
            }

            foreach (string path in movedAssets)
            {
                string assetPath = path.ToLower();
                string extension = System.IO.Path.GetExtension(assetPath);
                if (string.Equals(extension, ".prefab"))
                {
                    PrefabCheckTool.ApplyOptions(path);
                }
                //Debug.LogWarning(path);
            }
        }

        public static void OnPreprocessAniController(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            if (animatorController == null)
            {
                return;
            }

            string overPath = assetPath.Replace(".controller", ".overrideController");

            AnimatorOverrideController overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(overPath);
            if (overrideController == null)
            {
                overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = animatorController;

                AssetDatabase.CreateAsset(overrideController, overPath);
            }
            else
            {
                overrideController.runtimeAnimatorController = animatorController;
            }
        }
    }
}

