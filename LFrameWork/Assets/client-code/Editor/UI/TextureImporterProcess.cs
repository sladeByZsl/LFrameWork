using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public enum TextureType
{
    Default,
    UICommon,
    UITexture,
    UITextureNoAlpha,
    UIIcon
}

public class TextureImporterProcess : AssetPostprocessor
{
    #region UI贴图
    List<string> atlasPath = new List<string>() { "Assets/Arts/ui/common" };//图集目录
    List<string> textureWithAlpha = new List<string>() { "Assets/Arts/ui/texture" };//Texture目录带alpha通道
    List<string> textureNoAlpha = new List<string>() { "Assets/Arts/ui/texture_NoAlpha" };//Texture目录没有alpha通道
    List<string> iconPath = new List<string>() { "Assets/Arts/ui/icon" };//Icon的路径,用于动态图集
    #endregion

    #region 场景贴图

    #endregion

    void OnPreprocessTexture()
    {
        //获得importer实例
        TextureImporter tImporter = assetImporter as TextureImporter;

        TextureType textureType = GetTextureType(tImporter.assetPath);

        bool hasAlpha = false;
        if (tImporter.alphaSource != TextureImporterAlphaSource.None)
        {
            hasAlpha = tImporter.DoesSourceTextureHaveAlpha() || tImporter.alphaSource == TextureImporterAlphaSource.FromGrayScale;
        }


        if (textureType == TextureType.UICommon)
        {
            tImporter.textureType = TextureImporterType.Sprite;
            tImporter.spriteImportMode = SpriteImportMode.Single;
            string atlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name.ToLower();
            tImporter.spritePackingTag = atlasName;
            //设置Read/Write Enabled开关,不勾选
            tImporter.isReadable = false;
            tImporter.alphaIsTransparency = hasAlpha;
            //设置UI纹理Generate Mipmaps开关,不勾选
            tImporter.mipmapEnabled = false;
            tImporter.streamingMipmaps = false;
            //设置UI纹理WrapMode开关,Clamp
            tImporter.wrapMode = TextureWrapMode.Clamp;
        }
        else if (textureType == TextureType.UITexture || textureType == TextureType.UITextureNoAlpha || textureType == TextureType.UIIcon)
        {
            tImporter.textureType = TextureImporterType.Default;
            tImporter.alphaIsTransparency = hasAlpha;
            if (textureType == TextureType.UIIcon)
            {
                tImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            }
            tImporter.isReadable = false;
            //设置UI纹理Generate Mipmaps开关,不勾选
            tImporter.mipmapEnabled = false;
            tImporter.streamingMipmaps = false;
        }

        var standardAndroidImporter = GetAndroidStandardImportSetting(tImporter, textureType, hasAlpha);
        tImporter.SetPlatformTextureSettings(standardAndroidImporter);
        var standardIOSImporter = GetIOSStandardImportSetting(tImporter, textureType, hasAlpha);
        tImporter.SetPlatformTextureSettings(standardIOSImporter);
        var standardWinImporter = GetWinStandardImportSetting(tImporter, textureType, hasAlpha);
        tImporter.SetPlatformTextureSettings(standardWinImporter);
    }


    public static TextureImporterPlatformSettings GetIOSStandardImportSetting(TextureImporter importer,
        TextureType type, bool hasAlpha)
    {
        TextureImporterCompression compression = TextureImporterCompression.Compressed;
        var format = hasAlpha ? TextureImporterFormat.ASTC_RGBA_6x6 : TextureImporterFormat.ASTC_RGB_6x6;
        TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        var ti = new TextureImporterPlatformSettings
        {
            name = "iPhone",
            overridden = true,
            format = format,
            textureCompression = compression,
            maxTextureSize = textureMaxSize(type),
            compressionQuality = GetTextureCompressQuality(),
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
        TextureResizeAlgorithm resizeAlgorithm = TextureResizeAlgorithm.Mitchell;

        var ti = new TextureImporterPlatformSettings
        {
            name = "Android",
            overridden = true,
            format = format,
            textureCompression = compression,
            maxTextureSize = textureMaxSize(type),
            compressionQuality = GetTextureCompressQuality(),
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

        var ti = new TextureImporterPlatformSettings
        {
            name = "Standalone",
            overridden = true,
            format = format,
            textureCompression = compression,
            maxTextureSize = textureMaxSize(type),
            compressionQuality = GetTextureCompressQuality(),
            resizeAlgorithm = resizeAlgorithm
        };

        return ti;
    }

    private static int textureMaxSize(TextureType type)
    {
        if (type == TextureType.Default ||
            type == TextureType.UICommon ||
            type == TextureType.UITexture ||
            type == TextureType.UITextureNoAlpha ||
            type == TextureType.UIIcon)
        {
            return 1024;
        }
        else
        {
            return 1024;
        }
    }

    private static int GetTextureCompressQuality()
    {
        return (int)UnityEngine.TextureCompressionQuality.Normal;
    }

    public TextureType GetTextureType(string path)
    {
        if (IsPathValid(atlasPath, path))
        {
            return TextureType.UICommon;
        }
        else if (IsPathValid(textureWithAlpha, path))
        {
            return TextureType.UITexture;
        }
        else if (IsPathValid(textureNoAlpha, path))
        {
            return TextureType.UITextureNoAlpha;
        }
        else if (IsPathValid(iconPath, path))
        {
            return TextureType.UITextureNoAlpha;
        }
        return TextureType.Default;
    }

    public bool IsPathValid(List<string> collectionList, string path)
    {
        for (int i = 0; i < collectionList.Count; i++)
        {
            if (path.StartsWith(collectionList[i]))
            {
                return true;
            }
        }
        return false;
    }

    [MenuItem("校验工具/1-检测UI规范")]
    static public void AutoValidate()
    {
        //写入csv日志
        StreamWriter sw = new StreamWriter("Validate.csv", false, System.Text.Encoding.UTF8);
        sw.WriteLine("1-检测UI规范");

        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        foreach (string s in allAssets)
        {
            if (s.StartsWith("Assets/"))
            {
                Texture tex = AssetDatabase.LoadAssetAtPath(s, typeof(Texture)) as Texture;

                if (tex)
                {
                    //检测纹理资源命名是否合法
                    if (!Regex.IsMatch(s, @"^[a-zA-Z][a-zA-Z0-9_/.]*$"))
                    {
                        sw.WriteLine(string.Format("illegal texture filename,{0}", s));
                    }

                    //判断纹理尺寸是否符合四的倍数
                    if (((tex.width % 4) != 0) || ((tex.height % 4) != 0))
                    {
                        sw.WriteLine(string.Format("illegal texture W/H size,{0},{1},{2}", s, tex.width, tex.height));
                    }
                }
            }
        }

        sw.Flush();
        sw.Close();
        EditorUtility.DisplayDialog("检测UI规范", "UI检测完毕", "确定");
    }
}