using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using TextureCompressionQuality = UnityEngine.TextureCompressionQuality;

namespace FunPlus.Resources
{
	public enum TextureType
	{
		DEFAULT =  0,
		UICommon =  1,
		UIIcon =  2,
        UITexture =  3,
        Spine =  4,
        ParticleTex=  5,
        Particle512Tex =  6,
        SceneTex = 7,
        SceneSpriteTex = 8,
        SceneNormalTex =  9,
        TerrainMask =  10,
        ParticleSpecTex = 11,
        UISpecTex = 12,
        SceneCubTex = 13,
        SpeSpine = 14,
        ModelNormalTex = 15,
        ModelTex = 16,
    }


    public class TextureImportUtil
    {
        private static int textureMaxSize(TextureType type)
        {
            if (type == TextureType.DEFAULT  || type == TextureType.Spine)
            {
                return 1024;
            }
            else if (type == TextureType.UISpecTex || type == TextureType.SpeSpine)
            {
                return 2048;
            }
            else if (type == TextureType.UICommon)
            {
                return 1024;
            }
            else if ((type == TextureType.UIIcon) ||
                (type == TextureType.UITexture))
            {
                return 1024;
            }
            else if (type == TextureType.ParticleTex)
            {
                return 256;
            }
            else if (type == TextureType.Particle512Tex)
            {
                return 512;
            }
            else if (type == TextureType.SceneTex || type == TextureType.ModelTex)
            {
                return 1024;
            }
            else if (type == TextureType.SceneSpriteTex)
            {
                return 1024;
            }
            else if (type == TextureType.SceneNormalTex || type == TextureType.ModelNormalTex)
            {
                return 1024;
            }
            else if (type == TextureType.TerrainMask)
            {
                return 1024;
            }
            else if (type == TextureType.SceneCubTex)
            {
                return 256;
            }
            return 2048;
        }

        public static void TryResetTextureImporter(TextureImporter importer, TextureType type, string assetPath)
        {
            if(importer == false)
            {
                return;
            }

            //�Ƿ��ͨ��
            bool hasAlpha = false;

            if (importer.alphaSource != TextureImporterAlphaSource.None)
            {
                hasAlpha = importer.DoesSourceTextureHaveAlpha() || importer.alphaSource == TextureImporterAlphaSource.FromGrayScale;
            }

            if (type == TextureType.UICommon)
            {
                hasAlpha = true;
            }

            if (hasAlpha)
            {
                if (type == TextureType.TerrainMask)
                {
                    importer.alphaIsTransparency = false;
                }
                else
                    importer.alphaIsTransparency = true;
            }

           
                // ��������
            if (type == TextureType.UICommon)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                string atlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name.ToLower();
                if (string.IsNullOrEmpty(atlasName) == false)
                {
                    importer.spritePackingTag = atlasName;
                }
                else
                {
                    // Debug.LogError();
                }
            }
            else if (type == TextureType.SceneSpriteTex)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePackingTag = string.Empty;
            }
            else if (type == TextureType.SceneNormalTex || type == TextureType.ModelNormalTex)
            {
                importer.textureType = TextureImporterType.NormalMap;
            }
            
            else
            {
                importer.textureType = TextureImporterType.Default;
                importer.spritePackingTag = string.Empty;
            }


            
            //����������
            if(type == TextureType.SceneSpriteTex || type == TextureType.SceneTex || type == TextureType.SceneCubTex)
            {
                importer.mipmapEnabled = true;
                importer.streamingMipmaps = true;
            }
            else if (type == TextureType.ModelNormalTex ||type == TextureType.ModelTex)
            {
                importer.mipmapEnabled = true;
                importer.streamingMipmaps = false;
            }
            else
            {
                importer.mipmapEnabled = false;
                importer.streamingMipmaps = false;
            }
            
            importer.streamingMipmaps = false;
            if(type == TextureType.UIIcon || type == TextureType.SceneCubTex)
            {
                importer.npotScale = TextureImporterNPOTScale.ToNearest;
            }
            else
            {
                importer.npotScale = TextureImporterNPOTScale.None;
            }
             
            importer.isReadable = false;
           // importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            if (type == TextureType.SceneCubTex)
            {
                importer.textureShape = TextureImporterShape.TextureCube;
            }
            else
            {
                importer.textureShape = TextureImporterShape.Texture2D;
            }
            
            importer.maxTextureSize = textureMaxSize(type);

            var standardAndroidImporter = GetAndroidStandardImportSetting(importer, type, hasAlpha);
            importer.SetPlatformTextureSettings(standardAndroidImporter);
            var standardIOSImporter = GetIOSStandardImportSetting(importer, type, hasAlpha);
            importer.SetPlatformTextureSettings(standardIOSImporter);
            var standardWinImporter = GetWinStandardImportSetting(importer, type, hasAlpha);
            importer.SetPlatformTextureSettings(standardWinImporter);
        }

        public static TextureImporterPlatformSettings GetIOSStandardImportSetting(TextureImporter importer,
            TextureType type, bool hasAlpha)
        {
            TextureImporterCompression compression = TextureImporterCompression.Compressed;
            var format = hasAlpha ? TextureImporterFormat.ASTC_RGBA_6x6 : TextureImporterFormat.ASTC_RGB_6x6;
            if (type == TextureType.TerrainMask)
            {
                compression = TextureImporterCompression.CompressedHQ;
                
            }

            TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            if (type == TextureType.Spine || type == TextureType.SpeSpine)
            {
                resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
            }

            var ti = new TextureImporterPlatformSettings
            {
                name = "iPhone",
                overridden = true,
                format = format,
                maxTextureSize = textureMaxSize(type),
                compressionQuality = GetTextureCompressQuality(type),
                textureCompression = compression,
                resizeAlgorithm = resizeAlgorithm
            };
            return ti;
        }

        public static TextureImporterPlatformSettings GetAndroidStandardImportSetting(TextureImporter importer,
            TextureType type,
            bool hasAlpha)
        {

            TextureImporterCompression compression = TextureImporterCompression.Compressed;
            TextureImporterFormat format = hasAlpha ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
            if (type == TextureType.TerrainMask)
            {
                compression = TextureImporterCompression.CompressedHQ;                
            }
            TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            if (type == TextureType.Spine || type == TextureType.SpeSpine)
            {
                resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
            }

            var ti = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                format = format,
                textureCompression = compression,
                maxTextureSize = textureMaxSize(type),
                compressionQuality = GetTextureCompressQuality(type),
                androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit,
                resizeAlgorithm = resizeAlgorithm
            };

            return ti;
        }

        public static TextureImporterPlatformSettings GetWinStandardImportSetting(TextureImporter importer,
            TextureType type, bool hasAlpha)
        {
            TextureImporterCompression compression = TextureImporterCompression.Compressed;
            var format = hasAlpha ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB24;
            TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            if (type == TextureType.Spine || type == TextureType.SpeSpine)
            {
                resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
            }
            if (type == TextureType.TerrainMask)
            {
                compression = TextureImporterCompression.CompressedHQ;
                format = TextureImporterFormat.BC7;
            }

            var ti = new TextureImporterPlatformSettings
            {
                name = "Standalone",
                overridden = true,
                format = format,
                textureCompression = compression,
                maxTextureSize = textureMaxSize(type),
                compressionQuality = GetTextureCompressQuality(type),
                resizeAlgorithm = resizeAlgorithm
            };

            return ti;
        }

        private static int GetTextureCompressQuality(TextureType type)
        {
            return (int)TextureCompressionQuality.Normal;
        }
    }
		
}