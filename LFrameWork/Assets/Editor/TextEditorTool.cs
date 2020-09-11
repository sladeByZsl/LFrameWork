
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// The text editor tool.
    /// </summary>
    public static class TextEditorTool
    {
        private static ExternalTool intelliJIdea;
        private static ExternalTool sublimeText;
        private static ExternalTool visualStudioCode;
        private static int activeIndex;
        private static string[] displayOptions = new string[]
        {
            "IntelliJ IDEA",
            "Sublime Text",
            "Visual Studio Code",
        };

        /// <summary>
        /// Open a file by active text editor tool for specify row and column.
        /// </summary>
        public static bool OpenText(string path, int row, int column)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                UnityEngine.Debug.LogErrorFormat(
                    "The file {0} is not exitsed.", path);
                return false;
            }

            var idea = intelliJIdea.ToolPath;
            if (activeIndex == 0 && !string.IsNullOrEmpty(idea))
            {
                var cmd = $"\"{fullPath}\":{row}";
                Process.Start(idea, cmd);
                return true;
            }

            var sublime = sublimeText.ToolPath;
            if (activeIndex == 1 && !string.IsNullOrEmpty(sublime))
            {
                var cmd = $"\"{fullPath}\":{row}";
                Process.Start(sublime, cmd);
                return true;
            }

            var vscode = visualStudioCode.ToolPath;
            if (activeIndex == 2 && !string.IsNullOrEmpty(vscode))
            {
                var cmd = $"--goto \"{fullPath}\":{row}:{column}";
                Process.Start(vscode, cmd);
                return true;
            }

            var file = AssetDatabase.LoadAssetAtPath<UnityObject>(path);
            return AssetDatabase.OpenAsset(file, row);
        }

        /// <summary>
        /// Open a project by active text editor tool.
        /// </summary>
        public static bool OpenProject(string path)
        {
            var fullPath = Path.GetFullPath(path);

            var idea = intelliJIdea.ToolPath;
            if (activeIndex == 0 && !string.IsNullOrEmpty(idea))
            {
#if UNITY_EDITOR_OSX
                var cmd = $"\"{idea}\" --args \"{fullPath}\"";
                Process.Start("open", cmd);
#else
                var cmd = $"\"{fullPath}\"";
                Process.Start(idea, cmd);
#endif
                return true;
            }

            var sublime = sublimeText.ToolPath;
            if (activeIndex == 1 && !string.IsNullOrEmpty(sublime))
            {
#if UNITY_EDITOR_OSX
                var cmd = $"\"{sublime}\" --args \"{fullPath}\"";
                Process.Start("open", cmd);
#else
                var cmd = $"\"{fullPath}\"";
                Process.Start(sublime, cmd);
#endif
                return true;
            }

            var vscode = visualStudioCode.ToolPath;
            if (activeIndex == 2 && !string.IsNullOrEmpty(vscode))
            {
#if UNITY_EDITOR_OSX
                var cmd = $"\"{vscode}\" --args \"{fullPath}\"";
                Process.Start("open", cmd);
#else
                var cmd = $"\"{fullPath}\"";
                Process.Start(vscode, cmd);
#endif
                return true;
            }

            EditorUtility.RevealInFinder(fullPath);
            return true;
        }

        /// <summary>
        /// Draw the TextEditor selection GUI.
        /// </summary>
        internal static void DrawToolGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Editor:");
            EditorGUI.BeginChangeCheck();
            activeIndex = EditorGUILayout.Popup(activeIndex, displayOptions);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("FunPlus.TextEditor.ActiveIndex", activeIndex);
            }

            EditorGUILayout.EndHorizontal();

            switch (activeIndex)
            {
            case 0:
                intelliJIdea.DrawToolGUI();
                break;
            case 1:
                sublimeText.DrawToolGUI();
                break;
            case 2:
                visualStudioCode.DrawToolGUI();
                break;
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            var searchPaths = new string[]
            {
                "C:\\Program Files\\JetBrains\\IntelliJ IDEA Community Edition 2018.2.2\\bin\\idea64.exe",
                "D:\\Program Files\\JetBrains\\IntelliJ IDEA Community Edition 2018.2.2\\bin\\idea64.exe",
                "E:\\Program Files\\JetBrains\\IntelliJ IDEA Community Edition 2018.2.2\\bin\\idea64.exe",
                "F:\\Program Files\\JetBrains\\IntelliJ IDEA Community Edition 2018.2.2\\bin\\idea64.exe",
            };

            intelliJIdea = new ExternalTool(
                "FunPlus.External.IntelliJIdea",
                "IntelliJ IDEA",
                searchPaths);

            searchPaths = new string[]
            {
#if UNITY_EDITOR_OSX
                "/Applications/Sublime Text.app",
#else
                "C:\\Sublime Text 3\\sublime_text.exe",
                "C:\\Program Files\\Sublime Text 3\\sublime_text.exe",
                "C:\\Program Files (x86)\\Sublime Text 3\\sublime_text.exe",
                "D:\\Sublime Text 3\\sublime_text.exe",
                "D:\\Program Files\\Sublime Text 3\\sublime_text.exe",
                "D:\\Program Files (x86)\\Sublime Text 3\\sublime_text.exe",
                "E:\\Sublime Text 3\\sublime_text.exe",
                "E:\\Program Files\\Sublime Text 3\\sublime_text.exe",
                "E:\\Program Files (x86)\\Sublime Text 3\\sublime_text.exe",
                "F:\\Sublime Text 3\\sublime_text.exe",
                "F:\\Program Files\\Sublime Text 3\\sublime_text.exe",
                "F:\\Program Files (x86)\\Sublime Text 3\\sublime_text.exe",
#endif
            };

            sublimeText = new ExternalTool(
                "FunPlus.External.SublimeText",
                "Sublime Text",
                searchPaths);

            searchPaths = new string[]
            {
#if UNITY_EDITOR_OSX
                "/Applications/Visual Studio Code.app",
#else
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Microsoft VS Code\\Code.exe"),
                "C:\\Program Files\\Microsoft VS Code\\Code.exe",
                "C:\\Program Files (x86)\\Microsoft VS Code\\Code.exe",
                "D:\\Program Files\\Microsoft VS Code\\Code.exe",
                "D:\\Program Files (x86)\\Microsoft VS Code\\Code.exe",
                "E:\\Program Files\\Microsoft VS Code\\Code.exe",
                "E:\\Program Files (x86)\\Microsoft VS Code\\Code.exe",
                "F:\\Program Files\\Microsoft VS Code\\Code.exe",
                "F:\\Program Files (x86)\\Microsoft VS Code\\Code.exe",
#endif
            };

            visualStudioCode = new ExternalTool(
                "FunPlus.External.VisualStudioCode",
                "Visual Studio Code",
                searchPaths);

            activeIndex = EditorPrefs.GetInt(
                "FunPlus.TextEditor.ActiveIndex", 0);
        }
    }
