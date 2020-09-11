
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The external tool.
/// </summary>
internal sealed class ExternalTool
{
    private string key;
    private string name;
    private string[] searchPaths;
    private string toolPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalTool"/> class.
    /// </summary>
    internal ExternalTool(string key, string name, string[] searchPaths)
    {
        this.key = key;
        this.name = name;
        this.searchPaths = searchPaths;
    }

    /// <summary>
    /// Gets the tool path.
    /// </summary>
    internal string ToolPath
    {
        get
        {
            // Try to find the tool path from EditorPrefs.
            if (string.IsNullOrEmpty(this.toolPath))
            {
                if (EditorPrefs.HasKey(this.key))
                {
                    var path = EditorPrefs.GetString(this.key);
                    if (File.Exists(path))
                    {
                        this.toolPath = path;
                    }
                    else
                    {
                        EditorPrefs.DeleteKey(this.key);
                    }
                }
            }

            // Try to search the tool path.
            if (string.IsNullOrEmpty(this.toolPath))
            {
                this.FindToolPath();
            }

            return this.toolPath;
        }
    }

    /// <summary>
    /// Draw the tool GUI.
    /// </summary>
    internal void DrawToolGUI()
    {
        var toolPath = this.ToolPath;
        var label = string.Empty;
        var toolName = this.name;
        var displayName = GetAppFriendlyName(toolPath);
        if (toolName == displayName)
        {
            label = $"{toolName}:";
        }
        else
        {
            label = $"{toolName}: {displayName}";
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(225));
        if (GUILayout.Button("Search..."))
        {
            this.FindToolPath();
        }

        if (GUILayout.Button("Browse..."))
        {
#if UNITY_EDITOR_OSX
                const string Suffix = "app";
#else
            const string Suffix = "exe";
#endif

            var text = EditorUtility.OpenFilePanel(
                "Browse for application",
                string.Empty,
                Suffix);
            if (text.Length != 0)
            {
                this.SetToolPath(text);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField(toolPath);
    }

    private void SetToolPath(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        this.toolPath = path;
        EditorPrefs.SetString(this.key, path);
    }

    private void FindToolPath()
    {
        foreach (var path in this.searchPaths)
        {
#if UNITY_EDITOR_OSX
                if (Directory.Exists(path))
#else
            if (File.Exists(path))
#endif
            {
                this.toolPath = path;
                EditorPrefs.SetString(this.key, path);
            }
        }
    }

    private string GetAppFriendlyName(string app)
    {
        var unityEditorAssembly =
            Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
        var utilType = unityEditorAssembly.GetType("UnityEditor.OSUtil");
        var method = utilType.GetMethod("GetAppFriendlyName");
        return (string)method.Invoke(null, new object[] { app });
    }
}
