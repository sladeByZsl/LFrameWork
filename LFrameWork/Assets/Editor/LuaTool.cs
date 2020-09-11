using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LuaTool
{
    public const string LuaScriptsFolder = "Assets";
    /// <summary>
    /// Open the lua project
    /// </summary>
    [MenuItem("Assets/Open Lua Project")]
    public static void OpenProject()
    {
        TextEditorTool.OpenProject(LuaScriptsFolder);
    }
}
